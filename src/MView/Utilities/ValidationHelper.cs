using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MView.Utilities
{
    public static class ValidationHelper
    {
        public static bool IsValid(this DependencyObject instance)
        {
            // Validate recursivly
            return !Validation.GetHasError(instance) && LogicalTreeHelper.GetChildren(instance).OfType<DependencyObject>().All(child => child.IsValid());
        }
    }
}
