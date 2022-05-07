using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MView.Validators
{
    public class EncryptionKeyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (Regex.IsMatch((string)value, @"[0-9a-zA-Z]{32}"))
            {
                return new ValidationResult(true, "The encryption key is valid.");
            }
            else
            {
                return new ValidationResult(false, "The encryption key is not valid.");
            }
        }
    }
}
