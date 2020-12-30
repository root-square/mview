using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace MView.Converters
{
    [ValueConversion(typeof(bool), typeof(Nullable<bool>))]
    public class InverseBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool?)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool?)value == null)
            {
                return false;
            }
            else
            {
                return !(bool)value;
            }
        }

        #endregion
    }
}
