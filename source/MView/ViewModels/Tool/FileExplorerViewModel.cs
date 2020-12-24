using MView.Bases;
using MView.Commands;
using MView.Core;
using MView.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MView.ViewModels.Tool
{
    public class FileExplorerViewModel : ToolViewModelBase
    {
        #region ::Fields::

        public const string ToolContentId = "FileExplorer";

        private ObservableCollection<DirectoryItem> _nodes = new ObservableCollection<DirectoryItem>();
        private ObservableCollection<DirectoryItem> _selectedNodes = new ObservableCollection<DirectoryItem>();

        #endregion

        #region ::Constructors::

        public FileExplorerViewModel() : base("File Explorer")
        {
            ContentId = ToolContentId;

            Nodes.Add(new DirectoryItem(new DirectoryInfo(@"E:\MVTest"), true, true));
        }

        #endregion

        #region ::Properties::

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

        #endregion

        #region ::Methods::

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
                foreach(DirectoryItem item in directories)
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

        #endregion
    }
}
