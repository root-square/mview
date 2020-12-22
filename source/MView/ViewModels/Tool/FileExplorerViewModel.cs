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

        public const string ToolContentId = "FileExplorer";

        private ObservableCollection<DirectoryItem> _nodes = new ObservableCollection<DirectoryItem>();
        private ObservableCollection<DirectoryItem> _selectedNodes = new ObservableCollection<DirectoryItem>();

        #endregion

        #region ::Constructors::

        public FileExplorerViewModel() : base("File Explorer")
        {
            ContentId = ToolContentId;
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

        #endregion
    }
}
