using MView.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView
{
    public class Settings
    {
        #region ::Singleton Members::

        [NonSerialized]
        private static Settings _instance;

        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Settings();
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        #endregion

        #region ::Consts::

        public const string SettingsPath = "./MView/settings.json";

        public const string HistoryPath = "./MView/history.json";

        public const string LayoutPath = "./MView/layout.config";

        public static readonly string[] AvailableExtensions = new string[] { ".rpgmvo", ".rpgmvm", ".rpgmvw", ".rpgmvp", ".ogg_", ".m4a_", ".wav_", ".png_", ".ogg", ".m4a", ".wav", ".png", ".json", ".rpgsave", ".script" }; 

        #endregion

        #region ::Constructors::

        public Settings()
        {
            IsSkipUpdates = false;

            ThemeStyle = ThemeStyle.Light;
        }

        #endregion

        #region ::Application Flags::

        public bool IsSkipUpdates { get; set; }

        #endregion

        #region ::General::

        public ThemeStyle ThemeStyle { get; set; }

        #endregion

        #region ::Add-on::

        #endregion

        #region ::Cryptography Manager::

        #endregion

        #region ::Data Manager::

        #endregion

        #region ::Save Data Manager::

        #endregion

        #region ::Script Manager::

        #endregion
    }
}
