using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MView.ViewModels.Dialogs
{
    public class InformationViewModel : Screen
    {
        public string VersionInformation { get; set; }

        public InformationViewModel()
        {
            string text = string.Empty;
            text += $"Application Version : 2.1\r\n";

            VersionInformation = text;
        }

        public void OnViewerUnloaded(FlowDocumentScrollViewer viewer)
        {
            if (viewer != null)
            {
                viewer.Document = null;
            }
        }
    }
}
