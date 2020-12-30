using MView.Bases;
using MView.Core;
using MView.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Windows.Controls;

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

        private string _title = "ToolHost";

        private Page _toolPage = null;

        private ObservableCollection<DirectoryItem> _nodes = new ObservableCollection<DirectoryItem>();
        private ObservableCollection<DirectoryItem> _selectedNodes = new ObservableCollection<DirectoryItem>();

        private string _selectedItemsString = "Selected Items(0)";

        private bool _isUseCurrentFile = true;

        #endregion

        #region ::Constructors::

        public ToolHostViewModel()
        {
            _nodes = Workspace.Instance.FileExplorer.Nodes;
            _selectedNodes = Workspace.Instance.FileExplorer.SelectedNodes;

            SelectedNodes.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
        }

        public ToolHostViewModel(Page toolPage, string name = null)
        {
            if (!string.IsNullOrEmpty(name))
            {
                _title = name;
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
            set
            {
                _title = value;
                RaisePropertyChanged();
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

        #endregion

        #region ::Methods::

        private bool IsBaseOf(string target, string candidate)
        {
            target = Path.GetFullPath(target);
            candidate = Path.GetFullPath(candidate);

            if (target.Equals(candidate, StringComparison.OrdinalIgnoreCase) || target.StartsWith(candidate + "\\", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<string> GetSelectedFiles(List<string> extensions = null)
        {
            // Get directories and files.
            List<string> result = new List<string>();
            List<DirectoryItem> directories = new List<DirectoryItem>();
            List<DirectoryItem> files = new List<DirectoryItem>();

            foreach (DirectoryItem item in _selectedNodes)
            {
                if (item.Type == DirectoryItemType.BaseDirectory || item.Type == DirectoryItemType.Directory)
                {
                    directories.Add(item);
                }
                else
                {
                    files.Add(item);
                }
            }

            // Ignore duplicated directories.
            for (int i = 0; i < directories.Count; i++)
            {
                foreach (DirectoryItem item in directories)
                {
                    if (IsBaseOf(directories[i].FullName, item.FullName) && directories.IndexOf(directories[i]) != i)
                    {
                        directories.RemoveAt(i);
                        break;
                    }
                }
            }

            // Ignore duplicated files.
            for (int i = 0; i < files.Count; i++)
            {
                foreach (DirectoryItem item in directories)
                {
                    if (IsBaseOf(files[i].FullName, item.FullName) && files.IndexOf(files[i]) != i)
                    {
                        files.RemoveAt(i);
                        break;
                    }
                }
            }

            // Get subitems.
            foreach (DirectoryItem item in directories)
            {
                result.AddRange(FileManager.GetFiles(item.FullName, extensions));
            }

            foreach (DirectoryItem item in files)
            {
                result.Add(item.FullName);
            }

            return result;
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
