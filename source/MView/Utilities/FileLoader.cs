using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MView.Utilities
{
    public static class FileLoader
    {
        public static byte[] LoadFile(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }

        public static MemoryStream LoadFileToStream(string filePath)
        {
            return new MemoryStream(File.ReadAllBytes(filePath));
        }

        public static ImageSource LoadImage(string filePath)
        {
            using (MemoryStream stream = LoadFileToStream(filePath))
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = stream;
                img.EndInit();

                return img;
            }
        }

        public static ImageSource LoadImageFromStream(Stream stream)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.StreamSource = stream;
            img.EndInit();

            return img;
        }
    }
}
