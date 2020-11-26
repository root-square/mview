using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView
{
    public class Settings
    {
        public static Settings _instance;

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

        #region ::Cryptography::

        public bool VerifyFakeHeaderFlag { get; set; }

        public bool KeepCryptoKeyFlag { get; set; }

        public bool CreateCryptoBackupFlag { get; set; }

        public string CryptoSavePath { get; set; }

        public string CryptoBackupPath { get; set; }

        #endregion

        #region ::RPG Save::

        public bool CreateRpgsaveBackupFlag { get; set; }

        public string RpgsaveSavePath { get; set; }

        public string RpgsaveBackupPath { get; set; }

        #endregion

        #region ::Translation::

        public bool CreateTranslationBackupFlag { get; set; }

        public string TranslationSavePath { get; set; }

        public string TranslationBackupPath { get; set; }

        #endregion
    }
}
