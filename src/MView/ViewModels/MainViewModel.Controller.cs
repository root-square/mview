using Caliburn.Micro;
using MView.Utilities.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MView.Utilities;

namespace MView.ViewModels
{
    public partial class MainViewModel
    {
        public void ExploreEncryptionKey()
        {
            VistaOpenFileDialog dialog = new VistaOpenFileDialog();
            dialog.Title = LocalizationHelper.GetText("CONTROLLER_EXPLORE_ENCKEY_DIALOG_TITLE");
            dialog.Filter = LocalizationHelper.GetText("CONTROLLER_EXPLORE_ENCKEY_DIALOG_FILTER");
            dialog.Multiselect = false;
            dialog.CheckFileExists = true;

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    JObject system = JObject.Parse(TextManager.ReadTextFile(dialog.FileName, Encoding.UTF8));

                    if (system.ContainsKey("encryptionKey"))
                    {
                        Settings.EncryptionKey = system["encryptionKey"]!.ToString() ?? string.Empty;
                    }
                    else
                    {
                        MessageBox.Show(LocalizationHelper.GetText("CONTROLLER_ALERT_ENCKEY_NOT_FOUND"), "MView", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                catch (JsonReaderException)
                {
                    MessageBox.Show(LocalizationHelper.GetText("CONTROLLER_ALERT_CANNOT_OPEN_SYSTEM_JSON"), "MView", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        public void ExploreOutputPath()
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.Description = LocalizationHelper.GetText("CONTROLLER_EXPLORE_OUTPATH_DIALOG_TITLE");
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == true)
            {
                Settings.OutputPath = dialog.SelectedPath;
            }
        }
    }
}
