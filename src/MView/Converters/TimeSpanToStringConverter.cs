using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MView.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(string))]
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "NULL";
            }

            TimeSpan time = (TimeSpan)value;

            if (time.Days != 0)
            {
                return $"{time.Days:D2}:{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
            }
            else if (time.Hours != 0)
            {
                return $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
            }
            else
            {
                return $"{time.Minutes:D2}:{time.Seconds:D2}";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
