using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MView.Validators
{
    public class DirectoryValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!Directory.Exists((string)value))
            {
                return new ValidationResult(false, "The directory does not exist.");
            }

            return new ValidationResult(true, "The directory does exist.");
        }
    }
}
