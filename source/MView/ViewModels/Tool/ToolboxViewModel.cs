using MView.Bases;
using MView.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MView.ViewModels.Tool
{
    public class ToolboxViewModel : ToolViewModelBase
    {
        #region ::Fields::

        public const string ToolContentId = "Toolbox";

        private ObservableCollection<ToolboxCategory> _nodes = new ObservableCollection<ToolboxCategory>();

        #endregion

        #region ::Constructors::

        public ToolboxViewModel() : base("Toolbox")
        {
            ContentId = ToolContentId;
            Initialize();
        }

        #endregion

        #region ::Properties::

        public ObservableCollection<ToolboxCategory> Nodes
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

        #endregion

        #region ::Methods::

        private void Initialize()
        {
            ToolboxCategory cryptography = new ToolboxCategory("Cryptography", true);
            cryptography.SubItems.Add(new ToolboxItem("Resource Encrypter", null, false));
            cryptography.SubItems.Add(new ToolboxItem("Resource Decrypter", null, false));

            ToolboxCategory data = new ToolboxCategory("Data", true);
            data.SubItems.Add(new ToolboxItem("Hash Comparerer", null, false));
            data.SubItems.Add(new ToolboxItem("Text Comparerer", null, false));

            ToolboxCategory saveData = new ToolboxCategory("Save Data", true);
            saveData.SubItems.Add(new ToolboxItem("Save Data Manager", null, false));

            ToolboxCategory script = new ToolboxCategory("Script", true);
            script.SubItems.Add(new ToolboxItem("Script Importer", null, false));
            script.SubItems.Add(new ToolboxItem("Script Exporter", null, false));
            script.SubItems.Add(new ToolboxItem("Script Translator", null, false));
            script.SubItems.Add(new ToolboxItem("Script Migration Manager", null, false));

            _nodes.Add(cryptography);
            _nodes.Add(data);
            _nodes.Add(saveData);
            _nodes.Add(script);
        }

        #endregion
    }
}
