using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using MView.Utilities;
using MView.Utilities.Indexing;
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

        public async Task SetContentAsync(IndexedItem? item)
        {
            Item = item;
            //Image = new BitmapImage() { StreamSource = fileStream };
        }
    }
}
