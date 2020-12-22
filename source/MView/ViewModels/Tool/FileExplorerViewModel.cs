using MView.Bases;
using MView.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace MView.ViewModels.Tool
{
    public class FileExplorerViewModel : ToolViewModelBase
    {
        #region ::Fields::

        public event EventHandler SelectedItemChanged;

        public const string ToolContentId = "FileExplorer";

        private ObservableCollection<DirectoryItem> _items = new ObservableCollection<DirectoryItem>();
        private DirectoryItem _selectedItem;

        #endregion

        #region ::Constructors::

        public FileExplorerViewModel() : base("File Explorer")
        {
            ContentId = ToolContentId;

            Items.Add(new DirectoryItem(new DirectoryInfo(@"E:\Translate\Game\검은 약속"), true, true));
        }

        #endregion

        #region ::Properties::

        public ObservableCollection<DirectoryItem> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                RaisePropertyChanged();
            }
        }

        public DirectoryItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged();

                if (SelectedItemChanged != null)
                    SelectedItemChanged(this, EventArgs.Empty);
            }
        }

        #endregion

        #region ::Methods::

        #endregion
    }
}
