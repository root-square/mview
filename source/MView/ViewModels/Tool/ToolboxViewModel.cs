using MahApps.Metro.IconPacks;
using MView.Bases;
using MView.Entities;
using MView.Pages;
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
            cryptography.SubItems.Add(new ToolboxItem(PackIconMaterialKind.LockCheckOutline, "Resource Encrypter", nameof(ResourceEncrypterPage), true, 700, 900));
            cryptography.SubItems.Add(new ToolboxItem(PackIconMaterialKind.LockOpenCheckOutline, "Resource Decrypter", nameof(ResourceDecrypterPage), true, 700, 900));

            ToolboxCategory data = new ToolboxCategory("Data", true);
            data.SubItems.Add(new ToolboxItem(PackIconMaterialKind.FolderPoundOutline, "Hash Comparer", nameof(HashComparerPage), false, 550, 210));
            data.SubItems.Add(new ToolboxItem(PackIconMaterialKind.TimelineTextOutline, "Text Comparer", nameof(TextComparerPage), false, 550, 210));

            ToolboxCategory saveData = new ToolboxCategory("Save Data", true);
            saveData.SubItems.Add(new ToolboxItem(PackIconMaterialKind.ContentSaveEditOutline, "Save Data Manager", nameof(SaveDataManagerPage), true));

            ToolboxCategory script = new ToolboxCategory("Script", true);
            script.SubItems.Add(new ToolboxItem(PackIconMaterialKind.FileImportOutline, "Script Importer", nameof(ScriptImporterPage), true, 700, 840));
            script.SubItems.Add(new ToolboxItem(PackIconMaterialKind.FileExportOutline, "Script Exporter", nameof(ScriptExporterPage), true, 700, 840));
            script.SubItems.Add(new ToolboxItem(PackIconMaterialKind.Translate, "Script Translator", nameof(ScriptTranslatorPage), true, 700, 860));
            script.SubItems.Add(new ToolboxItem(PackIconMaterialKind.SwapHorizontalVariant, "Script Migration Manager", nameof(ScriptMigrationManagerPage), false));

            _nodes.Add(cryptography);
            _nodes.Add(data);
            _nodes.Add(saveData);
            _nodes.Add(script);
        }

        #endregion
    }
}
