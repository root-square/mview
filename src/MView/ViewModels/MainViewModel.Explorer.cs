using Caliburn.Micro;
using Microsoft.Win32;
using MView.Utilities;
using MView.Utilities.Indexing;
using Ookii.Dialogs.Wpf;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using ProgressBarStyle = Ookii.Dialogs.Wpf.ProgressBarStyle;
using TaskDialog = Ookii.Dialogs.Wpf.TaskDialog;
using TaskDialogButton = Ookii.Dialogs.Wpf.TaskDialogButton;
using TaskDialogIcon = Ookii.Dialogs.Wpf.TaskDialogIcon;

namespace MView.ViewModels
{
    public partial class MainViewModel
    {
        private BindableCollection<IndexedItem> _indexedItems = new BindableCollection<IndexedItem>();

        public BindableCollection<IndexedItem> IndexedItems
        {
            get => _indexedItems;
            set
            {
                Set(ref _indexedItems, value);

                NotifyOfPropertyChange("IsEmpty");
            }
        }

        public bool IsEmpty
        {
            get => _indexedItems.Count == 0 ? true : false;
        }

        private BindableCollection<IndexedItem> _selectedItems = new BindableCollection<IndexedItem>();

        public BindableCollection<IndexedItem> SelectedItems
        {
            get => _selectedItems;
            set => Set(ref _selectedItems, value);
        }

        private IndexedItem? _selectedItem = null;

