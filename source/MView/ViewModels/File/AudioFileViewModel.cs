using MView.Bases;
using MView.Commands;
using MView.Core;
using MView.Core.Cryptography;
using MView.Entities;
using MView.Utilities;
using MView.Utilities.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MView.ViewModels.File
{
    public class AudioFileViewModel : FileViewModelBase, IDisposable
    {
        #region ::Fields::

        private AudioPlayback _audioPlayback;
        private List<IVisualizationPlugin> _visualizations;
        private IVisualizationPlugin _selectedVisualization;
        private float _volume = 1.0f;

        private FileProperties _fileProperties = new FileProperties();

        private ICommand _playCommand;
        private ICommand _pauseCommand;
        private ICommand _stopCommand;

        #endregion

        #region ::Constructors::

        public AudioFileViewModel(string filePath) : base(filePath)
        {
            Initialize(filePath);
        }

        #endregion

        #region ::Properties::

        public IList<IVisualizationPlugin> Visualizations
        {
            get
            {
                return _visualizations;
            }
        }

        public IVisualizationPlugin SelectedVisualization
        {
            get
            {
                return _selectedVisualization;
            }
            set
            {
                if (_selectedVisualization != value)
                {
                    _selectedVisualization = value;
                    RaisePropertyChanged("SelectedVisualization");
                    RaisePropertyChanged("Visualization");
                }
            }
        }

        public object Visualization
        {
            get
            {
                return _selectedVisualization.Content;
            }
        }

        public float Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = value;
                _audioPlayback.SetVolume(value);
                RaisePropertyChanged();
                RaisePropertyChanged("VolumeString");
            }
        }

        public string VolumeString
        {
            get
            {
                return $"{Math.Round(_volume * 100)}%";
            }
        }

        public string FileSizeString
        {
            get
            {
                return _fileProperties.Size;
            }
        }

        public ICommand PlayCommand
        {
            get
            {
                return (_playCommand) ?? (_playCommand = new DelegateCommand(Play));
            }
        }

        public ICommand PauseCommand
        {
            get
            {
                return (_pauseCommand) ?? (_pauseCommand = new DelegateCommand(Pause));
            }
        }

        public ICommand StopCommand
        {
            get
            {
                return (_stopCommand) ?? (_stopCommand = new DelegateCommand(Stop));
            }
        }

        #endregion

        #region ::Methods::

        private async void Initialize(string filePath)
        {
            string loadedFilePath = await FileLoadAsync(filePath);

            if (loadedFilePath == null)
            {
                return;
            }

            // Initialize AudioPlayback and Visualizations.
            _visualizations = ReflectionHelper.CreateAllInstancesOf<IVisualizationPlugin>().ToList();
            _selectedVisualization = _visualizations.FirstOrDefault();

            _audioPlayback = new AudioPlayback();
            _audioPlayback.MaximumCalculated += OnAudioGraphMaximumCalculated;
            _audioPlayback.FftCalculated += OnAudioGraphFftCalculated;

            string extension = Path.GetExtension(filePath).ToLower();

            if (extension == ".ogg" || extension == ".rpgmvo" || extension == ".ogg_")
            {
                _audioPlayback.Load(loadedFilePath, true); // Load audio file using Ogg Vorbis wave reader.
            }
            else
            {
                _audioPlayback.Load(loadedFilePath);
            }

            _fileProperties = new FileProperties(filePath);
        }

        private async Task<string> FileLoadAsync(string filePath)
        {
            try
            {
                string extension = Path.GetExtension(filePath).ToLower();

                if (extension == ".rpgmvo" || extension == ".ogg_" || extension == ".rpgmvm" || extension == ".m4a_" || extension == ".rpgmvw" || extension == ".wav_")
                {
                    var verifyTask = new Task<bool>(() => CryptographyProvider.VerifyFakeHeader(filePath));
                    verifyTask.Start();
                    await verifyTask;

                    if (!verifyTask.Result)
                    {
                        throw new InvalidDataException("An invalid *.rpgmvo(*.ogg_), *.rpgmvm(*.m4a_), *.rpgmvw(*.wav_) file was entered.");
                    }

                    string tempFilePath = Path.GetTempFileName();

                    var restoreTask = new Task(() => CryptographyProvider.RestoreHeader(filePath, tempFilePath));
                    restoreTask.Start();
                    await restoreTask;

                    return tempFilePath;
                }
                else
                {
                    return filePath;
                }
            }
            catch (Exception ex)
            {
                Workspace.Instance.Report.AddReportWithIdentifier($"{ex.Message}\r\n{ex.StackTrace}", ReportType.Warning);
                return null;
            }
        }

        private void Play()
        {
            if (FilePath == null)
            {
                return;
            }
            if (FilePath != null)
            {
                _audioPlayback.Play();
            }
        }

        private void Stop()
        {
            _audioPlayback.Stop();
        }

        private void Pause()
        {
            _audioPlayback.Pause();
        }

        #endregion

        #region ::Event Subscribers::

        void OnAudioGraphFftCalculated(object sender, FftEventArgs e)
        {
            if (SelectedVisualization != null)
            {
                SelectedVisualization.OnFftCalculated(e.Result);
            }
        }

        void OnAudioGraphMaximumCalculated(object sender, MaxSampleEventArgs e)
        {
            if (SelectedVisualization != null)
            {
                SelectedVisualization.OnMaxCalculated(e.MinSample, e.MaxSample);
            }
        }

        #endregion

        #region ::IDisposable Members::

        protected override void OnClose()
        {
            Dispose();
            base.OnClose();
        }

        ~AudioFileViewModel()
        {
            Dispose(false);
        }

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                Stop();
                _audioPlayback.Dispose();
            }

            if (disposing)
            {
                Stop();
                _audioPlayback.Dispose();
            }

            // .NET Framework에 의하여 관리되지 않는 외부 리소스들을 여기서 정리합니다.
            _disposed = true;
        }

        #endregion
    }
}
