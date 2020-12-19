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

        #endregion

        #region ::Constructors::

        public Settings()
        {
            IsSkipUpdates = false;
        }

        #endregion

        #region ::Application Flags::

        public bool IsSkipUpdates { get; set; }

        #endregion

        #region ::General::

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
