using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MView.Utilities
{
    internal static class VariableBuilder
    {
        #region ::Application::

        internal static string GetProductVersion()
        {
            return Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
        }

        internal static string GetFileVersion()
        {
            return Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyVersionAttribute>()!.Version;
        }

        internal static string GetApplicationLocation()
        {
            return Path.Combine(GetBaseDirectory(), AppDomain.CurrentDomain.FriendlyName + ".exe");
        }

        #endregion

        #region ::I/O::

        internal static string GetBaseDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        internal static string GetSettingsPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\settings.json");
        }

        #endregion
    }
}
