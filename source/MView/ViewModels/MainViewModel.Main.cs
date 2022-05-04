using Caliburn.Micro;
using MView.Utilities;
using MView.Utilities.Indexing;
using MView.Utilities.Text;
using MView.ViewModels.Dialogs;
using Ookii.Dialogs.Wpf;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MView.ViewModels
{
    public partial class MainViewModel : Screen
    {
        #region ::Variables::

        private Settings _settings = IoC.Get<Settings>();

        public Settings Settings
        {
            get => _settings;
            set => Set(ref _settings, value);
        }

        private string _statusText = "Status";

        public string StatusText
        {
            get => _statusText;
            set => Set(ref _statusText, value);
        }

        #endregion

        #region ::Constructors::

        public MainViewModel()
        {
            ConnectToLogBroker();
        }

        private void ConnectToLogBroker()
        {
            App.LogBroker.LogEmittedEvent += (log) =>
            {
                StatusText = log;
            };

            Log.Information("The Main VM has established a connection to the Log Broker.");
        }

        #endregion

        #region ::Menu Interactions::

        // Task
        public async void Encrypt()
        {
            EncryptorViewModel viewModel = IoC.Get<EncryptorViewModel>();
            bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

            if (dialogResult == true)
            {

            }
        }

        public async void Decrypt()
        {
            DecrypterViewModel viewModel = IoC.Get<DecrypterViewModel>();
            bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

            if (dialogResult == true)
            {

            }
        }

        public async void Estimate()
        {
            
        }

        public async void Restore()
        {
            RestorerViewModel viewModel = IoC.Get<RestorerViewModel>();
            bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

            if (dialogResult == true)
            {

            }
        }

        // App

        public async void PickNoT()
        {
            var settings = IoC.Get<Settings>();
            int initialValue = settings.NumberOfThreads;

            NoTPickerViewModel viewModel = IoC.Get<NoTPickerViewModel>();
            viewModel.NumberOfThreads = initialValue;

            bool? dialogResult = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);

            if (dialogResult == true)
            {
                if (viewModel.NumberOfThreads <= 0 || viewModel.NumberOfThreads > 10)
                {
                    settings.NumberOfThreads = initialValue;
                    MessageBox.Show("Out of range.", "MView", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                settings.NumberOfThreads = viewModel.NumberOfThreads;
                MessageBox.Show($"The number of threads is set to {viewModel.NumberOfThreads}.", "MView", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public async void OpenInformation()
        {
            InformationViewModel viewModel = IoC.Get<InformationViewModel>();
            await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel).ConfigureAwait(false);
        }

        #endregion
    }
}
