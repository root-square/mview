using MView.Bases;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MView.Converters
{
    /// <summary>
    /// Active document converter.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class ActiveDocumentConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FileViewModelBase)
                return value;

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FileViewModelBase)
                return value;

            return Binding.DoNothing;
        }

        #endregion
    }
}