        public IndexedItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value != null)
                {
                    Set(ref _selectedItem, value);
                    RefreshViewerAsync().ConfigureAwait(false);
                }
            }
        }

        public async Task OpenAsync(string[] pathsToIndex, bool indexAllFiles = false)
        {
            Exception? exception = null;

            var task = Task.Run(() =>
            {
                List<IndexedItem> itemList = new List<IndexedItem>();

                foreach (string path in pathsToIndex)
                {
                    if (IndexingManager.IsDirectory(path))
                    {
                        if (!Directory.Exists(path))
                        {
                            IndexedItems.Clear();
                            exception = new DirectoryNotFoundException("The directory does not found.");
                            return;
                        }

                        if (Path.GetPathRoot(path) == Path.GetFullPath(path))
                        {
                            IndexedItems.Clear();
                            exception = new InvalidOperationException("The root directory cannot be loaded.");
                            return;
                        }

                        string rootDirectory = pathsToIndex.Length == 1 ? path : Path.GetDirectoryName(path)!;

                        itemList.AddRange(IndexingManager.GetFiles(new DirectoryInfo(path), rootDirectory, indexAllFiles ? null : CryptographyProvider.EXTENSIONS.ToList()));
                    }
                    else
                    {
                        if (!File.Exists(path))
                        {
                            IndexedItems.Clear();
                            exception = new FileNotFoundException("The file does not found.");
                            return;
                        }

                        IndexedItem? item = IndexingManager.GetFile(new FileInfo(path), Path.GetDirectoryName(path)!, indexAllFiles ? null : CryptographyProvider.EXTENSIONS.ToList());

                        if (item != null)
                        {
                            itemList.Add(item);
                        }
                    }
                }

                itemList = itemList.Distinct().ToList();

                IndexedItems.Clear();
                IndexedItems.AddRange(itemList);

                NotifyOfPropertyChange("IsEmpty");
            });

            await task.ConfigureAwait(false);

            if (exception != null)
            {
                throw exception;
            }
        }

        public void OpenFiles()
        {
            VistaOpenFileDialog openFileDialog = new VistaOpenFileDialog();
            openFileDialog.Title = LocalizationHelper.GetText("EXPLORER_OPEN_FILES_DIALOG_TITLE");
            openFileDialog.Filter = LocalizationHelper.GetText("EXPLORER_OPEN_FILES_DIALOG_FILTER");
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            if (openFileDialog.FileNames.Length < 1)
            {
                return;
            }

            bool indexAllFiles = false;

            using (TaskDialog taskDialog = new TaskDialog())
            {
                taskDialog.WindowTitle = "MView";
                taskDialog.MainInstruction = LocalizationHelper.GetText("EXPLORER_LOAD_DIALOG_TITLE");
                taskDialog.Content = LocalizationHelper.GetText("EXPLORER_LOAD_DIALOG_DESC");
                taskDialog.Footer = LocalizationHelper.GetText("EXPLORER_LOAD_DIALOG_FOOTER");
                taskDialog.FooterIcon = TaskDialogIcon.Warning;

                TaskDialogButton yesButton = new TaskDialogButton(ButtonType.Custom) { Text = LocalizationHelper.GetText("COMMON_YES") };
                TaskDialogButton noButton = new TaskDialogButton(ButtonType.Custom) { Text = LocalizationHelper.GetText("COMMON_NO") };
                taskDialog.Buttons.Add(yesButton);
                taskDialog.Buttons.Add(noButton);

                TaskDialogButton button = taskDialog.ShowDialog();

                if (button == yesButton)
                {
                    indexAllFiles = true;
                }
                else if (button == noButton)
                {
                    indexAllFiles = false;
                }
                else
                {
                    return;
                }
            }

            CancellationTokenSource cancellationtokenSource = new CancellationTokenSource();

            using (ProgressDialog progressDialog = new ProgressDialog())
            {
                progressDialog.WindowTitle = "MView";
                progressDialog.Text = LocalizationHelper.GetText("EXPLORER_TASK_DIALOG_TITLE");
                progressDialog.Description = LocalizationHelper.GetText("EXPLORER_TASK_DIALOG_DESC");
                progressDialog.ShowTimeRemaining = true;
                progressDialog.ShowCancelButton = false;
                progressDialog.ProgressBarStyle = ProgressBarStyle.ProgressBar;

                progressDialog.DoWork += async (sender, e) =>
                {
                    try
                    {
                        await OpenAsync(openFileDialog.FileNames, indexAllFiles);
                    }
                    catch (FileNotFoundException)
                    {
                        cancellationtokenSource.Cancel();

                        MessageBox.Show(LocalizationHelper.GetText("EXPLORER_ALERT_FILE_NOT_FOUND"), "MView", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                };

                progressDialog.Show(cancellationtokenSource.Token);
            }
        }

        public void OpenFolders()
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            folderBrowserDialog.Description = LocalizationHelper.GetText("EXPLORER_OPEN_FOLDERS_DIALOG_TITLE");
            folderBrowserDialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
            folderBrowserDialog.Multiselect = true;

            if (folderBrowserDialog.ShowDialog() != true)
            {
                return;
            }

            if (folderBrowserDialog.SelectedPaths.Length < 1)
            {
                return;
            }

            bool indexAllFiles = false;

            using (TaskDialog taskDialog = new TaskDialog())
            {
                taskDialog.WindowTitle = "MView";
                taskDialog.MainInstruction = LocalizationHelper.GetText("EXPLORER_LOAD_DIALOG_TITLE");
                taskDialog.Content = LocalizationHelper.GetText("EXPLORER_LOAD_DIALOG_DESC");
                taskDialog.Footer = LocalizationHelper.GetText("EXPLORER_LOAD_DIALOG_FOOTER");
                taskDialog.FooterIcon = TaskDialogIcon.Warning;

                TaskDialogButton yesButton = new TaskDialogButton(ButtonType.Custom) { Text = LocalizationHelper.GetText("COMMON_YES") };
                TaskDialogButton noButton = new TaskDialogButton(ButtonType.Custom) { Text = LocalizationHelper.GetText("COMMON_NO") };
                taskDialog.Buttons.Add(yesButton);
                taskDialog.Buttons.Add(noButton);

                TaskDialogButton button = taskDialog.ShowDialog();

                if (button == yesButton)
                {
                    indexAllFiles = true;
                }
                else if (button == noButton)
                {
                    indexAllFiles = false;
                }
                else
                {
                    return;
                }
            }

            CancellationTokenSource cancellationtokenSource = new CancellationTokenSource();

            using (ProgressDialog progressDialog = new ProgressDialog())
            {
                progressDialog.WindowTitle = "MView";
                progressDialog.Text = LocalizationHelper.GetText("EXPLORER_TASK_DIALOG_TITLE");
                progressDialog.Description = LocalizationHelper.GetText("EXPLORER_TASK_DIALOG_DESC");
                progressDialog.ShowTimeRemaining = true;
                progressDialog.ShowCancelButton = false;
                progressDialog.ProgressBarStyle = ProgressBarStyle.ProgressBar;

                progressDialog.DoWork += async (sender, e) =>
                {
                    try
                    {
                        await OpenAsync(folderBrowserDialog.SelectedPaths, indexAllFiles);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        cancellationtokenSource.Cancel();

                        MessageBox.Show(LocalizationHelper.GetText("EXPLORER_ALERT_DIR_NOT_FOUND"), "MView", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    catch (InvalidOperationException)
                    {
                        cancellationtokenSource.Cancel();

                        MessageBox.Show(LocalizationHelper.GetText("EXPLORER_ALERT_ROOT_DIR_SELECTED"), "MView", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                };

                progressDialog.Show(cancellationtokenSource.Token);
            }
        }

        public void OnFilesDropped(string[] files)
        {
            bool indexAllFiles = false;

            using (TaskDialog taskDialog = new TaskDialog())
            {
                taskDialog.WindowTitle = "MView";
                taskDialog.MainInstruction = LocalizationHelper.GetText("EXPLORER_LOAD_DIALOG_TITLE");
                taskDialog.Content = LocalizationHelper.GetText("EXPLORER_LOAD_DIALOG_DESC");
                taskDialog.Footer = LocalizationHelper.GetText("EXPLORER_LOAD_DIALOG_FOOTER");
                taskDialog.FooterIcon = TaskDialogIcon.Warning;

                TaskDialogButton yesButton = new TaskDialogButton(ButtonType.Custom) { Text = LocalizationHelper.GetText("COMMON_YES") };
                TaskDialogButton noButton = new TaskDialogButton(ButtonType.Custom) { Text = LocalizationHelper.GetText("COMMON_NO") };
                taskDialog.Buttons.Add(yesButton);
                taskDialog.Buttons.Add(noButton);

                TaskDialogButton button = taskDialog.ShowDialog();

                if (button == yesButton)
                {
                    indexAllFiles = true;
                }
                else if (button == noButton)
                {
                    indexAllFiles = false;
                }
                else
                {
                    return;
                }
            }

            CancellationTokenSource cancellationtokenSource = new CancellationTokenSource();

            using (ProgressDialog progressDialog = new ProgressDialog())
            {
                progressDialog.WindowTitle = "MView";
                progressDialog.Text = LocalizationHelper.GetText("EXPLORER_TASK_DIALOG_TITLE");
                progressDialog.Description = LocalizationHelper.GetText("EXPLORER_TASK_DIALOG_DESC");
                progressDialog.ShowTimeRemaining = true;
                progressDialog.ShowCancelButton = false;
                progressDialog.ProgressBarStyle = ProgressBarStyle.ProgressBar;

                progressDialog.DoWork += async (sender, e) =>
                {
                    try
                    {
                        await OpenAsync(files, indexAllFiles);
                    }
                    catch (FileNotFoundException)
                    {
                        cancellationtokenSource.Cancel();

                        MessageBox.Show(LocalizationHelper.GetText("EXPLORER_ALERT_FILE_NOT_FOUND"), "MView", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        cancellationtokenSource.Cancel();

                        MessageBox.Show(LocalizationHelper.GetText("EXPLORER_ALERT_DIR_NOT_FOUND"), "MView", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    catch (InvalidOperationException)
                    {
                        cancellationtokenSource.Cancel();

                        MessageBox.Show(LocalizationHelper.GetText("EXPLORER_ALERT_ROOT_DIR_SELECTED"), "MView", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                };

                progressDialog.Show(cancellationtokenSource.Token);
            }
        }

        public async void SelectAsync()
        {
            var task = Task.Run(() =>
            {
                foreach (var item in SelectedItems)
                {
                    item.IsSelected = true;
                }

                NotifyOfPropertyChange("IndexedItems");
            });

            await task.ConfigureAwait(false);
        }

        public async void SelectAllAsync()
        {
            var task = Task.Run(() =>
            {
                foreach (var item in IndexedItems)
                {
                    item.IsSelected = true;
                }

                NotifyOfPropertyChange("IndexedItems");
            });

            await task.ConfigureAwait(false);
        }

        public async void DeselectAsync()
        {
            var task = Task.Run(() =>
            {
                foreach (var item in SelectedItems)
                {
                    item.IsSelected = false;
                }

                NotifyOfPropertyChange("IndexedItems");
            });

            await task.ConfigureAwait(false);
        }

        public async void DeselectAllAsync()
        {
            var task = Task.Run(() =>
            {
                foreach (var item in IndexedItems)
                {
                    item.IsSelected = false;
                }

                NotifyOfPropertyChange("IndexedItems");
            });

            await task.ConfigureAwait(false);
        }

        public async void ReverseSelectionAsync()
        {
            var task = Task.Run(() =>
            {
                foreach (var item in IndexedItems)
                {
                    item.IsSelected = !item.IsSelected;
                }

                NotifyOfPropertyChange("IndexedItems");
            });

            await task.ConfigureAwait(false);
        }

        public async void DeleteAsync()
        {
            var task = Task.Run(async () =>
            {
                List<IndexedItem> targetItems = SelectedItems.ToList();

                bool isRefreshNeeded = false;

                if (targetItems.Contains(SelectedItem!))
                {
                    isRefreshNeeded = true;
                }

                foreach (var item in targetItems)
                {
                    SelectedItem = null;
                    SelectedItems.Remove(item);
                    IndexedItems.Remove(item);
                }

                NotifyOfPropertyChange("IndexedItems");
                NotifyOfPropertyChange("IsEmpty");

                if (isRefreshNeeded)
                {
                    await RefreshViewerAsync().ConfigureAwait(false);
                }
            });

            await task.ConfigureAwait(false);
        }

        public async void DeleteAllAsync()
        {
            var task = Task.Run(async () =>
            {
                SelectedItem = null;
                SelectedItems.Clear();
                IndexedItems.Clear();

                NotifyOfPropertyChange("IndexedItems");
                NotifyOfPropertyChange("IsEmpty");

                await RefreshViewerAsync().ConfigureAwait(false);
            });

            await task.ConfigureAwait(false);
        }
    }
}
