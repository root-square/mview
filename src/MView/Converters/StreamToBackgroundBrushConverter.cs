using Caliburn.Micro;
using MView.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MView.Converters
{
    [ValueConversion(typeof(Stream), typeof(Brush))]
    public class StreamToBackgroundBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!IoC.Get<Settings>().UseAdaptiveBackgroundColor)
            {
                return new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            }

            Stream stream = (Stream)value;

            if (stream != null && stream != Stream.Null)
            {
                try
                {
                    // Get an average color.
                    byte red;
                    byte green;
                    byte blue;
                    ColoringHelper.GetAverageColor(stream, out red, out green, out blue);

                    // Convert a color to HSV model.
                    double colorHue;
                    double colorSaturation;
                    double colorValue;
                    ColoringHelper.RGBToHSV(red, green, blue, out colorHue, out colorSaturation, out colorValue);

                    // Return a brush.
                    Color color = Color.FromRgb(0, 0, 0);

                    if (colorSaturation <= 0.3)
                    {
                        color = Color.FromRgb(0, 0, 0);
                    }

                    if (colorValue <= 0.3)
                    {
                        color = Color.FromRgb(255, 255, 255);
                    }

                    if (colorSaturation <= 0.3 && colorValue <= 0.3)
                    {
                        color = Color.FromRgb(0, 0, 0);
                    }

                    return new SolidColorBrush(color);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An unknown error occurred while calculating the background color brightness.");
                    return new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }
            }
            else
            {
                return new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
