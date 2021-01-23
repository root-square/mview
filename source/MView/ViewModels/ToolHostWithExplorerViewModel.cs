using MView.Bases;
using MView.Commands;
using MView.Core;
using MView.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace MView.ViewModels
{
    public class ToolHostWithExplorerViewModel : ViewModelBase
    {
        #region ::Singleton Members::

        [NonSerialized]
        private static ToolHostWithExplorerViewModel _instance;

        public static ToolHostWithExplorerViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ToolHostWithExplorerViewModel();
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        #endregion

        #region ::Fields::

        private string _title = string.Empty;
        private double _width = 0;
        private double _height = 0;

        private Page _toolPage = null;

        private ObservableCollection<DirectoryItem> _items = new ObservableCollection<DirectoryItem>();
        private ObservableCollection<DirectoryItem> _selectedItems = new ObservableCollection<DirectoryItem>();

        private string _selectedItemsString = string.Format("Selected Items({0})", 0);

        private bool _isUseCurrentFile = true;

        private ICommand _refreshCommand;

        #endregion

        #region ::Constructors::

        public ToolHostWithExplorerViewModel()
        {
            _title = "Tool Host Window";
            _width = 300;
            _height = 300;

            // Copy collections.
            List<DirectoryItem> items = Workspace.Instance.FileExplorer.ResetAllItems(Workspace.Instance.FileExplorer.Items);
            _items = new ObservableCollection<DirectoryItem>(items);

            SelectedItems.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
        }

        public ToolHostWithExplorerViewModel(Page toolPage, double width = 600, double height = 700)
        {
            if (!string.IsNullOrEmpty(toolPage.Title))
            {
                _title = toolPage.Title;
            }

            _width = width;
            _height = height;

            _toolPage = toolPage;

            // Copy collections.
            List<DirectoryItem> items = Workspace.Instance.FileExplorer.ResetAllItems(Workspace.Instance.FileExplorer.Items);
            _items = new ObservableCollection<DirectoryItem>(items);

            SelectedItems.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
        }

        #endregion

        #region ::Properties::

        public string Title
        {
            get
            {
                return _title;
            }
        }

        public double Width
        {
            get
            {
                return _width;
            }
        }

        public double Height
        {
            get
            {
                return _height;
            }
        }

        public Page ToolPage
        {
            get
            {
                return _toolPage;
            }
        }

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

        public ObservableCollection<DirectoryItem> SelectedItems
        {
            get
            {
                return _selectedItems;
            }
            set
            {
                _selectedItems = value;
                RaisePropertyChanged();
            }
        }

        public string SelectedItemsString
        {
            get
            {
                return _selectedItemsString;
            }
            set
            {
                _selectedItemsString = value;
                RaisePropertyChanged();
            }
        }

        public bool IsUseCurrentFile
        {
            get
            {
                return _isUseCurrentFile;
            }
            set
            {
                _isUseCurrentFile = value;
                RaisePropertyChanged();
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return (_refreshCommand) ?? (_refreshCommand = new DelegateCommand(OnRefresh));
            }
        }

        #endregion

        #region ::Command Actions::

        private void OnRefresh()
        {
            Items = new ObservableCollection<DirectoryItem>(Workspace.Instance.FileExplorer.RefreshItems(_items));
            SelectedItems = new ObservableCollection<DirectoryItem>();
        }

        #endregion

        #region ::CollectionChanged Event Subscriber::

        private void OnCollectionChanged(object sender, EventArgs e)
        {
            SelectedItemsString = string.Format("Selected Items({0})", SelectedItems.Count);
        }

        #endregion
    }
}
