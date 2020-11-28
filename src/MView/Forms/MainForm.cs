using Microsoft.WindowsAPICodePack.Dialogs;
using MView.Entities;
using MView.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            dialog.Filter = "Supported Files (*.rpgmvo, *.rpgmvm, *.rpgmvw, *.rpgmvp, *.ogg, *.m4a, *.wav, *.png)|*.rpgmvo;*.rpgmvm;*.rpgmvw;*.rpgmvp;*.ogg;*.m4a;*.wav;*.png"; // Filter files by extension


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

                    // Add file to list.
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
            AddReport(ReportType.Information, "Open-Folders operation started.");

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.Title = "Open folders...";
            dialog.Multiselect = true;
            dialog.IsFolderPicker = true;
            dialog.Multiselect = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.DefaultFileName = string.Empty; // Default file name

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                foreach (string path in dialog.FileNames)
                {
                    ListViewItem item = new ListViewItem(DocumentType.DIRECTORY.ToString());
                    item.SubItems.Add(Path.GetFileName(path));
                    item.SubItems.Add(GetDirectorySize(path));
                    item.SubItems.Add(path);

                    item.Checked = true;

                    fileList.Items.Add(item);
                }

                AddReport(ReportType.Completed, "Open-Folders operation completed.");
            }
            else
            {
                AddReport(ReportType.Caution, "Open-Folders operation aborted.");
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

        private void taskTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form = new CryptographyForm(this);
            form.ShowDialog();
        }

        private void informationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form = new InformationForm();
            form.ShowDialog();
        }
        
        #endregion

        #region ::File Indexer::

        private string GetModifiedExtension(string extension)
        {
            string result = string.Empty;

            switch (extension.ToLower())
            {
                // Cryptography section.
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
                default:
                    result = ".dat";
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

        public Dictionary<string, string> IndexFromList(string saveDirectory, string[] extensions)
        {
            AddReport(ReportType.Information, "Indexing started.");
            SetStatusLabelText("Indexing");

            List<ListViewItem> items = new List<ListViewItem>();
            Dictionary<string, string> files = new Dictionary<string, string>();

            // Get checked items.
            foreach (ListViewItem item in fileList.Items)
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
                    string fileType = item.SubItems[0].Text;
                    string path = item.SubItems[3].Text;

                    if (fileType == "DIRECTORY") // Directory indexing.
                    {
                        AddReport(ReportType.Information, $"Catched a directory. => '{path}'");

                        List<string> tempList = FileManager.GetFiles(path, new List<string>(extensions));

                        foreach (string temp in tempList)
                        {
                            // Create expected save path.
                            string relativePath = temp.Replace(Path.GetDirectoryName(path) + @"\", string.Empty);
                            string savePath = Path.Combine(saveDirectory, relativePath);
                            savePath = GetModifiedPath(savePath);

                            AddReport(ReportType.Information, $"Indexed '{temp}' -> '{savePath}'.");
                            files.Add(temp, savePath);
                        }
                    }
                    else // Files indexing.
                    {
                        if (extensions.Contains(Path.GetExtension(path)))
                        {
                            string savePath = Path.Combine(saveDirectory, saveDirectory, Path.GetFileName(path));
                            savePath = GetModifiedPath(savePath);

                            AddReport(ReportType.Information, $"Indexed '{path}' -> '{savePath}'.");
                            files.Add(path, savePath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    AddReport(ReportType.Warning, $"{ex.Message}, {ex.StackTrace}");
                }
            }

            AddReport(ReportType.Completed, "Indexing completed.");

            return files;
        }

        #endregion
    }
}
