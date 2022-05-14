﻿using Caliburn.Micro;
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
        public readonly static string[] KnownExtensions = new string[] { ".png", ".ogg", ".m4a", ".wav", ".rpgmvp", ".rpgmvo", ".rpgmvm", ".rpgmvw", ".png_", ".ogg_", ".m4a_", ".wav_" };

        public readonly static string[] ImageExtensions = new string[] { ".png", ".rpgmvp", ".png_" };

        public readonly static string[] AudioExtensions = new string[] { ".ogg", ".m4a", ".wav", ".rpgmvo", ".rpgmvm", ".rpgmvw", ".ogg_", ".m4a_", ".wav_" };

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

        private bool _useBackFiller = true;

        public bool UseBackFiller
        {
            get => _useBackFiller;
            set => Set(ref _useBackFiller, value);
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
            UseMultiThreading = settings.UseMultiThreading;
            NumberOfThreads = settings.NumberOfThreads;
            AudioVolume = settings.AudioVolume;
        }
    }
}
