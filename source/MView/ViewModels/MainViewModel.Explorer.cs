using Caliburn.Micro;
using MView.Utilities.Indexing;
using MView.Utilities.Text;
using Ookii.Dialogs.Wpf;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MView.ViewModels
{
    public partial class MainViewModel
    {
        #region ::Variables::

        private BindableCollection<IndexedItem> _indexedItems = new BindableCollection<IndexedItem>();

        public BindableCollection<IndexedItem> IndexedItems
        {
            get => _indexedItems;
            set => Set(ref _indexedItems, value);
        }

        private IndexedItem _selectedItem = new IndexedItem();

        public IndexedItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value != null)
                {
                    Set(ref _selectedItem, value);
                }
            }
        }

        #endregion

        #region ::Menu Interactions::

        // File
        public void OpenFiles()
        {
            VistaOpenFileDialog openFileDialog = new VistaOpenFileDialog();
            openFileDialog.Title = "Please select files.";
            openFileDialog.Filter = "RPG MV resources|*.rpgmvp;*.rpgmvo;*.rpgmvm;*.rpgmvw|RPG MZ resources|*.png_;*.ogg_;*.m4a_;*.wav_|Resources|*.png;*.ogg;*.m4a;*.wav|All files|*.*";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                // Clear the collection.
                IndexedItems.Clear();

                // Add the selected files.
                string[] selectedFiles = openFileDialog.FileNames;

                // Check whether to index all file extensions.
                bool indexAllExtensions = false;

                using (TaskDialog taskDialog = new TaskDialog())
                {
                    taskDialog.WindowTitle = "MView";
                    taskDialog.MainInstruction = "Do you want to index all file extensions?";
                    taskDialog.Content = "If you click 'OK', MView indexes all file extensions, not just RPG MV/MZ resources file extensions.";
                    taskDialog.Footer = "Some errors can occur if files that are not RPG MV/MZ resources are processed in MView.";
                    taskDialog.FooterIcon = TaskDialogIcon.Warning;

                    TaskDialogButton yesButton = new TaskDialogButton(ButtonType.Yes);
                    TaskDialogButton noButton = new TaskDialogButton(ButtonType.No);
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

                // Process
                ProgressDialog progressDialog = new ProgressDialog()
                {
                    WindowTitle = "MView",
                    Text = "MView File Indexer",
                    Description = "Indexing selected files...",
                    ShowTimeRemaining = true,
                    ShowCancelButton = false,
                    ProgressBarStyle = ProgressBarStyle.MarqueeProgressBar
                };

                progressDialog.DoWork += (sender, e) => {
                    Thread.Sleep(1000);

                    progressDialog.ProgressBarStyle = ProgressBarStyle.ProgressBar;

                    List<string> extensions = Settings.KnownExtensions.ToList();

                    for (int i = 0; i < selectedFiles.Length; i++)
                    {
                        IndexedItem? item = IndexingManager.GetFile(new FileInfo(selectedFiles[i]), indexAllExtensions ? null : extensions);

                        if (item != null)
                        {
                            IndexedItems.Add(item);
                        }

                        int progress = i / selectedFiles.Length * 100;
                        progressDialog.ReportProgress(progress, null, string.Format(System.Globalization.CultureInfo.CurrentCulture, "Indexing selected files: {0}%", progress));
                    }

                    Log.Information($"{IndexedItems.Count} files have been indexed.");
                };

                progressDialog.Show();
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
                // Clear the collection.
                IndexedItems.Clear();

                // Add the selected files.
                string[] selectedPaths = folderBrowserDialog.SelectedPaths;

                // Check whether to index all file extensions.
                bool indexAllExtensions = false;

                using (TaskDialog taskDialog = new TaskDialog())
                {
                    taskDialog.WindowTitle = "MView";
                    taskDialog.MainInstruction = "Do you want to index all file extensions?";
                    taskDialog.Content = "If you click 'OK', MView indexes all file extensions, not just RPG MV/MZ resources file extensions.";
                    taskDialog.Footer = "Some errors can occur if files that are not RPG MV/MZ resources are processed in MView.";
                    taskDialog.FooterIcon = TaskDialogIcon.Warning;

                    TaskDialogButton yesButton = new TaskDialogButton(ButtonType.Yes);
                    TaskDialogButton noButton = new TaskDialogButton(ButtonType.No);
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

                // Process
                ProgressDialog progressDialog = new ProgressDialog()
                {
                    WindowTitle = "MView",
                    Text = "MView File Indexer",
                    Description = "Indexing selected folders...",
                    ShowTimeRemaining = true,
                    ShowCancelButton = false,
                    ProgressBarStyle = ProgressBarStyle.MarqueeProgressBar
                };

                progressDialog.DoWork += (sender, e) => {
                    Thread.Sleep(1000);

                    progressDialog.ProgressBarStyle = ProgressBarStyle.ProgressBar;

                    List<string> extensions = Settings.KnownExtensions.ToList();

                    for (int i = 0; i < selectedPaths.Length; i++)
                    {
                        List<IndexedItem> items = IndexingManager.GetFiles(new DirectoryInfo(selectedPaths[i]), indexAllExtensions ? null : extensions);

                        IndexedItems.AddRange(items);

                        int progress = i / selectedPaths.Length * 100;
                        progressDialog.ReportProgress(progress, null, string.Format(System.Globalization.CultureInfo.CurrentCulture, "Indexing selected folders: {0}%", progress));
                    }

                    Log.Information($"{IndexedItems.Count} folders have been indexed.");
                };

                progressDialog.Show();
            }
        }

        public void ClearFiles()
        {
            IndexedItems.Clear();

            Log.Information($"Indexed items are cleared.");
        }

        public void Exit()
        {
            Application.Current.Shutdown();
        }

        // List
        public void Select()
        {
            SelectedItem.IsSelected = false;
        }

        public void SelectAll()
        {

        }

        public void Deselect()
        {
            
        }

        public void DeselectAll()
        {

        }

        public void Reverse()
        {

        }

        public void Delete()
        {

        }

        public void DeleteAll()
        {

        }

        #endregion
    }
}
