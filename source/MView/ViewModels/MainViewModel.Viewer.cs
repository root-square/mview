using MView.Utilities;
using MView.Utilities.Indexing;
using MView.Utilities.Text;
using NAudio.Vorbis;
using NAudio.Wave;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MView.ViewModels
{
    public partial class MainViewModel
    {
        #region ::Variables::

        // Viewer visibility
        private bool _isImageViewerVisible = false;

        public bool IsImageViewerVisible
        {
            get => _isImageViewerVisible;
            set => Set(ref _isImageViewerVisible, value);
        }

        private bool _isAudioPlayerVisible = false;

        public bool IsAudioPlayerVisible
        {
            get => _isAudioPlayerVisible;
            set => Set(ref _isAudioPlayerVisible, value);
        }

        // Source stream.
        private Stream _sourceStream = Stream.Null;

        public Stream SourceStream
        {
            get => _sourceStream;
            set => Set(ref _sourceStream, value);
        }

        // Image
        private Stream _imageStream = Stream.Null;

        public Stream ImageStream
        {
            get => _imageStream;
            set => Set(ref _imageStream, value);
        }

        // Audio
        private IWavePlayer? _wavePlayer = null;

        public IWavePlayer? WavePlayer
        {
            get => _wavePlayer;
            set => Set(ref _wavePlayer, value);
        }

        private WaveStream? _waveStream = null;

        public WaveStream? WaveStream
        {
            get => _waveStream;
            set => Set(ref _waveStream, value);
        }

        private Timer _refreshTimer = new Timer(1000);

        public TimeSpan CurrentTime
        {
            get
            {
                if (_waveStream != null)
                {
                    return _waveStream.CurrentTime;
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
            set
            {
                if (_waveStream != null)
                {
                    _waveStream.CurrentTime = value;
                    NotifyOfPropertyChange("CurrentTime");
                }
            }
        }

        public TimeSpan TotalTime
        {
            get
            {
                if (_waveStream != null)
                {
                    return _waveStream.TotalTime;
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }

        public float AudioVolume
        {
            get => _wavePlayer?.Volume ?? 100.0F;
            set
            {
                if (WavePlayer != null)
                {
                    WavePlayer.Volume = value;
                    NotifyOfPropertyChange("AudioVolume");
                }
            }
        }

        // Metadata
        private bool _showMetadata = true;

        public bool ShowMetadata
        {
            get => _showMetadata;
            set => Set(ref _showMetadata, value);
        }

        private StringBuilder _metadataBuilder = new StringBuilder();

        private string _metadata = String.Empty;

        public string Metadata
        {
            get => _metadata;
            set => Set(ref _metadata, value);
        }

        #endregion

        #region ::Metadata::

        public void AppendMetadata(string text)
        {
            _metadataBuilder.Append(text);
        }

        public void AppendMetadataLine(string text)
        {
            string metadata = _metadataBuilder.ToString();

            if (!string.IsNullOrEmpty(metadata) &&!metadata.EndsWith(Environment.NewLine))
            {
                _metadataBuilder.Append("\r\n");
            }

            _metadataBuilder.Append(text);
        }

        public void ClearMetadata()
        {
            _metadataBuilder.Clear();
            BuildMetadata();
        }

        public void BuildMetadata()
        {
            Metadata = _metadataBuilder.ToString();
        }

        public async Task RefreshMetadataAsync()
        {
            var task = Task.Factory.StartNew(() =>
            {
                if (!File.Exists(_selectedItem.FullPath))
                {
                    Log.Warning("The selected file does not exist.");
                    return;
                }

                // Write a metadata.
                ClearMetadata();

                AppendMetadataLine($"* File Name : {_selectedItem.FileName}");
                AppendMetadataLine($"* Full Path : {_selectedItem.FullPath}");
                AppendMetadataLine($"* File Size : {UnitConverter.GetFileSizeString(_selectedItem.Size)}({_selectedItem.Size} Byte)");

                string extension = Path.GetExtension(_selectedItem.FileName);

                if (!Settings.KnownExtensions.Any(p => p.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                {
                    AppendMetadataLine($"* This format is not supported.");
                }

                Log.Information("The metadata has been refreshed(FILE : {_selectedItem.FullPath}).");
                BuildMetadata();
            });

            await task;
        }

        #endregion

        #region ::Audio::

        private void InitializeAudio(Stream stream, bool isVorbis = false)
        {
            // Dispose the previous player.
            if (_wavePlayer != null)
            {
                _wavePlayer.Dispose();
                _wavePlayer = null;
            }

            // Initialize a player.
            _wavePlayer = new WaveOut();

            // Read a audio file.
            if (isVorbis)
            {
                _waveStream = new VorbisWaveReader(stream, true);
            }
            else
            {
                _waveStream = new StreamMediaFoundationReader(stream);
            }

            _wavePlayer.Init(_waveStream);

            // Refresh times.
            _waveStream.CurrentTime = TimeSpan.Zero;

            NotifyOfPropertyChange("CurrentTime");
            NotifyOfPropertyChange("TotalTime");

            // Add events.
            _wavePlayer.PlaybackStopped += (sender, e) =>
            {
                _waveStream.CurrentTime = TimeSpan.Zero;

                NotifyOfPropertyChange("CurrentTime");
                NotifyOfPropertyChange("TotalTime");
            };

            // Initialize the refresh timer.
            if (_refreshTimer != null)
            {
                _refreshTimer.Dispose();
            }

            _refreshTimer = new Timer(1000);

            _refreshTimer.Elapsed += (sender, e) =>
            {
                if (_wavePlayer.PlaybackState == PlaybackState.Playing)
                {
                    NotifyOfPropertyChange("CurrentTime");
                }
            };
        }

        public void PlayAudio()
        {
            if (_wavePlayer != null && _wavePlayer.PlaybackState != PlaybackState.Playing && _refreshTimer != null)
            {
                _wavePlayer.Play();
                _refreshTimer.Start();
            }
        }

        public void PauseAudio()
        {
            if (_wavePlayer != null && _wavePlayer.PlaybackState == PlaybackState.Playing && _refreshTimer != null)
            {
                _wavePlayer.Pause();
                _refreshTimer.Stop();
            }
        }

        public void StopAudio()
        {
            if (_wavePlayer != null && _wavePlayer.PlaybackState == PlaybackState.Playing && _refreshTimer != null)
            {
                _wavePlayer.Stop();
                _refreshTimer.Stop();
            }
        }

        #endregion

        #region ::Viewer::

        public async Task RefreshViewerAsync()
        {
            var task = Task.Factory.StartNew(async () =>
            {
                if (!File.Exists(_selectedItem.FullPath))
                {
                    Log.Warning("The selected file does not exist.");
                    return;
                }

                // Hide viewers.
                IsImageViewerVisible = false;
                IsAudioPlayerVisible = false;

                string extension = Path.GetExtension(_selectedItem.FileName);

                if (Settings.KnownExtensions.Any(p => p.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                {
                    // Dispose all streams.
                    if (_sourceStream != null && _sourceStream != Stream.Null)
                    {
                        _sourceStream.Dispose();
                    }

                    if (_imageStream != null && _imageStream != Stream.Null)
                    {
                        _imageStream.Dispose();
                    }

                    // Get a file stream;
                    if (CryptographyProvider.EXTENSIONS_ENCRYPTED.Any(p => p.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                    {
                        SourceStream = await CryptographyProvider.GetRestoredFileAsync(_selectedItem.FullPath);
                    }
                    else if (CryptographyProvider.EXTENSIONS_DECRYPTED.Any(p => p.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                    {
                        SourceStream = new FileStream(_selectedItem.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    }
                    else
                    {
                        throw new Exception("No extensions found.");
                    }

                    // Show a viewer.
                    if (Settings.ImageExtensions.Any(p => p.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                    {
                        // Refresh the image viewer stream.
                        ImageStream = SourceStream;

                        // Show the image viewer.
                        IsImageViewerVisible = true;
                    }
                    else
                    {
                        // Refresh the audio player stream.
                        string[] oggVorbisExtensions = new string[] { ".ogg", ".rpgmvo", ".ogg_" };

                        bool isVorbis = oggVorbisExtensions.Any(p => p.Equals(extension, StringComparison.OrdinalIgnoreCase));
                        InitializeAudio(_sourceStream, isVorbis);

                        // Show the audio player.
                        IsAudioPlayerVisible = true;
                    }
                }

                Log.Information($"The viewer has been refreshed(FILE : {_selectedItem.FullPath}).");
            });

            await task;
        }

        public void ResetViewer()
        {
            // Dispose all streams.
            if (_sourceStream != null && _sourceStream != Stream.Null)
            {
                _sourceStream.Dispose();
                SourceStream = Stream.Null;
            }

            if (_imageStream != null && _imageStream != Stream.Null)
            {
                _imageStream.Dispose();
                ImageStream = Stream.Null;
            }

            // Hide viewers.
            IsImageViewerVisible = false;
            IsAudioPlayerVisible = false;

            // Clear the metadata.
            ClearMetadata();

            Log.Information("The viewer has been resetted.");
        }

        #endregion
    }
}
