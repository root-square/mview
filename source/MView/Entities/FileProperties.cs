using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace MView.Entities
{
    public class FileProperties
    {
        #region ::Constructors::

        public FileProperties()
        {
            Name = "";

            FullName = "";

            Extension = "";

            Size = GetFileSize(0);

            IsReadOnly = false;

            LastAccessTime = DateTime.UnixEpoch;
            LastAccessTimeUtc = DateTime.UnixEpoch;

            LastWriteTime = DateTime.UnixEpoch;
            LastWriteTimeUtc = DateTime.UnixEpoch;
        }

        public FileProperties(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The file properties could not be displayed because the file could not be found.");
            }

            FileInfo fileInfo = new FileInfo(filePath);

            Name = fileInfo.Name;

            FullName = fileInfo.FullName;

            Extension = fileInfo.Extension;

            Size = GetFileSize(fileInfo.Length);

            IsReadOnly = fileInfo.IsReadOnly;

            LastAccessTime = fileInfo.LastAccessTime;
            LastAccessTimeUtc = fileInfo.LastAccessTimeUtc;

            LastWriteTime = fileInfo.LastWriteTime;
            LastWriteTimeUtc = fileInfo.LastWriteTimeUtc;
        }

        #endregion

        #region ::Properties::

        [Category("File")]
        [ReadOnly(true)]
        public string Name { get; set; }

        [Category("File")]
        [ReadOnly(true)]
        public string FullName { get; set; }

        [Category("File")]
        [ReadOnly(true)]
        public string Extension { get; set; }

        [Category("File")]
        [ReadOnly(true)]
        public string Size { get; set; }

        [Category("File")]
        [ReadOnly(true)]
        public bool IsReadOnly { get; set; }

        [Category("Time")]
        [ReadOnly(true)]
        public DateTime LastAccessTime { get; set; }

        [Category("Time")]
        [ReadOnly(true)]
        public DateTime LastAccessTimeUtc { get; set; }

        [Category("Time")]
        [ReadOnly(true)]
        public DateTime LastWriteTime { get; set; }

        [Category("Time")]
        [ReadOnly(true)]
        public DateTime LastWriteTimeUtc { get; set; }

        #endregion

        #region ::Methods::

        private string GetFileSize(long length)
        {
            double byteCount = length;

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

        #endregion
    }
}
