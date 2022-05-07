using Caliburn.Micro;
using MView.Utilities.Indexing;
using MView.Utilities.Text;
using Ookii.Dialogs.Wpf;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MView.ViewModels
{
    public partial class MainViewModel
    {
        #region ::Variables::

        public BindableCollection<IndexedItem> IndexedItems { get; set; } = new BindableCollection<IndexedItem>();

        public CollectionViewSource ItemCollectionViewSource { get; set; } = new CollectionViewSource();

        private IndexedItem _selectedItem = new IndexedItem();

        public IndexedItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value != null)
                {
                    Set(ref _selectedItem, value);
                    RefreshViewerAsync().ConfigureAwait(false).GetAwaiter();
                    RefreshMetadataAsync().ConfigureAwait(false).GetAwaiter();
                }
            }
        }

        private BindableCollection<IndexedItem> _selectedItems = new BindableCollection<IndexedItem>();

        public BindableCollection<IndexedItem> SelectedItems
        {
            get => _selectedItems;
            set => Set(ref _selectedItems, value);
        }

        #endregion

        #region ::Workers::

        // File
        public void OpenFiles()
        {
            VistaOpenFileDialog openFileDialog = new VistaOpenFileDialog();
            openFileDialog.Title = "Please select files.";
            openFileDialog.Filter = "RPG MV resources|*.rpgmvp;*.rpgmvo;*.rpgmvm;*.rpgmvw|RPG MZ resources|*.png_;*.ogg_;*.m4a_;*.wav_|Resources|*.png;*.ogg;*.m4a;*.wav|All files|*.*";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                // Check whether to index all file extensions.
                bool indexAllExtensions = false;

                using (TaskDialog taskDialog = new TaskDialog())
                {
                    taskDialog.WindowTitle = "MView";
                    taskDialog.MainInstruction = "Do you want to index all file extensions?";
                    taskDialog.Content = "If you click 'OK', MView indexes all file extensions, not just RPG MV/MZ resources file extensions.";
                    taskDialog.Footer = "Some errors can occur if files that are not RPG MV/MZ resources are processed in MView.";
                    taskDialog.FooterIcon = TaskDialogIcon.Warning;

                    TaskDialogButton yesButton = new TaskDialogButton(ButtonType.Custom) { Text = "Yes" };
                    TaskDialogButton noButton = new TaskDialogButton(ButtonType.Custom) { Text = "No" };
                    taskDialog.Buttons.Add(yesButton);
                    taskDialog.Buttons.Add(noButton);

                    TaskDialogButton button = taskDialog.ShowDialog();

                    if (button == yesButton)
                    {
                        indexAllExtensions = true;
                    }
                    else if (button == noButton)
                    {
                        indexAllExtensions = false;
                    }
                    else
                    {
                        return;
                    }
                }

                // Clear the collection.
                IndexedItems.Clear();

                // Get selected files.
                string[] selectedFiles = openFileDialog.FileNames;

                if (selectedFiles.Length < 1)
                {
                    Log.Warning("Plesae select some files.");
                    return;
                }

                // Process
                using (ProgressDialog progressDialog = new ProgressDialog())
                {
                    progressDialog.WindowTitle = "MView";
                    progressDialog.Text = "MView File Indexer";
                    progressDialog.Description = "Indexing selected files...";
                    progressDialog.ShowTimeRemaining = true;
                    progressDialog.ShowCancelButton = false;
                    progressDialog.ProgressBarStyle = ProgressBarStyle.MarqueeProgressBar;

                    progressDialog.DoWork += (sender, e) => {
                        List<string> extensions = Settings.KnownExtensions.ToList();

                        for (int i = 0; i < selectedFiles.Length; i++)
                        {
                            IndexedItem? item = IndexingManager.GetFile(new FileInfo(selectedFiles[i]), Path.GetDirectoryName(selectedFiles[i]), indexAllExtensions ? null : extensions);

                            if (item != null)
                            {
                                IndexedItems.Add(item);
                            }
                        }

                        Log.Information($"{IndexedItems.Count} items have been indexed.");
                    };

                    progressDialog.Show();
                }
            }
        }

        public void OpenFolders()
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            folderBrowserDialog.Description = "Please select folders.";
            folderBrowserDialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
            folderBrowserDialog.Multiselect = true;

            if (folderBrowserDialog.ShowDialog() == true)
            {
                // Check whether to index all file extensions.
                bool indexAllExtensions = false;

                using (TaskDialog taskDialog = new TaskDialog())
                {
                    taskDialog.WindowTitle = "MView";
                    taskDialog.MainInstruction = "Do you want to index all file extensions?";
                    taskDialog.Content = "If you click 'OK', MView indexes all file extensions, not just RPG MV/MZ resources file extensions.";
                    taskDialog.Footer = "Some errors can occur if files that are not RPG MV/MZ resources are processed in MView.";
                    taskDialog.FooterIcon = TaskDialogIcon.Warning;

                    TaskDialogButton yesButton = new TaskDialogButton(ButtonType.Custom) { Text= "Yes" };
                    TaskDialogButton noButton = new TaskDialogButton(ButtonType.Custom) { Text = "No" };
                    taskDialog.Buttons.Add(yesButton);
                    taskDialog.Buttons.Add(noButton);

                    TaskDialogButton button = taskDialog.ShowDialog();

                    if (button == yesButton)
                    {
                        indexAllExtensions = true;
                    }
                    else if (button == noButton)
                    {
                        indexAllExtensions = false;
                    }
                    else
                    {
                        return;
                    }
                }

                // Clear the collection.
                IndexedItems.Clear();

                // Get selected paths.
                string[] selectedPaths = folderBrowserDialog.SelectedPaths;

                if (selectedPaths.Length < 1)
                {
                    Log.Warning("Plesae select some folders.");
                    return;
                }

                // Process
                using (ProgressDialog progressDialog = new ProgressDialog())
                {
                    progressDialog.WindowTitle = "MView";
                    progressDialog.Text = "MView File Indexer";
                    progressDialog.Description = "Indexing selected folders...";
                    progressDialog.ShowTimeRemaining = true;
                    progressDialog.ShowCancelButton = false;
                    progressDialog.ProgressBarStyle = ProgressBarStyle.MarqueeProgressBar;

                    progressDialog.DoWork += (sender, e) => {
                        List<string> extensions = Settings.KnownExtensions.ToList();

                        for (int i = 0; i < selectedPaths.Length; i++)
                        {
                            if (Path.GetDirectoryName(selectedPaths[i]) == Path.GetPathRoot(selectedPaths[i]))
                            {
                                Log.Warning($"The root directory has been selected. Skip the directory.");
                                continue;
                            }

                            string? rootDirectory = selectedPaths.Length == 1 ? selectedPaths[i] : Path.GetDirectoryName(selectedPaths[i]);

                            List<IndexedItem> items = IndexingManager.GetFiles(new DirectoryInfo(selectedPaths[i]), rootDirectory, indexAllExtensions ? null : extensions);

                            IndexedItems.AddRange(items);
                        }

                        Log.Information($"{IndexedItems.Count} items have been indexed.");
                    };

                    progressDialog.Show();
                }
            }
        }

        public void ClearFiles()
        {
            DeleteAll();
        }

        public void Exit()
        {
            Application.Current.Shutdown();
        }

        // List
        public void Select()
        {
            foreach (var item in SelectedItems)
            {
                item.IsSelected = true;
            }
        }

        public void SelectAll()
        {
            foreach (var item in IndexedItems)
            {
                item.IsSelected = true;
            }
        }

        public void Deselect()
        {
            foreach (var item in SelectedItems)
            {
                item.IsSelected = false;
            }
        }

        public void DeselectAll()
        {
            foreach (var item in IndexedItems)
            {
                item.IsSelected = false;
            }
        }

        public void Reverse()
        {
            foreach (var item in IndexedItems)
            {
                item.IsSelected = !item.IsSelected;
            }
        }

        public void Delete()
        {
            List<IndexedItem> targetItems = SelectedItems.ToList();

            foreach (var item in targetItems)
            {
                IndexedItems.Remove(item);
            }
        }

        public void DeleteAll()
        {
            IndexedItems.Clear();
            ResetViewer();
            GC.Collect();
        }

        #endregion
    }
}
