using MView.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MView.Converters
{
    /// <summary>
    /// Type to image converter.
    /// </summary>
    [ValueConversion(typeof(DirectoryItemType), typeof(ImageSource))]
    public class TypeToImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapImage bitmap = new BitmapImage();

            DirectoryItemType type = (DirectoryItemType)value;

            if (type == DirectoryItemType.BaseDirectory)
            {
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("pack://application:,,/Resources/icon_basedirectory.png");
                bitmap.EndInit();
            }
            else if (type == DirectoryItemType.Directory)
            {
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("pack://application:,,/Resources/icon_directory.png");
                bitmap.EndInit();
            }
            else
            {
                string path = (string)parameter;
                string extension = Path.GetExtension(path).ToLower();

                if (extension == ".ogg" || extension == ".m4a" || extension == ".wav")
                {
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("pack://application:,,/Resources/icon_audiofile.png");
                    bitmap.EndInit();
                }
                else if (extension == ".png")
                {
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("pack://application:,,/Resources/icon_image.png");
                    bitmap.EndInit();
                }
                else if (extension == ".rpgmvo" || extension == ".rpgmvm" || extension == ".rpgmvw" || extension == ".rpgmvp" || extension == ".ogg_" || extension == ".m4a_" || extension == ".wav_" || extension == ".png_")
                {
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("pack://application:,,/Resources/icon_encrypted.png");
                    bitmap.EndInit();
                }
                else if (extension == ".json" || extension == ".script")
                {
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("pack://application:,,/Resources/icon_code.png");
                    bitmap.EndInit();
                }
                else if (extension == ".rpgsave")
                {
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("pack://application:,,/Resources/icon_savedata.png");
                    bitmap.EndInit();
                }
                else
                {
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("pack://application:,,/Resources/icon_file.png");
                    bitmap.EndInit();
                }
            }

            return bitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
