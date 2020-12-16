using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MView.Converters
{
    /// <summary>
    /// Boolean to Visibility converter.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isInverted = parameter == null ? false : (bool)parameter;
            bool isVisible = value == null ? false : (bool)value;

            if (isVisible)
            {
                return isInverted ? Visibility.Hidden : Visibility.Visible;
            }
            else
            {
                return isInverted ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visiblility = value == null ? Visibility.Hidden : (Visibility)value;
            bool isInverted = parameter == null ? false : (bool)parameter;

            return (visiblility == Visibility.Visible) != isInverted;
        }

        #endregion
    }
}
