using MView.Bases;
using MView.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MView.ViewModels.File
{
    public class ImageFileViewModel : FileViewModelBase
    {
        #region ::Fields::

        private SolidColorBrush _backgroundBrush = new SolidColorBrush(Color.FromArgb(0,0,0,0));

        private FileProperties _fileProperties = new FileProperties();

        private string _imageSizeString = string.Empty;

        #endregion

        #region ::Constructors::

        public ImageFileViewModel(string filePath) : base(filePath)
        {
            Initialize(filePath);
        }

        #endregion

        #region ::Properties::

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
            var task = Task.Run(() =>
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
                bi.EndInit();

                _fileProperties = new FileProperties(filePath);

                ImageSizeString = $"{(int)bi.Width} * {(int)bi.Height} px";
            });

            await task;
        }

        #endregion
    }
}
