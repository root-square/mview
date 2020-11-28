using Microsoft.WindowsAPICodePack.Dialogs;
using MView.Entities;
using MView.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MView.Forms
{
    public partial class CryptographyForm : Form
    {
        MainForm _main;

        public CryptographyForm(MainForm main)
        {
            InitializeComponent();

            _main = main;
        }

        #region ::UI::

        private void saveDirectoryButton_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.Title = "Open folder...";
            dialog.Multiselect = false;
            dialog.IsFolderPicker = true;
            dialog.Multiselect = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.DefaultFileName = string.Empty; // Default file name

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (!Directory.Exists(dialog.FileName))
                {
                    MessageBox.Show("Directory not found.", "MView", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                saveDirectoryBox.Text = dialog.FileName;

                _main.AddReport(ReportType.Completed, "Open-Folder operation completed.");
            }
            else
            {
                _main.AddReport(ReportType.Caution, "Open-Folder operation aborted.");
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
                JObject system = JObject.Parse(FileManager.ReadTextFile(dialog.FileName, Encoding.UTF8));

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

        private void codeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            codeBox.Enabled = codeCheckBox.Checked;
            codeButton.Enabled = codeCheckBox.Checked;
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

            saveDirectoryBox.Enabled = false;
            saveDirectoryButton.Enabled = false;
            codeBox.Enabled = false;
            codeButton.Enabled = false;
            verifyCheckBox.Enabled = false;
            codeCheckBox.Enabled = false;
            encryptButton.Enabled = false;
            decryptButton.Enabled = false;

            Encrypt(_main.IndexFromList(saveDirectoryBox.Text, CryptographyProvider.DecryptedExtensions));

            saveDirectoryBox.Enabled = true;
            saveDirectoryButton.Enabled = true;
            codeBox.Enabled = true;
            codeButton.Enabled = true;
            verifyCheckBox.Enabled = true;
            codeCheckBox.Enabled = true;
            encryptButton.Enabled = true;
            decryptButton.Enabled = true;
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

            saveDirectoryBox.Enabled = false;
            saveDirectoryButton.Enabled = false;
            codeBox.Enabled = false;
            codeButton.Enabled = false;
            verifyCheckBox.Enabled = false;
            codeCheckBox.Enabled = false;
            encryptButton.Enabled = false;
            decryptButton.Enabled = false;

            Decrypt(_main.IndexFromList(saveDirectoryBox.Text, CryptographyProvider.EncryptedExtensions));

            saveDirectoryBox.Enabled = true;
            saveDirectoryButton.Enabled = true;
            codeBox.Enabled = true;
            codeButton.Enabled = true;
            verifyCheckBox.Enabled = true;
            codeCheckBox.Enabled = true;
            encryptButton.Enabled = true;
            decryptButton.Enabled = true;
        }

        #endregion

        #region ::Encrypt/Decrypt::

        private void Encrypt(Dictionary<string, string> files)
        {
            _main.AddReport(ReportType.Information, "Encrypting started.");
            _main.SetStatusLabelText("Working");

            int processedCount = 0;

            foreach (var pair in files)
            {
                try
                {
                    int progressValue = (processedCount / files.Count) * 100;
                    processedCount++;

                    CryptographyProvider.EncryptHeader(pair.Key, pair.Value, codeBox.Text);
                    _main.AddReport(ReportType.Information, $"({processedCount}/{files.Count}) Encrypted '{pair.Key}'.");
                }
                catch (Exception ex)
                {
                    _main.AddReport(ReportType.Warning, $"{pair.Key}, {ex.Message}, {ex.StackTrace}");
                }
            }

            _main.AddReport(ReportType.Completed, "Encrypting completed.");
            _main.SetStatusLabelText("Completed");
        }

        private void Decrypt(Dictionary<string, string> files)
        {
            _main.AddReport(ReportType.Information, "Decrypting started.");
            _main.SetStatusLabelText("Working");

            int processedCount = 0;

            foreach (var pair in files)
            {
                try
                {
                    int progressValue = (processedCount / files.Count) * 100;
                    processedCount++;

                    // Verify fake header.
                    if (verifyCheckBox.Checked)
                    {
                        if (!CryptographyProvider.VerifyFakeHeader(pair.Key))
                        {
                            _main.AddReport(ReportType.Caution, $"Ignored '{pair.Key}'.");
                            continue;
                        }
                    }

                    // Decrypt or restore.
                    if (codeCheckBox.Checked)
                    {
                        CryptographyProvider.DecryptHeader(pair.Key, pair.Value, codeBox.Text);
                        _main.AddReport(ReportType.Information, $"({processedCount}/{files.Count}) Decrypted '{pair.Key}'.");
                    }
                    else
                    {
                        if (Path.GetExtension(pair.Key).ToLower() != ".rpgmvo")
                        {
                            CryptographyProvider.RestoreHeader(pair.Key, pair.Value);
                            _main.AddReport(ReportType.Information, $"({processedCount}/{files.Count}) Decrypted '{pair.Key}'.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _main.AddReport(ReportType.Warning, $"{pair.Key}, {ex.Message}, {ex.StackTrace}");
                }
            }

            _main.AddReport(ReportType.Completed, "Decrypting completed.");
            _main.SetStatusLabelText("Completed");
        }

        #endregion
    }
}
