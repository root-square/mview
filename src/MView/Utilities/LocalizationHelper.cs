using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView.Utilities
{
    internal static class LocalizationHelper
    {
        internal static string GetText(string key)
        {
            return (string?)App.Current.Resources[key] ?? "NULL";
        }
    }
}
