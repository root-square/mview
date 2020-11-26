using MView.Entities;
using MView.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MView.Forms
{
    public partial class CryptographyForm : Form
    {
        MainForm _main;

        Settings _settings = Settings.Instance;

        public CryptographyForm(MainForm main)
        {
            InitializeComponent();

            _main = main;

            codeBox.Enabled = _settings.UseEncryptionCodeFlag;
            codeButton.Enabled = _settings.UseEncryptionCodeFlag;
            saveDirectoryBox.Text = _settings.CryptoSavePath;
            verifyCheckBox.Checked = _settings.VerifyFakeHeaderFlag;
            codeCheckBox.Checked = _settings.UseEncryptionCodeFlag;
        }

        #region ::UI::

        private void saveDirectoryButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Please select the folder where the datas will be stored.";
            dialog.ShowNewFolderButton = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _main.AddReport(ReportType.Information, $"Crypto save directory selected. => '{dialog.SelectedPath}'");
                saveDirectoryBox.Text = dialog.SelectedPath;
            }
        }

        private void codeButton_Click(object sender, EventArgs e)
        {
            // Configure open file dialog box
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open file...";
            dialog.Multiselect = false;
            dialog.CheckFileExists = true;
            dialog.ValidateNames = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.FileName = string.Empty; // Default file name
            dialog.Filter = "System File (System.json)|System.json"; // Filter files by extension


            // Process open file dialog box results
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Get encryption code.
                JObject system = JObject.Parse(FileUtility.ReadTextFile(dialog.FileName, Encoding.UTF8));
                
                if (system.ContainsKey("encryptionKey"))
                {
                    _main.AddReport(ReportType.Information, $"Catched encryption code. => '{system["encryptionKey"]}");
                    codeBox.Text = system["encryptionKey"].ToString();
                }
                else
                {
                    _main.AddReport(ReportType.Caution, $"Encryption code could not be found.");
                    MessageBox.Show("Encryption code could not be found.", "MView", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void verifyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _settings.VerifyFakeHeaderFlag = verifyCheckBox.Checked;
        }

        private void codeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            codeBox.Enabled = codeCheckBox.Checked;
            codeButton.Enabled = codeCheckBox.Checked;
            _settings.UseEncryptionCodeFlag = codeCheckBox.Checked;
        }

        private void encryptButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(saveDirectoryBox.Text))
            {
                _main.AddReport(ReportType.Caution, $"Save directory path cannot be null or empty.");
                MessageBox.Show("Save directory path cannot be null or empty.", "MView", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(saveDirectoryBox.Text))
            {
                _main.AddReport(ReportType.Caution, $"Save directory does not exist.");
                MessageBox.Show("Save directory does not exist.", "MView", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!codeCheckBox.Checked)
            {
                _main.AddReport(ReportType.Caution, $"The encryption operation cannot proceed because the encryption code has not been entered.");
                MessageBox.Show("The encryption operation cannot proceed because the encryption code has not been entered.", "MView", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (codeCheckBox.Checked && !Regex.IsMatch(codeBox.Text, @"^[a-zA-Z0-9]{32,32}$"))
            {
                _main.AddReport(ReportType.Caution, $"Invalid encryption code entered. => '{codeBox.Text}'");
                MessageBox.Show("Invalid encryption code entered.", "MView", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Run crypto task with encrypt service.
            EncryptService(IndexFromList(CryptographyUtility.DecryptedExtensions, CryptographyUtility.EncryptedExtensions));
        }

        private void decryptButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(saveDirectoryBox.Text))
            {
                _main.AddReport(ReportType.Caution, $"Save directory path cannot be null or empty.");
                MessageBox.Show("Save directory path cannot be null or empty.", "MView", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(saveDirectoryBox.Text))
            {
                _main.AddReport(ReportType.Caution, $"Save directory does not exist.");
                MessageBox.Show("Save directory does not exist.", "MView", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (codeCheckBox.Checked && !Regex.IsMatch(codeBox.Text, @"^[a-zA-Z0-9]{32,32}$"))
            {
                _main.AddReport(ReportType.Caution, $"Invalid encryption code entered. => '{codeBox.Text}'");
                MessageBox.Show("Invalid encryption code entered.", "MView", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Run crypto task with decrypt service.
            DecryptService(IndexFromList(CryptographyUtility.EncryptedExtensions, CryptographyUtility.DecryptedExtensions));
        }

        #endregion

        private string GetModifiedExtension(string extension)
        {
            string result = string.Empty;

            switch (extension.ToLower())
            {
                case ".ogg":
                    result = ".rpgmvo";
                    break;
                case ".m4a":
                    result = ".rpgmvm";
                    break;
                case ".wav":
                    result = ".rpgmvw";
                    break;
                case ".png":
                    result = ".rpgmvp";
                    break;
                case ".rpgmvo":
                    result = ".ogg";
                    break;
                case ".rpgmvm":
                    result = ".m4a";
                    break;
                case ".rpgmvw":
                    result = ".wav";
                    break;
                case ".rpgmvp":
                    result = ".png";
                    break;
            }

            return result;
        }

        private string GetModifiedPath(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            string modifiedExtension = GetModifiedExtension(extension);

            return Path.Combine(directory, fileName + modifiedExtension);
        }

        private Dictionary<string, string> IndexFromList(string[] originalExtensions, string[] modifiedExtensions)
        {
            // Indexing.
            _main.AddReport(ReportType.Information, "Indexing started.");
            _main.SetStatusLabelText("Indexing");
            _main.SetStatusBarStyle(ProgressBarStyle.Marquee);

            List<ListViewItem> items = new List<ListViewItem>();
            Dictionary<string, string> files = new Dictionary<string, string>();

            // Get checked items.
            foreach (ListViewItem item in _main.fileList.Items)
            {
                if (item.Checked)
                {
                    items.Add(item);
                }
            }

            foreach (ListViewItem item in items)
            {
                try
                {
                    string type = item.SubItems[0].Text;
                    string path = item.SubItems[3].Text;

                    if (type == "DIRECTORY") // Directory indexing.
                    {
                        string parentPath = Directory.GetParent(path).FullName;
                        List<string> tempList = FileUtility.GetFiles(path, new List<string>(originalExtensions));

                        foreach (string temp in tempList)
                        {
                            // Create expected save path.
                            string relativePath = temp.Replace(parentPath, string.Empty);
                            string savePath = Path.Combine(saveDirectoryBox.Text, relativePath);

                            _main.AddReport(ReportType.Information, $"Indexed '{temp}'.");
                            files.Add(temp, GetModifiedPath(savePath));
                        }
                    }
                    else
                    {
                        if (originalExtensions.Contains(Path.GetExtension(path)))
                        {
                            string savePath = Path.Combine(saveDirectoryBox.Text, Path.GetFileName(path));

                            _main.AddReport(ReportType.Information, $"Indexed '{path}'.");
                            files.Add(path, GetModifiedPath(savePath));
                        }
                    }
                }
                catch (Exception ex)
                {
                    _main.AddReport(ReportType.Warning, $"{ex.Message}, {ex.StackTrace}");
                }
            }

            _main.AddReport(ReportType.Completed, "Indexing completed.");
            _main.SetStatusBarStyle(ProgressBarStyle.Blocks);

            return files;
        }

        private void EncryptService(Dictionary<string, string> files)
        {
            // Encrypting.
            _main.AddReport(ReportType.Information, "Encrypting started.");
            _main.SetStatusLabelText("Working");
            _main.SetStatusBarStyle(ProgressBarStyle.Marquee);

            progressBar.Value = 0;

            int processedCount = 0;

            foreach (var pair in files)
            {
                try
                {
                    int progressValue = (processedCount / files.Count) * 100;
                    _main.SetStatusLabelText($"Working ({processedCount}, {files.Count})");
                    progressBar.Value = progressValue;
                    processedCount++;

                    CryptographyUtility.EncryptHeader(pair.Key, pair.Value, codeBox.Text);
                    _main.AddReport(ReportType.Information, $"Encrypted '{pair.Key}'.");
                }
                catch (Exception ex)
                {
                    _main.AddReport(ReportType.Warning, $"{ex.Message}, {ex.StackTrace}");
                }
            }

            _main.AddReport(ReportType.Completed, "Encrypting completed.");
            _main.SetStatusLabelText("Completed");
            _main.SetStatusBarStyle(ProgressBarStyle.Blocks);
        }

        private void DecryptService(Dictionary<string, string> files)
        {
            // Decrypting.
            _main.AddReport(ReportType.Information, "Decrypting started.");
            _main.SetStatusLabelText("Working");
            _main.SetStatusBarStyle(ProgressBarStyle.Marquee);

            progressBar.Value = 0;

            int processedCount = 0;

            foreach (var pair in files)
            {
                try
                {
                    int progressValue = (processedCount / files.Count) * 100;
                    _main.SetStatusLabelText($"Working ({processedCount}, {files.Count})");
                    progressBar.Value = progressValue;
                    processedCount++;

                    // Verify fake header.
                    if (verifyCheckBox.Checked)
                    {
                        if (!CryptographyUtility.VerifyFakeHeader(pair.Key))
                        {
                            _main.AddReport(ReportType.Caution, $"Ignored '{pair.Key}'.");
                            continue;
                        }
                    }

                    // Decrypt or restore.
                    if (codeCheckBox.Checked)
                    {
                        CryptographyUtility.DecryptHeader(pair.Key, pair.Value, codeBox.Text);
                        _main.AddReport(ReportType.Information, $"Decrypted '{pair.Key}'.");
                    }
                    else
                    {
                        if (Path.GetExtension(pair.Key).ToLower() != ".rpgmvo")
                        {
                            CryptographyUtility.RestoreHeader(pair.Key, pair.Value);
                            _main.AddReport(ReportType.Information, $"Decrypted '{pair.Key}'.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _main.AddReport(ReportType.Warning, $"{ex.Message}, {ex.StackTrace}");
                }
            }

            _main.AddReport(ReportType.Completed, "Decrypting completed.");
            _main.SetStatusLabelText("Completed");
            _main.SetStatusBarStyle(ProgressBarStyle.Blocks);
        }
    }
}
