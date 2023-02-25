using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MView.Validators
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty((string)value))
            {
                return new ValidationResult(false, "The value cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace((string)value))
            {
                return new ValidationResult(false, "The value cannot be null or whitespace.");
            }

            return new ValidationResult(true, "The value is valid.");
        }
    }
}
