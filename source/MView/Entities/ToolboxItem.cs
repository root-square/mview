using MahApps.Metro.IconPacks;
using MView.Bases;
using MView.Commands;
using MView.Pages;
using MView.Windows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MView.Entities
{
    public class ToolboxItem : ViewModelBase
    {
        #region ::Fields::

        private double _width = 0;
        private double _height = 0;

        private string _pageName;
        private bool _isUseExplorer = true;

        private ICommand _itemDoubleClickCommand;

        #endregion

        #region ::Constructors::

        public ToolboxItem(PackIconMaterialKind icon, string name, string pageName, bool isUseExplorer = true, double width = 600, double height = 700)
        {
            Icon = icon;
            Name = name;

            _width = width;
            _height = height;
            _pageName = pageName;
            _isUseExplorer = isUseExplorer;
        }

        #endregion

        #region ::Properties::

        public PackIconMaterialKind Icon { get; set; }

        public string Name { get; set; }

        public bool IsExpanded
        {
            get
            {
                return false;
            }
        }

        public ICommand ItemDoubleClickCommand
        {
            get
            {
                return (_itemDoubleClickCommand) ?? (_itemDoubleClickCommand = new DelegateCommand(ItemDoubleClick));
            }
        }

        #endregion

        #region ::Methods::

        #endregion

        #region ::Command Actions::

        public void ItemDoubleClick()
        {
            if (_pageName != null)
            {
                Workspace.Instance.Report.AddReportWithIdentifier("ToolboxItem is clicked. Requests ToolHost execution.", ReportType.Information);

                // Initialize tool page.
                Page page;
                switch (_pageName)
                {
                    case nameof(ResourceEncrypterPage):
                        page = new ResourceEncrypterPage();
                        break;
                    case nameof(ResourceDecrypterPage):
                        page = new ResourceDecrypterPage();
                        break;
                    case nameof(HashComparererPage):
                        page = new HashComparererPage();
                        break;
                    case nameof(TextComparererPage):
                        page = new TextComparererPage();
                        break;
                    case nameof(SaveDataManagerPage):
                        page = new SaveDataManagerPage();
                        break;
                    case nameof(ScriptImporterPage):
                        page = new ScriptImporterPage();
                        break;
                    case nameof(ScriptExporterPage):
                        page = new ScriptExporterPage();
                        break;
                    case nameof(ScriptTranslatorPage):
                        page = new ScriptTranslatorPage();
                        break;
                    case nameof(ScriptMigrationManagerPage):
                        page = new ScriptMigrationManagerPage();
                        break;
                    default:
                        page = new Page();
                        break;
                }

                // Show tool page with tool host.
                if (_isUseExplorer)
                {
                    Window window = new ToolHostWithExplorerWindow(page, _width, _height);
                    window.ShowDialog();
                }
                else
                {
                    Window window = new ToolHostWindow(page, _width, _height);
                    window.ShowDialog();
                }
            }
            else
            {
                Workspace.Instance.Report.AddReportWithIdentifier("ToolboxItem is clicked. But it's value is null.", ReportType.Caution);
            }
        }

        #endregion
    }
}
