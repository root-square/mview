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
            var attribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            return attribute != null ? attribute!.InformationalVersion : "dev";
        }

        internal static string GetFileVersion()
        {
            var attribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyVersionAttribute>();
            return attribute != null ? attribute!.Version : "dev";
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
