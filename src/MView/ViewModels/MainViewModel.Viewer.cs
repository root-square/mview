using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using MView.Utilities;
using MView.ViewModels.Pages;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace MView.ViewModels
{
    public partial class MainViewModel
    {
        private INavigationService? _viewerNavigationService;

        private MemoryStream _sourceFileStream = new MemoryStream();

        public MemoryStream SourceFileStream
        {
            get => _sourceFileStream;
            set => Set(ref _sourceFileStream, value);
        }

        private MemoryStream _restoredFileStream = new MemoryStream();

        public MemoryStream RestoredFileStream
        {
            get => _restoredFileStream;
            set => Set(ref _restoredFileStream, value);
        }

        public void RegisterFrame(Frame frame)
        {
            _viewerNavigationService = new FrameAdapter(frame);

            _container.Instance(_viewerNavigationService);
        }

        private async Task ShowAlertAsync(PackIconKind icon, string text)
        {
            await Application.Current.Dispatcher.InvokeAsync(delegate
            {
                IoC.Get<AlertViewModel>().SetContent(_selectedItem, icon, text);
                _viewerNavigationService?.NavigateToViewModel(typeof(AlertViewModel));
            }, DispatcherPriority.Normal);
        }

        private async Task ShowAudioPlayerAsync()
        {
            Exception? exception = null;

            await Application.Current.Dispatcher.InvokeAsync(async delegate
            {
                try
                {
                    IoC.Get<AudioPlayerViewModel>().Set(_selectedItem);
                    _viewerNavigationService?.NavigateToViewModel(typeof(AudioPlayerViewModel));
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }, DispatcherPriority.Normal);

            if (exception != null)
            {
                throw exception;
            }
        }

        private async Task ShowCodeViewerAsync()
        {
            Exception? exception = null;

            await Application.Current.Dispatcher.InvokeAsync(async delegate
            {
                try
                {
                    await IoC.Get<CodeViewerViewModel>().SetDocumentAsync(_selectedItem);
                    _viewerNavigationService?.NavigateToViewModel(typeof(CodeViewerViewModel));
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }, DispatcherPriority.Normal);

            if (exception != null)
            {
                throw exception;
            }
        }

        private async Task ShowImageViewerAsync()
        {
            Exception? exception = null;

            await Application.Current.Dispatcher.InvokeAsync(async delegate
            {
                try
                {
                    IoC.Get<ImageViewerViewModel>().Set(_selectedItem);
                    _viewerNavigationService?.NavigateToViewModel(typeof(ImageViewerViewModel));
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }, DispatcherPriority.Normal);

            if (exception != null)
            {
                throw exception;
            }
        }

        public async Task RefreshViewerAsync()
        {
            var task = Task.Run(async () =>
            {
                try
                {
                    if (_selectedItem == null || !File.Exists(_selectedItem.FullPath))
                    {
                        await ShowAlertAsync(PackIconKind.Alert, LocalizationHelper.GetText("VIEWER_ALERT_FILE_NOT_FOUND"));
                        return;
                    }

                    string extension = Path.GetExtension(_selectedItem.FullPath).ToLower();

                    switch (extension)
                    {
                        case ".jpg":
                        case ".gif":
                        case ".png":
                        case ".rpgmvp":
                        case ".png_":
                            await ShowImageViewerAsync();
                            break;
                        case ".ogg":
                        case ".rpgmvo":
                        case ".ogg_":
                        case ".m4a":
                        case ".rpgmvm":
                        case ".m4a_":
                        case ".wav":
                        case ".rpgmvw":
                        case ".wav_":
                            await ShowAudioPlayerAsync();
                            break;
                        case ".txt":
                        case ".bat":
                        case ".ps1":
                        case ".html":
                        case ".css":
                        case ".js":
                        case ".py":
                        case ".rb":
                        case ".ini":
                        case ".xml":
                        case ".json":
                            await ShowCodeViewerAsync();
                            break;
                        default:
                            await ShowAlertAsync(PackIconKind.Alert, LocalizationHelper.GetText("VIEWER_ALERT_PREVIEW_UNSUPPORTED"));
                            break;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Log.Warning(ex, "The file is too large to load.");
                    await ShowAlertAsync(PackIconKind.Alert, LocalizationHelper.GetText("VIEWER_ALERT_FILE_IS_TOO_LARGE"));
                    return;
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "An unknown exception has occurred.");
                    await ShowAlertAsync(PackIconKind.Alert, LocalizationHelper.GetText("VIEWER_ALERT_UNKNOWN_EXCEPTION"));
                    return;
                }
            });

            await task;
        }
    }
}
