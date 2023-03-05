using Caliburn.Micro;
using MView.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView
{
    public class Settings : PropertyChangedBase
    {
        #region ::GENERAL::

        private bool _useDarkTheme = false;

        public bool UseDarkTheme
        {
            get => _useDarkTheme;
            set
            {
                Set(ref _useDarkTheme, value);
                App.ApplyTheme(value);
            }
        }

        #endregion

        #region ::THREADING::

        private bool _useMultiThreading = true;

        public bool UseMultiThreading
        {
            get => _useMultiThreading;
            set => Set(ref _useMultiThreading, value);
        }

        private int _numberOfThreads = 8;

        public int NumberOfThreads
        {
            get => _numberOfThreads;
            set => Set(ref _numberOfThreads, value);
        }

        #endregion

        #region ::VIEWER::

        private bool _useAdaptiveBackgroundColor = true;

        public bool UseAdaptiveBackgroundColor
        {
            get => _useAdaptiveBackgroundColor;
            set => Set(ref _useAdaptiveBackgroundColor, value);
        }

        private float _audioVolume = 1.0F;

        public float AudioVolume
        {
            get => _audioVolume;
            set => Set(ref _audioVolume, value);
        }

        #endregion

        #region ::CONTROLLER::

        private string _encryptionKey = string.Empty;

        public string EncryptionKey
        {
            get => _encryptionKey;
            set => Set(ref _encryptionKey, value);
        }

        private string _outputPath = string.Empty;

        public string OutputPath
        {
            get => _outputPath;
            set => Set(ref _outputPath, value);
        }

        private bool _useRMMV = true;

        public bool UseRMMV
        {
            get => _useRMMV;
            set => Set(ref _useRMMV, value);
        }

        private bool _rememberInputs = true;

        public bool RememberInputs
        {
            get => _rememberInputs;
            set => Set(ref _rememberInputs, value);
        }

        private bool _outputToSourcePath = false;

        public bool OutputToSourcePath
        {
            get => _outputToSourcePath;
            set => Set(ref _outputToSourcePath, value);
        }

        private bool _encryptAllFiles = false;

        public bool EncryptAllFiles
        {
            get => _encryptAllFiles;
            set => Set(ref _encryptAllFiles, value);
        }

        private bool _copyUnencryptedFiles = true;

        public bool CopyUnencryptedFiles
        {
            get => _copyUnencryptedFiles;
            set => Set(ref _copyUnencryptedFiles, value);
        }

        private bool _inferExtension = true;

        public bool InferExtension
        {
            get => _inferExtension;
            set => Set(ref _inferExtension, value);
        }

        #endregion

        public void DeepCopyFrom(Settings settings)
        {
            UseDarkTheme = settings.UseDarkTheme;

            UseMultiThreading = settings.UseMultiThreading;
            NumberOfThreads = settings.NumberOfThreads;

            UseAdaptiveBackgroundColor = settings.UseAdaptiveBackgroundColor;
            AudioVolume = settings.AudioVolume;

            EncryptionKey = settings.EncryptionKey;
            OutputPath = settings.OutputPath;
            UseRMMV = settings.UseRMMV;
            RememberInputs = settings.RememberInputs;
            OutputToSourcePath = settings.OutputToSourcePath;
            EncryptAllFiles = settings.EncryptAllFiles;
            CopyUnencryptedFiles = settings.CopyUnencryptedFiles;
            InferExtension = settings.InferExtension;
        }
    }
}
