using MView.Bases;
using MView.Core.Cryptography;
using MView.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MView.ViewModels.File
{
    public class ImageFileViewModel : FileViewModelBase
    {
        #region ::Fields::

        private SolidColorBrush _backgroundBrush = new SolidColorBrush(Color.FromArgb(0,0,0,0));

        private FileProperties _fileProperties = new FileProperties();

        private string _imageFilePath = null;

        private string _imageSizeString = string.Empty;

        #endregion

        #region ::Constructors::

        public ImageFileViewModel(string filePath) : base(filePath)
        {
            Initialize(filePath);
        }

        #endregion

        #region ::Properties::

        public string ImageFilePath
        {
            get
            {
                return _imageFilePath;
            }
            set
            {
                _imageFilePath = value;
                RaisePropertyChanged();
            }
        }

        public SolidColorBrush BackgroundBrush
        {
            get
            {
                return _backgroundBrush;
            }
            set
            {
                _backgroundBrush = value;
                RaisePropertyChanged();
            }
        }

        public string FileSizeString
        {
            get
            {
                return _fileProperties.Size;
            }
        }

        public string ImageSizeString
        {
            get
            {
                return _imageSizeString;
            }
            set
            {
                _imageSizeString = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ::Methods::

        private async void Initialize(string filePath)
        {
            Workspace.Instance.SetStatus(TaskStatusType.Loading, $"Loading a file... ({filePath})");

            var task = Task.Run(() =>
            {
                try
                {
                    string extension = Path.GetExtension(filePath).ToLower();

                    if (extension == ".rpgmvp" || extension == ".png_")
                    {
                        if (!CryptographyProvider.VerifyFakeHeader(filePath))
                        {
                            throw new InvalidDataException("An invalid *.rpgmvp(*.png_) file was entered.");
                        }

                        string tempFilePath = Path.GetTempFileName();
                        CryptographyProvider.RestoreHeader(filePath, tempFilePath);
                        ImageFilePath = tempFilePath;
                    }
                    else
                    {
                        ImageFilePath = filePath;
                    }

                    _fileProperties = new FileProperties(filePath);

                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri(ImageFilePath, UriKind.RelativeOrAbsolute);
                    bi.EndInit();

                    ImageSizeString = $"{(int)bi.Width} * {(int)bi.Height} px";
                }
                catch (Exception ex)
                {
                    Workspace.Instance.Report.AddReportWithIdentifier($"{ex.Message}\r\n{ex.StackTrace}", ReportType.Warning);
                }
            });

            await task;

            Workspace.Instance.SetStatus(TaskStatusType.Completed, $"Completed.");
        }

        #endregion
    }
}
