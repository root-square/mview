using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView.Utilities.Indexing
{
    public class IndexedItem : PropertyChangedBase
    {
        private bool _isSelected = false;

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        private string _fileName = string.Empty;

        public string FileName
        {
            get => _fileName;
            set => Set(ref _fileName, value);
        }

        private string _fullPath = string.Empty;

        public string FullPath
        {
            get => _fullPath;
            set => Set(ref _fullPath, value);
        }

        private string _rootDirectory = string.Empty;

        public string RootDirectory
        {
            get => _rootDirectory;
            set => Set(ref _rootDirectory, value);
        }

        private string _parentDirectory = string.Empty;

        public string ParentDirectory
        {
            get => _parentDirectory;
            set => Set(ref _parentDirectory, value);
        }

        private long _size = 0;

        public long Size
        {
            get => _size;
            set => Set(ref _size, value);
        }

        private string _sizeString = string.Empty;

        public string SizeString
        {
            get => _sizeString;
            set => Set(ref _sizeString, value);
        }
    }
}
