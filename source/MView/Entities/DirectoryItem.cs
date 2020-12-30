using MView.Bases;
using MView.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MView.Entities
{
    public class DirectoryItem : ViewModelBase
    {
        #region ::Fields::

        private ICommand _itemDoubleClickCommand;

        #endregion

        #region ::Constructors::

        public DirectoryItem()
        {

        }

        public DirectoryItem(FileInfo file, bool isExpanded = false)
        {
            Type = DirectoryItemType.File;
            Icon = GetIcon(Type, file.FullName);
            Name = file.Name;
            FullName = file.FullName;
            IsExpanded = isExpanded;
            IsSelected = false;
            SubItems = new List<DirectoryItem>();
        }

        public DirectoryItem(DirectoryInfo directory, bool isExpanded = false, bool isBase = false)
        {
            if (isBase)
            {
                Type = DirectoryItemType.BaseDirectory;
                Name = directory.FullName;
            }
            else
            {
                Type = DirectoryItemType.Directory;
                Name = directory.Name;
            }

            Icon = GetIcon(Type, directory.FullName);
            FullName = directory.FullName;
            IsExpanded = isExpanded;
            IsSelected = false;

            List<DirectoryItem> items = new List<DirectoryItem>();

            try
            {
                var subDirectories = directory.EnumerateDirectories();

                foreach (DirectoryInfo subDirectory in subDirectories)
                {
                    items.Add(new DirectoryItem(subDirectory, false));
                }
            }
            catch { }

            try
            {
                var files = directory.EnumerateFiles();

                foreach (FileInfo file in files)
                {
                    items.Add(new DirectoryItem(file));
                }
            }
            catch { }

            SubItems = items;
        }

        #endregion

        #region ::Properties::

        public DirectoryItemType Type { get; set; }

        public ImageSource Icon { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public bool IsExpanded { get; set; }

        public bool IsSelected { get; set; }

        public List<DirectoryItem> SubItems { get; set; }

        public ICommand ItemDoubleClickCommand
        {
            get
            {
                return (_itemDoubleClickCommand) ?? (_itemDoubleClickCommand = new DelegateCommand(ItemDoubleClick));
            }
        }

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
                    bitmap.UriSource = new Uri("pack://application:,,/Resources/icon_data.png");
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

        #region ::Command Actions::

        public void ItemDoubleClick()
        {
            if (Type != DirectoryItemType.BaseDirectory && Type != DirectoryItemType.Directory)
            {
                Workspace.Instance.ActiveDocument = Workspace.Instance.OpenFile(FullName);
            }
        }

        #endregion
    }
}
