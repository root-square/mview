using Caliburn.Micro;
using MView.Utilities;
using Ookii.Dialogs.Wpf;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MView.ViewModels.Dialogs
{
    public class RestorerViewModel : Screen
    {
        private string _outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public string OutputDirectory
        {
            get => _outputDirectory;
            set => Set(ref _outputDirectory, value);
        }

        private bool _verifyFakeHeader = true;

        public bool VerifyFakeHeader
        {
            get => _verifyFakeHeader;
            set => Set(ref _verifyFakeHeader, value);
        }

        public void ExploreOutputPath()
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Please select a folder.";
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.

            if (dialog.ShowDialog() == true)
            {
                OutputDirectory = dialog.SelectedPath;
            }
        }

        public async void Cancel()
        {
            await TryCloseAsync(false);
        }

        public async void Confirm(Window window)
        {
            if (window == null)
            {
                Log.Warning("The dialog shouldn't be null.");
                return;
            }

            if (!ValidationHelper.IsValid(window))
            {
                MessageBox.Show("Invalid value entered.", "MView", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Log.Warning("Invalid value entered.");
                return;
            }

            await TryCloseAsync(true);
        }
    }
}
