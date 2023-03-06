using Caliburn.Micro;
using HL.Manager;
using MaterialDesignThemes.Wpf;
using MView.Utilities;
using MView.Utilities.Indexing;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MView.ViewModels.Pages
{
    public class ImageViewerViewModel : Screen
    {
        private IndexedItem? _item = null;

        public IndexedItem? Item
        {
            get => _item;
            set => Set(ref _item, value);
        }

        private BitmapImage _image = new BitmapImage();

        public BitmapImage Image
        {
            get => _image;
            set => Set(ref _image, value);
        }

        private double _scale = 1.0;

        public double Scale
        {
            get => _scale;
            set
            {
                Set(ref _scale, value);
                ScaleSizes();
            }
        }

        private double _gridWidth = 0;

        public double GridWidth
        {
            get => _gridWidth;
            set => Set(ref _gridWidth, value);
        }

        private double _gridHeight = 0;

        public double GridHeight
        {
            get => _gridHeight;
            set => Set(ref _gridHeight, value);
        }

        private double _scaledWidth = 0;

        public double ScaledWidth
        {
            get => _scaledWidth;
            set => Set(ref _scaledWidth, value);
        }

        private double _scaledHeight = 0;

        public double ScaledHeight
        {
            get => _scaledHeight;
            set => Set(ref _scaledHeight, value);
        }

        private void ScaleSizes()
        {
            ScaledWidth = _image.PixelWidth * _scale;
            ScaledHeight = _image.PixelHeight * _scale;

            var gridWidth = ScaledWidth * 1.6;
            var gridHeight = ScaledHeight * 1.6;

            if (GridWidth == gridWidth)
            {
                gridWidth += 2;
            }

            if (GridHeight == gridHeight)
            {
                gridHeight += 2;
            }

            GridWidth = gridWidth;
            GridHeight = gridHeight;
        }

        public async Task SetContentAsync(IndexedItem? item)
        {
            // Note: If the file is too large, it will not be loaded.
            if (item?.Size > 10485760) // 10 MB
            {
                throw new InvalidOperationException("The file is too large to load.");
            }

            Item = item;

            string extension = Path.GetExtension(item?.FullPath!).ToLower();

            switch (extension)
            {
                case ".jpg":
                case ".gif":
                case ".png":
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                        image.UriSource = new Uri(item?.FullPath!);
                        image.EndInit();
                        Image = image;
                        break;
                    }
                case ".rpgmvp":
                case ".png_":
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                        image.StreamSource = await CryptographyProvider.RestoreAndGetAsync(item?.FullPath!);
                        image.EndInit();
                        Image = image;
                        break;
                    }
            }

            ScaleSizes();
        }

        public void DownSize()
        {
            double step = 0.1;

            if (_scale - step >= 0.1)
            {
                Scale -= step;
            }
            else
            {
                Scale = 0.1;
            }
        }

        public void UpSize()
        {
            double step = 0.1;

            if (_scale + step <= 2.0)
            {
                Scale += step;
            }
            else
            {
                Scale = 2.0;
            }
        }

        public void RestoreSize()
        {
            Scale = 1.0;
        }
    }
}
