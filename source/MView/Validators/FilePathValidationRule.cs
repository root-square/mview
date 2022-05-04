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
    public class FilePathValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!File.Exists((string)value))
            {
                return new ValidationResult(false, "The file does not exist.");
            }

            return new ValidationResult(true, "The file does exist.");
        }
    }
}
