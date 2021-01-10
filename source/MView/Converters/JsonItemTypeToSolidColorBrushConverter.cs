using MView.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MView.Converters
{
    /// <summary>
    /// TaskStatus to SolidColorBrush converter.
    /// </summary>
    [ValueConversion(typeof(JsonItemType), typeof(SolidColorBrush))]
    public class JsonItemTypeToSolidColorBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            JsonItemType type = (JsonItemType)value;

            if (type == JsonItemType.Object)
            {
                return new SolidColorBrush(Color.FromRgb(31, 58, 147));
            }
            else if (type == JsonItemType.Array)
            {
                return new SolidColorBrush(Color.FromRgb(31, 58, 147));
            }
            else if (type == JsonItemType.String)
            {
                return new SolidColorBrush(Color.FromRgb(31, 58, 147));
            }
            else if (type == JsonItemType.Number)
            {
                return new SolidColorBrush(Color.FromRgb(0, 177, 106));
            }
            else if (type == JsonItemType.Boolean)
            {
                return new SolidColorBrush(Color.FromRgb(31, 58, 147));
            }
            else if (type == JsonItemType.Null)
            {
                return new SolidColorBrush(Color.FromRgb(207, 0, 15));
            }
            else
            {
                return new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
