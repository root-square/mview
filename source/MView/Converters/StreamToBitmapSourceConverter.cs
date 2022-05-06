using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MView.Converters
{
    [ValueConversion(typeof(Stream), typeof(BitmapSource))]
    public class StreamToBitmapSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Stream stream = (Stream)value;

            if (stream != null && stream != Stream.Null)
            {
                try
                {
                    stream.Position = 0;
                    return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                }
                catch (NotSupportedException ex)
                {
                    Log.Error(ex, "The file is not in the correct image format.");
                    return new BitmapImage();
                }
            }
            else
            {
                return new BitmapImage();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
