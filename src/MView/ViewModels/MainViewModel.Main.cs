﻿using Caliburn.Micro;
using MView.Utilities;
using MView.Utilities.Indexing;
using MView.Utilities.Text;
using MView.ViewModels.Dialogs;
using Ookii.Dialogs.Wpf;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MView.ViewModels
{
    public partial class MainViewModel : Screen, IDisposable
    {
        #region ::Variables::

        private Stopwatch _stopwatch = new Stopwatch();

        private Settings _settings = IoC.Get<Settings>();

        public Settings Settings
        {
            get => _settings;
            set => Set(ref _settings, value);
        }

        private bool _isWorking = false;

        public bool IsWorking
        {
            get => _isWorking;
            set => Set(ref _isWorking, value);
        }

        private string _statusText = "Status";

        public string StatusText
        {
            get => _statusText;
            set => Set(ref _statusText, value);
        }

        #endregion

        #region ::Constructors::

        public MainViewModel()
        {
            ConnectToLogBroker();

            // Initialize the IndexedItemsCVS : In MainViewModel.Explorer
            ItemCollectionViewSource.Source = IndexedItems;
            ItemCollectionViewSource.GroupDescriptions.Add(new PropertyGroupDescription("ParentDirectory"));
        }

        private void ConnectToLogBroker()
        {
            App.LogBroker.LogEmittedEvent += (log) =>
            {
                StatusText = log;
            };

            Log.Information("The Main VM has established a connection to the Log Broker.");
        }

        #endregion

        #region ::Workers::

        // Task
        public async void Encrypt()
        {
            try
            {
                EncryptorViewModel viewModel = IoC.Get<EncryptorViewModel>();
                bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

                if (dialogResult == true)
                {
                    Settings settings = IoC.Get<Settings>();

                    // Check the number of threads.
                    if (settings.NumberOfThreads < 0 || settings.NumberOfThreads > 10)
                    {
                        Log.Fatal("The number of threads out of range.");
                        return;
                    }

                    // Process
                    IsWorking = true;
                    _stopwatch.Restart(); // Start

                    // Get selected items.
                    List<IndexedItem> selectedItems;

                    if (viewModel.EncryptAllFileFormats)
                    {
                        selectedItems = IndexedItems.Where(p => p.IsSelected == true).ToList();
                    }
                    else
                    {
                        selectedItems = IndexedItems.Where(p => p.IsSelected == true && CryptographyProvider.EXTENSIONS_DECRYPTED.Contains(Path.GetExtension(p.FileName), StringComparer.OrdinalIgnoreCase)).ToList();
                    }

                    // Set the number of threads.
                    int numberOfThreads = 0;

                    if (settings.UseMultiThreading)
                    {
                        numberOfThreads = settings.NumberOfThreads;
                    }
                    else
                    {
                        numberOfThreads = 1;
                    }

                    // Distribute tasks to threads.
                    var tasks = new List<Task>();
                    var throttler = new SemaphoreSlim(settings.NumberOfThreads, settings.NumberOfThreads);

                    foreach (IndexedItem item in selectedItems)
                    {
                        // Do an async wait until we can schedule again.
                        await throttler.WaitAsync();

                        tasks.Add(EncryptInternalAsync(item, viewModel.EncryptionKey, viewModel.OutputDirectory, viewModel.VerifyFakeHeader, viewModel.EncryptWithRMMZExtensions, throttler));
                    }

                    _stopwatch.Stop(); // Stop
                    IsWorking = false;

                    Log.Information($"The operation completed successfully({_stopwatch.ElapsedMilliseconds}ms).");

                    // --------------------------------------------------------------------------------

                    // Show a result dialog.
                    using (TaskDialog taskDialog = new TaskDialog())
                    {
                        taskDialog.WindowTitle = "MView";
                        taskDialog.MainInstruction = "The operation completed successfully.";
                        taskDialog.Content = $"ELAPSED TIME : {_stopwatch.ElapsedMilliseconds}ms";
                        taskDialog.Footer = "If an unknown error occurs, please report it to the developer.";
                        taskDialog.FooterIcon = TaskDialogIcon.Information;

                        TaskDialogButton openFolderButton = new TaskDialogButton(ButtonType.Custom) { Text = "Open the folder" };
                        TaskDialogButton viewLogsButton = new TaskDialogButton(ButtonType.Custom) { Text = "View logs" };
                        TaskDialogButton okButton = new TaskDialogButton(ButtonType.Custom) { Text = "OK" };
                        taskDialog.Buttons.Add(openFolderButton);
                        taskDialog.Buttons.Add(viewLogsButton);
                        taskDialog.Buttons.Add(okButton);

                        TaskDialogButton button = taskDialog.ShowDialog();

                        if (button == openFolderButton)
                        {
                            Process.Start("explorer.exe", viewModel.OutputDirectory);
                        }
                        else if (button == viewLogsButton)
                        {
                            Process.Start("explorer.exe", Path.Combine(Environment.CurrentDirectory, @"data\logs"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IsWorking = false;
                Log.Warning(ex, "An unexpected exception has occured.");
                MessageBox.Show("An unexpected exception has occured.", "MView", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task EncryptInternalAsync(IndexedItem item, string key, string outputDirectory, bool verifyFakeHeader, bool encryptWithRMMZExtensions, SemaphoreSlim? throttler)
        {
            try
            {
                // Check the file.
                if (!File.Exists(item.FullPath))
                {
                    Log.Warning($"The file does not exist(FILE : {item.FullPath}).");
                    return;
                }

                // Verify the fake header of the file.
                if (verifyFakeHeader)
                {
                    bool verifyingResult = await CryptographyProvider.VerifyFakeHeaderAsync(item.FullPath);

                    if (verifyingResult)
                    {
                        Log.Warning($"This file is already encrypted(FILE : {item.FullPath}).");
                        return;
                    }
                }

                // Build variables.
                string relativePath = Path.GetRelativePath(item.RootDirectory, item.FullPath);
                string outputPath = Path.Combine(outputDirectory, relativePath);

                string extension = Path.GetExtension(outputPath).ToLower();

                if (CryptographyProvider.EXTENSIONS_DECRYPTED.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    string targetExtension = string.Empty;

                    if (!encryptWithRMMZExtensions)
                    {
                        if (extension == ".png")
                        {
                            targetExtension = ".rpgmvp";
                        }
                        else if (extension == ".ogg")
                        {
                            targetExtension = ".rpgmvo";
                        }
                        else if (extension == ".m4a")
                        {
                            targetExtension = ".rpgmvm";
                        }
                        else if (extension == ".wav")
                        {
                            targetExtension = ".rpgmvw";
                        }
                    }
                    else
                    {
                        if (extension == ".png")
                        {
                            targetExtension = ".png_";
                        }
                        else if (extension == ".ogg")
                        {
                            targetExtension = ".ogg_";
                        }
                        else if (extension == ".m4a")
                        {
                            targetExtension = ".m4a_";
                        }
                        else if (extension == ".wav")
                        {
                            targetExtension = ".wav_";
                        }
                    }

                    outputPath = Path.Combine(Path.GetDirectoryName(outputPath), Path.GetFileNameWithoutExtension(outputPath) + targetExtension);
                }

                // Start a task.
                if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                }

                await CryptographyProvider.EncryptAsync(item.FullPath, outputPath, key);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"A fatal error occurred during the restore operation(FILE : {item.FullPath}).");
            }
            finally
            {
                throttler?.Release();
            }
        }

        public async void Decrypt()
        {
            try
            {
                DecrypterViewModel viewModel = IoC.Get<DecrypterViewModel>();
                bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

                if (dialogResult == true)
                {
                    Settings settings = IoC.Get<Settings>();

                    // Check the number of threads.
                    if (settings.UseMultiThreading && (settings.NumberOfThreads < 0 || settings.NumberOfThreads > 10))
                    {
                        Log.Fatal("The number of threads out of range.");
                        return;
                    }

                    // Process
                    IsWorking = true;
                    _stopwatch.Restart(); // Start

                    // Get selected items.
                    List<IndexedItem> selectedItems;

                    if (viewModel.DecryptAllFileFormats)
                    {
                        selectedItems = IndexedItems.Where(p => p.IsSelected == true).ToList();
                    }
                    else
                    {
                        selectedItems = IndexedItems.Where(p => p.IsSelected == true && CryptographyProvider.EXTENSIONS_ENCRYPTED.Contains(Path.GetExtension(p.FileName), StringComparer.OrdinalIgnoreCase)).ToList();
                    }

                    // Set the number of threads.
                    int numberOfThreads = 0;

                    if (settings.UseMultiThreading)
                    {
                        numberOfThreads = settings.NumberOfThreads;
                    }
                    else
                    {
                        numberOfThreads = 1;
                    }

                    // Distribute tasks to threads.
                    var tasks = new List<Task>();
                    var throttler = new SemaphoreSlim(settings.NumberOfThreads, settings.NumberOfThreads);

                    foreach (IndexedItem item in selectedItems)
                    {
                        // Do an async wait until we can schedule again.
                        await throttler.WaitAsync();

                        tasks.Add(DecryptInternalAsync(item, viewModel.EncryptionKey, viewModel.OutputDirectory, viewModel.VerifyFakeHeader, throttler));
                    }

                    await Task.WhenAll(tasks);

                    _stopwatch.Stop(); // Stop
                    IsWorking = false;

                    Log.Information($"The operation completed successfully({_stopwatch.ElapsedMilliseconds}ms).");

                    // --------------------------------------------------------------------------------

                    // Show a result dialog.
                    using (TaskDialog taskDialog = new TaskDialog())
                    {
                        taskDialog.WindowTitle = "MView";
                        taskDialog.MainInstruction = "The operation completed successfully.";
                        taskDialog.Content = $"ELAPSED TIME : {_stopwatch.ElapsedMilliseconds}ms";
                        taskDialog.Footer = "If an unknown error occurs, please report it to the developer.";
                        taskDialog.FooterIcon = TaskDialogIcon.Information;

                        TaskDialogButton openFolderButton = new TaskDialogButton(ButtonType.Custom) { Text = "Open the folder" };
                        TaskDialogButton viewLogsButton = new TaskDialogButton(ButtonType.Custom) { Text = "View logs" };
                        TaskDialogButton okButton = new TaskDialogButton(ButtonType.Custom) { Text = "OK" };
                        taskDialog.Buttons.Add(openFolderButton);
                        taskDialog.Buttons.Add(viewLogsButton);
                        taskDialog.Buttons.Add(okButton);

                        TaskDialogButton button = taskDialog.ShowDialog();

                        if (button == openFolderButton)
                        {
                            Process.Start("explorer.exe", viewModel.OutputDirectory);
                        }
                        else if (button == viewLogsButton)
                        {
                            Process.Start("explorer.exe", Path.Combine(Environment.CurrentDirectory, @"data\logs"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IsWorking = false;
                Log.Warning(ex, "An unexpected exception has occured.");
                MessageBox.Show("An unexpected exception has occured.", "MView", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DecryptInternalAsync(IndexedItem item, string key, string outputDirectory, bool verifyFakeHeader, SemaphoreSlim? throttler)
        {
            try
            {
                // Check the file.
                if (!File.Exists(item.FullPath))
                {
                    Log.Warning($"The file does not exist(FILE : {item.FullPath}).");
                    return;
                }

                // Verify the fake header of the file.
                if (verifyFakeHeader)
                {
                    bool verifyingResult = await CryptographyProvider.VerifyFakeHeaderAsync(item.FullPath);

                    if (!verifyingResult)
                    {
                        Log.Warning($"The file is not a valid RPG Maker MV/MZ resource file(FILE : {item.FullPath}).");
                        return;
                    }
                }

                // Build variables.
                string relativePath = Path.GetRelativePath(item.RootDirectory, item.FullPath);
                string outputPath = Path.Combine(outputDirectory, relativePath);

                string extension = Path.GetExtension(outputPath).ToLower();
                int extIndex = CryptographyProvider.EXTENSIONS_ENCRYPTED.ToList().IndexOf(extension);

                if (extIndex != -1)
                {
                    string targetExtension = CryptographyProvider.EXTENSIONS_DECRYPTED[extIndex];
                    outputPath = Path.Combine(Path.GetDirectoryName(outputPath), Path.GetFileNameWithoutExtension(outputPath) + targetExtension);
                }

                // Start a task.
                if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                }

                await CryptographyProvider.DecryptAsync(item.FullPath, outputPath, key);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"A fatal error occurred during the restore operation(FILE : {item.FullPath}).");
            }
            finally
            {
                throttler?.Release();
            }
        }

        public async void Estimate()
        {
            try
            {
                if (_selectedItem != null)
                {
                    // Check the file.
                    if (!File.Exists(_selectedItem.FullPath))
                    {
                        Log.Error($"The file does not exist(FILE : {_selectedItem.FullPath}).");
                        MessageBox.Show($"The file does not exist.", "MView", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    IsWorking = true;
                    _stopwatch.Restart(); // Start

                    string[] targetExtensions = new string[] { ".rpgmvp", ".rpgmvm", ".rpgmvw", ".png_", ".m4a_", ".wav_" };

                    string extension = Path.GetExtension(_selectedItem.FileName);

                    if (targetExtensions.Any(p => p.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                    {
                        string key = await CryptographyProvider.EstimateAsync(_selectedItem.FullPath);

                        _stopwatch.Stop(); // Stop
                        IsWorking = false;

                        Log.Information($"The operation completed successfully({_stopwatch.ElapsedMilliseconds}ms).");

                        // Show a result dialog.
                        using (TaskDialog taskDialog = new TaskDialog())
                        {
                            taskDialog.WindowTitle = "MView";
                            taskDialog.MainInstruction = "The operation completed successfully.";
                            taskDialog.Content = $"ELAPSED TIME : {_stopwatch.ElapsedMilliseconds}ms\r\nKEY : {key}";
                            taskDialog.Footer = "The estimated encryption key may not match the actual one.";
                            taskDialog.FooterIcon = TaskDialogIcon.Warning;

                            TaskDialogButton copyButton = new TaskDialogButton(ButtonType.Custom) { Text = "Copy Result" };
                            TaskDialogButton okButton = new TaskDialogButton(ButtonType.Custom) { Text = "OK" };
                            taskDialog.Buttons.Add(copyButton);
                            taskDialog.Buttons.Add(okButton);

                            TaskDialogButton button = taskDialog.ShowDialog();

                            // Copy the key to the clipboard.
                            if (button == copyButton)
                            {
                                string result = $"{key}";
                                Clipboard.SetText(result);
                            }
                        }
                    }
                    else
                    {
                        IsWorking = false;
                        Log.Warning($"\'{_selectedItem.FileName}\' is an unsupported format.");
                        MessageBox.Show($"\'{_selectedItem.FileName}\' is an unsupported format.\r\n(Supported formats: *.rpgmvp, *.rpgmvm, *.rpgmvw, *.png_, *.m4a_, *.wav_)", "MView", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                IsWorking = false;
                Log.Warning(ex, "An unexpected exception has occured.");
                MessageBox.Show("An unexpected exception has occured.", "MView", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void Restore()
        {
            try
            {
                RestorerViewModel viewModel = IoC.Get<RestorerViewModel>();
                bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

                if (dialogResult == true)
                {
                    Settings settings = IoC.Get<Settings>();

                    // Check the number of threads.
                    if (settings.NumberOfThreads < 0 || settings.NumberOfThreads > 10)
                    {
                        Log.Fatal("The number of threads out of range.");
                        return;
                    }

                    // Process
                    IsWorking = true;
                    _stopwatch.Restart(); // Start

                    // Get selected items.
                    List<IndexedItem> selectedItems = IndexedItems.Where(p => p.IsSelected == true && CryptographyProvider.EXTENSIONS_ENCRYPTED.Contains(Path.GetExtension(p.FileName), StringComparer.OrdinalIgnoreCase)).ToList();

                    // Set the number of threads.
                    int numberOfThreads = 0;

                    if (settings.UseMultiThreading)
                    {
                        numberOfThreads = settings.NumberOfThreads;
                    }
                    else
                    {
                        numberOfThreads = 1;
                    }

                    // Distribute tasks to threads.
                    var tasks = new List<Task>();
                    var throttler = new SemaphoreSlim(settings.NumberOfThreads, settings.NumberOfThreads);

                    foreach (IndexedItem item in selectedItems)
                    {
                        // Do an async wait until we can schedule again.
                        await throttler.WaitAsync();

                        tasks.Add(RestoreInternalAsync(item, viewModel.OutputDirectory, viewModel.VerifyFakeHeader, throttler));
                    }

                    _stopwatch.Stop(); // Stop
                    IsWorking = false;

                    Log.Information($"The operation completed successfully({_stopwatch.ElapsedMilliseconds}ms).");

                    // --------------------------------------------------------------------------------

                    // Show a result dialog.
                    using (TaskDialog taskDialog = new TaskDialog())
                    {
                        taskDialog.WindowTitle = "MView";
                        taskDialog.MainInstruction = "The operation completed successfully.";
                        taskDialog.Content = $"ELAPSED TIME : {_stopwatch.ElapsedMilliseconds}ms";
                        taskDialog.Footer = "If an unknown error occurs, please report it to the developer.";
                        taskDialog.FooterIcon = TaskDialogIcon.Information;

                        TaskDialogButton openFolderButton = new TaskDialogButton(ButtonType.Custom) { Text = "Open the folder" };
                        TaskDialogButton viewLogsButton = new TaskDialogButton(ButtonType.Custom) { Text = "View logs" };
                        TaskDialogButton okButton = new TaskDialogButton(ButtonType.Custom) { Text = "OK" };
                        taskDialog.Buttons.Add(openFolderButton);
                        taskDialog.Buttons.Add(viewLogsButton);
                        taskDialog.Buttons.Add(okButton);

                        TaskDialogButton button = taskDialog.ShowDialog();

                        if (button == openFolderButton)
                        {
                            Process.Start("explorer.exe", viewModel.OutputDirectory);
                        }
                        else if (button == viewLogsButton)
                        {
                            Process.Start("explorer.exe", Path.Combine(Environment.CurrentDirectory, @"data\logs"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IsWorking = false;
                Log.Warning(ex, "An unexpected exception has occured.");
                MessageBox.Show("An unexpected exception has occured.", "MView", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task RestoreInternalAsync(IndexedItem item, string outputDirectory, bool verifyFakeHeader, SemaphoreSlim? throttler)
        {
            try
            {
                // Check the file.
                if (!File.Exists(item.FullPath))
                {
                    Log.Warning($"The file does not exist(FILE : {item.FullPath}).");
                    return;
                }

                // Verify the fake header of the file.
                if (verifyFakeHeader)
                {
                    bool verifyingResult = await CryptographyProvider.VerifyFakeHeaderAsync(item.FullPath);

                    if (!verifyingResult)
                    {
                        Log.Warning($"The file is not a valid RPG Maker MV/MZ resource file(FILE : {item.FullPath}).");
                        return;
                    }
                }

                // Build variables.
                string relativePath = Path.GetRelativePath(item.RootDirectory, item.FullPath);
                string outputPath = Path.Combine(outputDirectory, relativePath);

                string extension = Path.GetExtension(outputPath).ToLower();
                int extIndex = CryptographyProvider.EXTENSIONS_ENCRYPTED.ToList().IndexOf(extension);

                if (extIndex != -1)
                {
                    string targetExtension = CryptographyProvider.EXTENSIONS_DECRYPTED[extIndex];
                    outputPath = Path.Combine(Path.GetDirectoryName(outputPath), Path.GetFileNameWithoutExtension(outputPath) + targetExtension);
                }

                // Start a task.
                if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                }

                await CryptographyProvider.RestoreAsync(item.FullPath, outputPath);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"A fatal error occurred during the restore operation(FILE : {item.FullPath}).");
            }
            finally
            {
                throttler?.Release();
            }
        }

        // App

        public async void PickNoT()
        {
            try
            {
                var settings = IoC.Get<Settings>();
                int initialValue = settings.NumberOfThreads;

                NoTPickerViewModel viewModel = IoC.Get<NoTPickerViewModel>();
                viewModel.NumberOfThreads = initialValue;

                bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

                if (dialogResult == true)
                {
                    if (viewModel.NumberOfThreads <= 0 || viewModel.NumberOfThreads > 10)
                    {
                        settings.NumberOfThreads = initialValue;
                        MessageBox.Show("The number of threads out of range.", "MView", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    settings.NumberOfThreads = viewModel.NumberOfThreads;
                    MessageBox.Show($"The number of threads is set to {viewModel.NumberOfThreads}.", "MView", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An unexpected exception has occured.");
            }
        }

        public async void OpenInformation()
        {
            InformationViewModel viewModel = IoC.Get<InformationViewModel>();
            await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);
        }

        #endregion

        #region ::IDisposable Members::

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    IndexedItems.Clear();

                    if (_sourceStream != null && _sourceStream != Stream.Null)
                    {
                        _sourceStream.Dispose();
                        SourceStream = Stream.Null;
                    }

                    if (_imageStream != null && _imageStream != Stream.Null)
                    {
                        _imageStream.Dispose();
                        ImageStream = Stream.Null;
                    }

                    if (_wavePlayer != null)
                    {
                        _wavePlayer.Dispose();
                        _wavePlayer = null;
                    }

                    if (_waveStream != null)
                    {
                        _waveStream.Dispose();
                        _waveStream = null;
                    }

                    _refreshTimer.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}