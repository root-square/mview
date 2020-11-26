using MView.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MView.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        #region ::Report::

        public void AddReport(ReportType type, string text)
        {
            ReportBox.AppendText($"{DateTime.Now.ToString("HH:mm:ss")} [{type.ToString()}] {text}\r\n");
            ReportBox.ScrollToCaret();
        }

        public void ResetReport()
        {
            ReportBox.Text = string.Empty;
        }

        #endregion

        #region ::Status::

        public void SetStatusLabelText(string text)
        {
            statusLabel.Text = text;
        }

        public void SetStatusBarValue(int value)
        {
            statusBar.Value = value;
        }

        public void SetStatusBarStyle(ProgressBarStyle style)
        {
            statusBar.Style = style;
        }

        #endregion

        #region ::UI::

        private string GetFileSize(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            double byteCount = fileInfo.Length;

            string size = "0 Bytes";
            if (byteCount >= 1073741824.0)
                size = string.Format("{0:##.##}", byteCount / 1073741824.0) + " GB";
            else if (byteCount >= 1048576.0)
                size = string.Format("{0:##.##}", byteCount / 1048576.0) + " MB";
            else if (byteCount >= 1024.0)
                size = string.Format("{0:##.##}", byteCount / 1024.0) + " KB";
            else if (byteCount > 0 && byteCount < 1024.0)
                size = byteCount.ToString() + " Bytes";

            return size;
        }

        private string GetDirectorySize(string directoryPath)
        {

            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            FileSystemInfo[] fileSystemInfoArray = directoryInfo.GetFileSystemInfos();

            long byteCount = 0L;

            for (int i = 0; i < fileSystemInfoArray.Length; i++)
            {
                FileInfo fileInfo = fileSystemInfoArray[i] as FileInfo;

                if (fileInfo != null)
                {
                    byteCount += fileInfo.Length;
                }
            }

            string size = "0 Bytes";
            if (byteCount >= 1073741824.0)
                size = string.Format("{0:##.##}", byteCount / 1073741824.0) + " GB";
            else if (byteCount >= 1048576.0)
                size = string.Format("{0:##.##}", byteCount / 1048576.0) + " MB";
            else if (byteCount >= 1024.0)
                size = string.Format("{0:##.##}", byteCount / 1024.0) + " KB";
            else if (byteCount > 0 && byteCount < 1024.0)
                size = byteCount.ToString() + " Bytes";

            return size;
        }

        private void filesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddReport(ReportType.Information, "Open-Files operation started.");

            // Configure open file dialog box
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open files...";
            dialog.Multiselect = true;
            dialog.CheckFileExists = true;
            dialog.ValidateNames = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.FileName = string.Empty; // Default file name
            dialog.Filter = "Supported Files (*.txt, *.json, *.rpgsave, *.rpgmvo, *.rpgmvm, *.rpgmvw, *.rpgmvp, *.ogg, *.m4a, *.wav, *.png)|*.txt;*.json;*.rpgsave;*.rpgmvo;*.rpgmvm;*.rpgmvw;*.rpgmvp;*.ogg;*.m4a;*.wav;*.png"; // Filter files by extension


            // Process open file dialog box results
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string path in dialog.FileNames)
                {
                    DocumentType type = DocumentType.OTHER;

                    switch (Path.GetExtension(path).ToLower())
                    {
                        case "":
                            type = DocumentType.DIRECTORY;
                            break;
                        case ".txt":
                            type = DocumentType.TXT;
                            break;
                        case ".json":
                            type = DocumentType.JSON;
                            break;
                        case ".rpgsave":
                            type = DocumentType.RPGSAVE;
                            break;
                        case ".rpgmvo":
                            type = DocumentType.RPGMVO;
                            break;
                        case ".rpgmvm":
                            type = DocumentType.RPGMVM;
                            break;
                        case ".rpgmvw":
                            type = DocumentType.RPGMVW;
                            break;
                        case ".rpgmvp":
                            type = DocumentType.RPGMVP;
                            break;
                        case ".ogg":
                            type = DocumentType.OGG;
                            break;
                        case ".m4a":
                            type = DocumentType.M4A;
                            break;
                        case ".wav":
                            type = DocumentType.WAV;
                            break;
                        case ".png":
                            type = DocumentType.PNG;
                            break;
                        default:
                            type = DocumentType.OTHER;
                            break;
                    }

                    ListViewItem item = new ListViewItem(type.ToString());
                    item.SubItems.Add(Path.GetFileName(path));
                    item.SubItems.Add(GetFileSize(path));
                    item.SubItems.Add(path);

                    item.Checked = true;

                    fileList.Items.Add(item);
                }

                AddReport(ReportType.Completed, "Open-Files operation completed.");
            }
            else
            {
                AddReport(ReportType.Caution, "Open-Files operation aborted.");
            }
        }

        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddReport(ReportType.Information, "Open-Folder operation started.");

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Please select the folder where the data you want to use is stored.";
            dialog.ShowNewFolderButton = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ListViewItem item = new ListViewItem(DocumentType.DIRECTORY.ToString());
                item.SubItems.Add(Path.GetFileName(dialog.SelectedPath));
                item.SubItems.Add(GetDirectorySize(dialog.SelectedPath));
                item.SubItems.Add(dialog.SelectedPath);

                item.Checked = true;

                fileList.Items.Add(item);

                AddReport(ReportType.Completed, "Open-Folder operation completed.");
            }
            else
            {
                AddReport(ReportType.Caution, "Open-Folder operation aborted.");
            }
        }

        private void exitEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Application.ExitThread();
        }

        private void selectAllAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in fileList.Items)
            {
                item.Checked = true;
            }

            AddReport(ReportType.Information, "List-Select All operation performed.");
        }

        private void deselectAllAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in fileList.Items)
            {
                item.Checked = false;
            }

            AddReport(ReportType.Information, "List-Deselect All operation performed.");
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in fileList.Items)
            {
                if (item.Checked)
                {
                    fileList.Items.RemoveAt(item.Index);
                }
            }

            AddReport(ReportType.Information, "List-Delete operation performed.");
        }

        private void deleteAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileList.Items.Clear();

            AddReport(ReportType.Information, "List-Delete All operation performed.");
        }

        private void cryptographyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form = new CryptographyForm(this);
            form.ShowDialog();
        }

        private void packageUnpackageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form = new RpgsaveForm(this);
            form.ShowDialog();
        }

        private void editEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form = new RpgsaveEditForm(this);
            form.ShowDialog();
        }

        private void importExportTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form = new TranslationForm(this);
            form.ShowDialog();
        }

        private void verifyVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form = new TranslationVerifyForm(this);
            form.ShowDialog();
        }

        private void migrateMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form = new TranslationMigrateForm(this);
            form.ShowDialog();
        }

        private void configureCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form = new ConfigureForm();
            form.ShowDialog();
        }

        private void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/junimiso04/MView");
        }

        private void informationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form = new InformationForm();
            form.ShowDialog();
        }

        #endregion
    }
}
