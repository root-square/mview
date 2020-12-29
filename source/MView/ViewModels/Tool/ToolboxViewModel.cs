using MahApps.Metro.IconPacks;
using MView.Bases;
using MView.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media.Imaging;

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

        private enum ToolIconType
        {
            ResourceEncrypter,
            ResourceDecrypter,
            HashComparerer,
            TextComparerer,
            SaveDataManager,
            ScriptImporter,
            ScriptExporter,
            ScriptTranslator,
            ScriptMigrationManager
        }
        private void Initialize()
        {
            // Initialize tools.
            ToolboxCategory cryptography = new ToolboxCategory("Cryptography", true);
            cryptography.SubItems.Add(new ToolboxItem(PackIconMaterialKind.LockCheckOutline, "Resource Encrypter", null));
            cryptography.SubItems.Add(new ToolboxItem(PackIconMaterialKind.LockOpenCheckOutline, "Resource Decrypter", null));

            ToolboxCategory data = new ToolboxCategory("Data", true);
            data.SubItems.Add(new ToolboxItem(PackIconMaterialKind.FolderPoundOutline, "Hash Comparerer", null));
            data.SubItems.Add(new ToolboxItem(PackIconMaterialKind.TimelineTextOutline, "Text Comparerer", null));

            ToolboxCategory saveData = new ToolboxCategory("Save Data", true);
            saveData.SubItems.Add(new ToolboxItem(PackIconMaterialKind.ContentSaveEditOutline, "Save Data Manager", null));

            ToolboxCategory script = new ToolboxCategory("Script", true);
            script.SubItems.Add(new ToolboxItem(PackIconMaterialKind.FileImportOutline, "Script Importer", null));
            script.SubItems.Add(new ToolboxItem(PackIconMaterialKind.FileExportOutline, "Script Exporter", null));
            script.SubItems.Add(new ToolboxItem(PackIconMaterialKind.Translate, "Script Translator", null));
            script.SubItems.Add(new ToolboxItem(PackIconMaterialKind.SwapHorizontalVariant, "Script Migration Manager", null));

            _nodes.Add(cryptography);
            _nodes.Add(data);
            _nodes.Add(saveData);
            _nodes.Add(script);
        }

        #endregion
    }
}
