using Caliburn.Micro;
using MView.Utilities;
using MView.Utilities.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ookii.Dialogs.Wpf;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MView.ViewModels.Dialogs
{
    public class DecrypterViewModel : Screen
    {
        private string _encryptionKey = "00000000000000000000000000000000";

        public string EncryptionKey
        {
            get => _encryptionKey;
            set => Set(ref _encryptionKey, value);
        }

        private string _outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public string OutputDirectory
        {
            get => _outputDirectory;
            set => Set(ref _outputDirectory, value);
        }

        private bool _verifyFakeHeader = true;

        public bool VerifyFakeHeader
        {
            get => _verifyFakeHeader;
            set => Set(ref _verifyFakeHeader, value);
        }

        private bool _decryptAllFiles = false;

        public bool DecryptAllFiles
        {
            get => _decryptAllFiles;
            set => Set(ref _decryptAllFiles, value);
        }

        public void ExploreEncryptionKey()
        {
            VistaOpenFileDialog dialog = new VistaOpenFileDialog();
            dialog.Title = "Please select a System.json file";
            dialog.Filter = "RPG MV/MZ System Data|System.json";
            dialog.Multiselect = false;
            dialog.CheckFileExists = true;

            if (dialog.ShowDialog() == true)
            {
                // Get encryption code.
                try
                {
                    JObject system = JObject.Parse(TextManager.ReadTextFile(dialog.FileName, Encoding.UTF8));

                    if (system.ContainsKey("encryptionKey"))
                    {
                        EncryptionKey = system["encryptionKey"].ToString();
                        Log.Verbose($"The decrypter catched a encryption key. ({EncryptionKey})");
                    }
                    else
                    {
                        Log.Warning($"A encryption key can not be found.");
                        MessageBox.Show("A encryption key can not be found.", "MView", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                catch (JsonReaderException ex)
                {
                    Log.Warning(ex, "Unable to load the file. It seems to be an invalid JSON file.");
                    MessageBox.Show("Unable to load the file. It seems to be an invalid JSON file.", "MView", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        public void ExploreOutputPath()
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Please select a folder.";
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.

            if (dialog.ShowDialog() == true)
            {
                OutputDirectory = dialog.SelectedPath;
            }
        }

        public async void Cancel()
        {
            await TryCloseAsync(false);
        }

        public async void Confirm(Window window)
        {
            if (window == null)
            {
                Log.Warning("The dialog shouldn't be null.");
                return;
            }

            if (!ValidationHelper.IsValid(window))
            {
                MessageBox.Show("Invalid value entered.", "MView", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Log.Warning("Invalid value entered.");
                return;
            }

            await TryCloseAsync(true);
        }
    }
}
