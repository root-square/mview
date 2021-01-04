using Microsoft.Win32;
using MView.Bases;
using MView.Commands;
using MView.Core;
using MView.Core.Cryptography;
using MView.Entities;
using MView.ViewModels.File;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MView.ViewModels.ToolPage
{
    public class ResourceEncrypterViewModel : ViewModelBase
    {
        #region ::Fields::

        private string _encryptionKey = string.Empty;
        private string _saveDirectory = string.Empty;
        private string _backupDirectory = string.Empty;

        private bool _isUseRMMV = true;
        private bool _isBackupFiles = false;

        private ICommand _encryptionKeyCommand;
        private ICommand _saveDirectoryCommand;
        private ICommand _backupDirectoryCommand;
        private ICommand _encryptCommand;

        #endregion

        #region ::Properties::

        public string EncryptionKey
        {
            get
            {
                return _encryptionKey;
            }
            set
            {
                _encryptionKey = value;
                RaisePropertyChanged();
            }
        }

        public string SaveDirectory
        {
            get
            {
                return _saveDirectory;
            }
            set
            {
                _saveDirectory = value;
                RaisePropertyChanged();
            }
        }

        public string BackupDirectory
        {
            get
            {
                return _backupDirectory;
            }
            set
            {
                _backupDirectory = value;
                RaisePropertyChanged();
            }
        }

        public bool IsUseRMMV
        {
            get
            {
                return _isUseRMMV;
            }
            set
            {
                _isUseRMMV = value;
                RaisePropertyChanged();
            }
        }

        public bool IsBackupFiles
        {
            get
            {
                return _isBackupFiles;
            }
            set
            {
                _isBackupFiles = value;
                RaisePropertyChanged();
            }
        }

        public ICommand EncryptionKeyCommand
        {
            get
            {
                return (_encryptionKeyCommand) ?? (_encryptionKeyCommand = new DelegateCommand(OnClickEncryptionKeyGetter));
            }
        }

        public ICommand SaveDirectoryCommand
        {
            get
            {
                return (_saveDirectoryCommand) ?? (_saveDirectoryCommand = new DelegateCommand(OnClickSaveDirectoryGetter));
            }
        }

        public ICommand BackupDirectoryCommand
        {
            get
            {
                return (_backupDirectoryCommand) ?? (_backupDirectoryCommand = new DelegateCommand(OnClickBackupDirectoryGetter));
            }
        }

        public ICommand EncryptCommand
        {
            get
            {
                return (_encryptCommand) ?? (_encryptCommand = new DelegateCommand(OnClickEncrypt));
            }
        }

        #endregion

        #region ::Methods::

        private string GetModifiedFilePath(string filePath, bool isUseRMMV = true)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            string encryptedExtension = Path.GetExtension(filePath).ToLower();


            if (extension == ".ogg")
            {
                if (isUseRMMV)
                {
                    encryptedExtension = ".rpgmvo";
                }
                else
                {
                    encryptedExtension = ".ogg_";
                }
            }
            else if (extension == ".m4a")
            {
                if (isUseRMMV)
                {
                    encryptedExtension = ".rpgmvm";
                }
                else
                {
                    encryptedExtension = ".m4a_";
                }
            }
            else if (extension == ".wav")
            {
                if (isUseRMMV)
                {
                    encryptedExtension = ".rpgmvw";
                }
                else
                {
                    encryptedExtension = ".wav_";
                }
            }
            else if (extension == ".png")
            {
                if (isUseRMMV)
                {
                    encryptedExtension = ".rpgmvp";
                }
                else
                {
                    encryptedExtension = ".png_";
                }
            }

            return Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + encryptedExtension);
        }

        private void Encrypt(Dictionary<string, string> files, string encryptionKey)
        {
            Workspace.Instance.SetStatus(TaskStatusType.Working, "Encrypting resources...");

            int processedCount = 0;

            foreach (var pair in files)
            {
                try
                {
                    int progressValue = (processedCount / files.Count) * 100;
                    processedCount++;

                    CryptographyProvider.EncryptHeader(pair.Key, pair.Value, encryptionKey);
                    Workspace.Instance.SetStatus(TaskStatusType.Working, $"Encrypting resources...({processedCount}/{files.Count})");
                }
                catch (Exception ex)
                {
                    Workspace.Instance.Report.AddReportWithIdentifier($"{ex.Message}\r\n{ex.StackTrace}", ReportType.Warning);
                }
            }

            Workspace.Instance.SetStatus(TaskStatusType.Completed, "Completed.");
        }

        #endregion

        #region ::Command Actions::

        private void OnClickEncryptionKeyGetter()
        {
            // Configure open file dialog box
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.CheckFileExists = true;
            dialog.ValidateNames = true;
            dialog.FileName = string.Empty; // Default file name
            dialog.Filter = "System Data|system.json"; // Filter files by extension


            // Process open file dialog box results
            if (dialog.ShowDialog() == true)
            {
                // Get encryption code.
                JObject system = JObject.Parse(FileManager.ReadTextFile(dialog.FileName, Encoding.UTF8));

                if (system.ContainsKey("encryptionKey"))
                {
                    EncryptionKey = system["encryptionKey"].ToString();
                    Workspace.Instance.Report.AddReportWithIdentifier($"Cached a encryption code.({system["encryptionKey"]})", ReportType.Information);
                }
                else
                {
                    Workspace.Instance.Report.AddReportWithIdentifier($"Encryption key could not be found.({dialog.FileName})", ReportType.Caution);
                }
            }
        }

        private void OnClickSaveDirectoryGetter()
        {
            // Configure open file dialog box
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.ValidateNames = true;
            dialog.FileName = "Directory"; // Default file name
            dialog.Filter = "All Files|*.*"; // Filter files by extension


            // Process open file dialog box results
            if (dialog.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(dialog.FileName))
                {
                    SaveDirectory = Directory.GetParent(dialog.FileName).FullName;
                    Workspace.Instance.Report.AddReportWithIdentifier($"Save directory specified.({SaveDirectory})", ReportType.Information);
                }
            }
        }

        private void OnClickBackupDirectoryGetter()
        {
            // Configure open file dialog box
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.ValidateNames = true;
            dialog.FileName = "Directory"; // Default file name
            dialog.Filter = "All Files|*.*"; // Filter files by extension


            // Process open file dialog box results
            if (dialog.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(dialog.FileName))
                {
                    BackupDirectory = Directory.GetParent(dialog.FileName).FullName;
                    Workspace.Instance.Report.AddReportWithIdentifier($"Backup directory specified.({BackupDirectory})", ReportType.Information);
                }
            }
        }

        private async void OnClickEncrypt()
        {
            // Check options.
            if (string.IsNullOrEmpty(_saveDirectory))
            {
                MessageBox.Show("Save directory path cannot be null or empty.", "MView", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Directory.Exists(_saveDirectory))
            {
                MessageBox.Show("Save directory does not exist.", "MView", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_encryptionKey) || !Regex.IsMatch(_encryptionKey, @"^[a-zA-Z0-9]{32,32}$"))
            {
                MessageBox.Show("Invalid encryption key is inputted.", "MView", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (IsBackupFiles)
            {
                if (string.IsNullOrEmpty(_backupDirectory))
                {
                    MessageBox.Show("Backup directory path cannot be null or empty.", "MView", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!Directory.Exists(_backupDirectory))
                {
                    MessageBox.Show("Backup directory does not exist.", "MView", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            // Check files.
            if (ToolHostWithExplorerViewModel.Instance.IsUseCurrentFile)
            {
                if (Workspace.Instance.ActiveDocument != null)
                {
                    FileViewModelBase file = Workspace.Instance.ActiveDocument;

                    if (file.GetType() != typeof(AudioFileViewModel) && file.GetType() != typeof(ImageFileViewModel))
                    {
                        MessageBox.Show("You cannot encrypt the file that is currently open.", "MView", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }

            if (ToolHostWithExplorerViewModel.Instance.SelectedNodes.Count == 0)
            {
                MessageBox.Show("Select a file to proceed.", "MView", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var task = Task.Run(() =>
            {
                // TODO : 백업 기능 추가, 확장자 변경 시키기.
                if (ToolHostWithExplorerViewModel.Instance.IsUseCurrentFile)
                {
                    Workspace.Instance.SetStatus(TaskStatusType.Working, "Encrypting resource...");

                    string filePath = Workspace.Instance.ActiveDocument.FilePath;
                    string savePath = Path.Combine(_saveDirectory, Path.GetFileName(filePath));
                    CryptographyProvider.EncryptHeader(filePath, savePath, _encryptionKey);

                    Workspace.Instance.SetStatus(TaskStatusType.Completed, "Completed.");
                }
                else
                {
                    Workspace.Instance.SetStatus(TaskStatusType.Loading, "Loading resources...");

                    // Get selected files.
                    string[] extensions = new string[] { ".ogg", ".m4a", ".wav", ".png" };
                    List<string> files = Workspace.Instance.FileExplorer.GetSelectedFiles(ToolHostWithExplorerViewModel.Instance.SelectedNodes.ToList(), extensions.ToList());

                    // Make file dictionary.
                    string baseDirectory = ToolHostWithExplorerViewModel.Instance.Nodes[0].FullName;
                    Dictionary<string, string> fileDictionary = Workspace.Instance.FileExplorer.IndexFromList(files, baseDirectory, _saveDirectory);
                    Dictionary<string, string> modifiedDictionary = new Dictionary<string, string>();
                    
                    foreach (var pair in fileDictionary)
                    {
                        modifiedDictionary.Add(pair.Key, GetModifiedFilePath(pair.Value));
                    }


                    Workspace.Instance.SetStatus(TaskStatusType.Ready, "Resources are loaded successfully.");

                    Encrypt(fileDictionary, _encryptionKey);
                }
            });

            await task;
        }

        #endregion
    }
}
