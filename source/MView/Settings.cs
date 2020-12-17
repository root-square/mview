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

        #endregion
    }
}
