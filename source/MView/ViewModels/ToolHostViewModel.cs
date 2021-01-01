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
    public class ToolHostViewModel : ViewModelBase
    {
        #region ::Singleton Members::

        [NonSerialized]
        private static ToolHostViewModel _instance;

        public static ToolHostViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ToolHostViewModel();
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

        private string _title = "Tool Host";

        private Page _toolPage = null;

        private ObservableCollection<DirectoryItem> _nodes = new ObservableCollection<DirectoryItem>();
        private ObservableCollection<DirectoryItem> _selectedNodes = new ObservableCollection<DirectoryItem>();

        private string _selectedItemsString = "Selected Items(0)";

        private bool _isUseCurrentFile = true;

        private ICommand _refreshCommand;

        #endregion

        #region ::Constructors::

        public ToolHostViewModel()
        {
            _nodes = Workspace.Instance.FileExplorer.Nodes;
            _selectedNodes = Workspace.Instance.FileExplorer.SelectedNodes;

            SelectedNodes.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
        }

        public ToolHostViewModel(Page toolPage)
        {
            if (!string.IsNullOrEmpty(toolPage.Title))
            {
                _title = toolPage.Title; // TODO : 페이지 타이틀로 고치기.
            }

            _toolPage = toolPage;
            _nodes = Workspace.Instance.FileExplorer.Nodes;
            _selectedNodes = Workspace.Instance.FileExplorer.SelectedNodes;

            SelectedNodes.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
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

        public Page ToolPage
        {
            get
            {
                return _toolPage;
            }
        }

        public ObservableCollection<DirectoryItem> Nodes
        {
            get
            {
                return _nodes;
            }
            set
            {
                _nodes = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<DirectoryItem> SelectedNodes
        {
            get
            {
                return _selectedNodes;
            }
            set
            {
                _selectedNodes = value;
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

        #region ::Methods::

        #endregion

        #region ::Command Actions::

        private void OnRefresh()
        {
            Nodes = new ObservableCollection<DirectoryItem>(Workspace.Instance.FileExplorer.RefreshNodes(_nodes));
            SelectedNodes = new ObservableCollection<DirectoryItem>();
        }

        #endregion

        #region ::CollectionChanged Event Subscriber::

        private void OnCollectionChanged(object sender, EventArgs e)
        {
            SelectedItemsString = $"Selected Items({SelectedNodes.Count})";
        }

        #endregion
    }
}
