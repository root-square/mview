using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public Settings()
        {
            BackupPath = Path.Combine(Application.StartupPath, "backup");

            VerifyFakeHeaderFlag = true;
            UseEncryptionCodeFlag = true;
            CreateCryptoBackupFlag = true;
            CryptoSavePath = string.Empty;

            CreateCryptoBackupFlag = true;
            RpgsaveSavePath = string.Empty;

            CreateTranslationBackupFlag = true;
            TranslationSavePath = string.Empty;
        }

        #region ::General::

        public string BackupPath { get; set; }

        #endregion

        #region ::Cryptography::

        public bool VerifyFakeHeaderFlag { get; set; }

        public bool UseEncryptionCodeFlag { get; set; }

        public bool CreateCryptoBackupFlag { get; set; }

        public string CryptoSavePath { get; set; }

        #endregion

        #region ::RPG Save::

        public bool CreateRpgsaveBackupFlag { get; set; }

        public string RpgsaveSavePath { get; set; }

        #endregion

        #region ::Translation::

        public bool CreateTranslationBackupFlag { get; set; }

        public string TranslationSavePath { get; set; }

        #endregion
    }
}
