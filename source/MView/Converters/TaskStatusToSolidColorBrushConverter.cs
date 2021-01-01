using MView.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace MView.Converters
{
    /// <summary>
    /// TaskStatus to SolidColorBrush converter.
    /// </summary>
    [ValueConversion(typeof(TaskStatusType), typeof(SolidColorBrush))]
    public class TaskStatusToSolidColorBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TaskStatusType status = (TaskStatusType)value;

            if (status == TaskStatusType.Idle || status == TaskStatusType.Ready || status == TaskStatusType.Completed)
            {
                return new SolidColorBrush(Color.FromRgb(0, 122, 204));
            }
            else if (status == TaskStatusType.Loading || status == TaskStatusType.Working)
            {
                return new SolidColorBrush(Color.FromRgb(202, 81, 0));
            }
            else
            {
                return new SolidColorBrush(Color.FromRgb(0, 122, 204));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
