using Caliburn.Micro;
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
        // Constants
        public readonly static string SettingsPath = Path.Combine(Environment.CurrentDirectory, @"data\settings.json");

        // Variables
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

        private bool _useAdaptiveBackgroundColor = true;

        public bool UseAdaptiveBackgroundColor
        {
            get => _useAdaptiveBackgroundColor;
            set => Set(ref _useAdaptiveBackgroundColor, value);
        }

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

        // Preferences
        private float _audioVolume = 1.0F;

        public float AudioVolume
        {
            get => _audioVolume;
            set => Set(ref _audioVolume, value);
        }

        // Methods
        public void DeepCopyFrom(Settings settings)
        {
            UseDarkTheme = settings.UseDarkTheme;
            UseAdaptiveBackgroundColor = settings.UseAdaptiveBackgroundColor;
            UseMultiThreading = settings.UseMultiThreading;
            NumberOfThreads = settings.NumberOfThreads;
            AudioVolume = settings.AudioVolume;
        }
    }
}
