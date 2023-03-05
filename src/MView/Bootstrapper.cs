using Caliburn.Micro;
using MView.ViewModels;
using MView.ViewModels.Dialogs;
using MView.ViewModels.Pages;
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

            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<Settings>()
                .Singleton<MainViewModel>()
                .Singleton<AlertViewModel>()
                .Singleton<AudioPlayerViewModel>()
                .Singleton<CodeViewerViewModel>()
                .Singleton<ImageViewerViewModel>()
                .PerRequest<NoTPickerViewModel>()
                .PerRequest<InformationViewModel>();
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
            Log.Fatal(ex.Exception, "An unhandled exception has occurred.");
            Log.CloseAndFlush();

            MessageBox.Show("An unhandled exception has occurred.\r\n" + ex.Exception.Message, "MView", MessageBoxButton.OK, MessageBoxImage.Error);
            
            Environment.Exit(1);
        }
    }
}
