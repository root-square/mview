using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MView.Entities
{
    public class DirectoryItem
    {
        #region ::Constructors::

        public DirectoryItem(DirectoryInfo directory, bool isExpanded = false, bool isBase = false)
        {
            if (isBase)
            {
                Type = DirectoryItemType.BaseDirectory;
            }
            else
            {
                Type = DirectoryItemType.Directory;
            }

            Icon = GetIcon(Type, directory.FullName);
            Name = directory.Name;
            FullName = directory.FullName;
            IsExpanded = isExpanded;

            List<DirectoryItem> items = new List<DirectoryItem>();

            try
            {
                DirectoryInfo[] subDirectories = directory.GetDirectories();

                foreach (DirectoryInfo subDirectory in subDirectories)
                {
                    items.Add(new DirectoryItem(subDirectory, false));
                }
            }
            catch { }

            try
            {
                FileInfo[] files = directory.GetFiles();

                foreach (FileInfo file in files)
                {
                    items.Add(new DirectoryItem(file));
                }
            }
            catch { }

            SubItems = items;
        }

        public DirectoryItem(FileInfo file, bool isExpanded = false)
        {
            Type = DirectoryItemType.File;
            Icon = GetIcon(Type, file.FullName);
            Name = file.Name;
            FullName = file.FullName;
            IsExpanded = isExpanded;
            SubItems = new List<DirectoryItem>();
        }

        #endregion

        #region ::Properties::

        public DirectoryItemType Type { get; set; }

        public ImageSource Icon { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public bool IsExpanded { get; set; }

        public List<DirectoryItem> SubItems { get; set; }

        #endregion

        #region ::Methods::

        private ImageSource GetIcon(DirectoryItemType type, string filePath)
        {
            BitmapImage bitmap = new BitmapImage();

            if (type == DirectoryItemType.BaseDirectory)
            {
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("pack://application:,,/Resources/icon_basedirectory.png");
                bitmap.EndInit();
            }
            else if (type == DirectoryItemType.Directory)
            {
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("pack://application:,,/Resources/icon_directory.png");
                bitmap.EndInit();
            }
            else
            {
                string extension = Path.GetExtension(filePath).ToLower();

                if (extension == ".ogg" || extension == ".m4a" || extension == ".wav")
                {
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("pack://application:,,/Resources/icon_audiofile.png");
                    bitmap.EndInit();
                }
                else if (extension == ".png")
                {
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("pack://application:,,/Resources/icon_image.png");
                    bitmap.EndInit();
                }
                else if (extension == ".rpgmvo" || extension == ".rpgmvm" || extension == ".rpgmvw" || extension == ".rpgmvp" || extension == ".ogg_" || extension == ".m4a_" || extension == ".wav_" || extension == ".png_")
                {
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("pack://application:,,/Resources/icon_encrypted.png");
                    bitmap.EndInit();
                }
                else if (extension == ".json" || extension == ".script")
                {
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("pack://application:,,/Resources/icon_code.png");
                    bitmap.EndInit();
                }
                else if (extension == ".rpgsave")
                {
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("pack://application:,,/Resources/icon_savedata.png");
                    bitmap.EndInit();
                }
                else
                {
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("pack://application:,,/Resources/icon_file.png");
                    bitmap.EndInit();
                }
            }

            return bitmap;
        }

        #endregion
    }
}
