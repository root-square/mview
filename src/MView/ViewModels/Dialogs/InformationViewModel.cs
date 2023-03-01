using Caliburn.Micro;
using MView.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MView.ViewModels.Dialogs
{
    public class InformationViewModel : Screen
    {
        public string VersionString { get; set; } = $"{VariableBuilder.GetProductVersion()}({VariableBuilder.GetFileVersion() ?? "dev"})";
    }
}
