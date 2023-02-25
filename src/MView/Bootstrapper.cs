using Caliburn.Micro;
using MView.ViewModels;
using MView.ViewModels.Dialogs;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MView
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container.Instance(_container);

            // Singletons
            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<Settings>();

            // ViewModels
            _container
                .Singleton<MainViewModel>()
                .PerRequest<NoTPickerViewModel>()
                .Singleton<InformationViewModel>()
                .PerRequest<EncryptorViewModel>()
                .PerRequest<DecrypterViewModel>()
                .PerRequest<RestorerViewModel>();
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            // Display the MainViewModel.
            await DisplayRootViewForAsync(typeof(MainViewModel));
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs ex)
        {
            MessageBox.Show(ex.Exception.Message, "An unknown error has occurred", MessageBoxButton.OK, MessageBoxImage.Error);

            Log.Fatal(ex.Exception, "An unknown error has occurred.");
        }
    }
}
