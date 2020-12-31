using MahApps.Metro.IconPacks;
using MView.Bases;
using MView.Commands;
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

        private Page _toolPage;

        private ICommand _itemDoubleClickCommand;

        #endregion

        #region ::Constructors::

        public ToolboxItem(PackIconMaterialKind icon, string name, Page toolPage = null)
        {
            Icon = icon;
            Name = name;
            _toolPage = toolPage;
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
            if (_toolPage != null)
            {
                Workspace.Instance.Report.AddReportWithIdentifier("ToolboxItem is clicked. Request ToolHost execution.", ReportType.Information);

                Window window = new ToolHostWindow(_toolPage);
                window.ShowDialog();
            }
            else
            {
                Workspace.Instance.Report.AddReportWithIdentifier("ToolboxItem is clicked. But it's value is null.", ReportType.Caution);
            }
        }

        #endregion
    }
}
