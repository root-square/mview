using MView.Bases;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace MView.ViewModels.Tool
{
    public class ScriptManagerViewModel : ToolViewModelBase
    {
        #region ::Fields::

        public const string ToolContentId = "ScriptManager";

        private string _selectedItemsString = "Selected Items(0)";

        #endregion

        #region ::Constructors::

        public ScriptManagerViewModel() : base("Script Manager")
        {
            ContentId = ToolContentId;

            Workspace.Instance.FileExplorer.SelectedNodes.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
        }

        #endregion

        #region ::Properties::

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

        #endregion

        #region ::CollectionChanged Event Subscriber::

        private void OnCollectionChanged(object sender, EventArgs e)
        {
            SelectedItemsString = $"Selected Items({Workspace.Instance.FileExplorer.SelectedNodes.Count})";
        }

        #endregion
    }
}
