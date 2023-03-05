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
using TagLib.IFD;

namespace MView.ViewModels
{
    public partial class MainViewModel
    {
        private INavigationService? _viewerNavigationService;

        private FileStream? _hexViewerStream = null;

        public FileStream? HexViewerStream
        {
            get => _hexViewerStream;
            set => Set(ref _hexViewerStream, value);
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
                    await IoC.Get<CodeViewerViewModel>().SetContentAsync(_selectedItem);
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
                    await IoC.Get<ImageViewerViewModel>().SetContentAsync(_selectedItem);
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

                    // Check and distribute the file.
                    string extension = Path.GetExtension(_selectedItem.FullPath).ToLower();

                    switch (extension)
                    {
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
                        case ".jpg":
                        case ".gif":
                        case ".png":
                        case ".rpgmvp":
                        case ".png_":
                            await ShowImageViewerAsync();
                            break;
                        default:
                            Log.Warning("This format does not support preview.");
                            await ShowAlertAsync(PackIconKind.Alert, LocalizationHelper.GetText("VIEWER_ALERT_PREVIEW_UNSUPPORTED"));
                            break;
                    }

                    await RefreshHexViewerAsync();
                    await RefreshMetadataAsync();
                }
                catch (Exception ex)
                {
                    switch (ex)
                    {
                        case InvalidOperationException invalidOpsEx:
                            Log.Warning(invalidOpsEx, "The file is too large to load.");
                            await ShowAlertAsync(PackIconKind.Alert, LocalizationHelper.GetText("VIEWER_ALERT_FILE_IS_TOO_LARGE"));
                            break;
                        default:
                            Log.Warning(ex, "An unknown exception has occurred.");
                            await ShowAlertAsync(PackIconKind.Alert, LocalizationHelper.GetText("VIEWER_ALERT_UNKNOWN_EXCEPTION"));
                            break;
                    }
                }
            });

            await task;
        }

        public async Task RefreshHexViewerAsync()
        {
            // Dispose the old stream.
            if (_hexViewerStream != null && _hexViewerStream?.CanRead == true)
            {
                await _hexViewerStream.DisposeAsync();
            }

            HexViewerStream = new FileStream(_selectedItem?.FullPath!, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public async Task RefreshMetadataAsync()
        {

        }
    }
}
