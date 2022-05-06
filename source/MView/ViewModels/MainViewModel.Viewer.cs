using MView.Utilities;
using MView.Utilities.Indexing;
using MView.Utilities.Text;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MView.ViewModels
{
    public partial class MainViewModel
    {
        #region ::Variables::

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

        private Stream _sourceStream = Stream.Null;

        public Stream SourceStream
        {
            get => _sourceStream;
            set => Set(ref _sourceStream, value);
        }

        private Stream _imageViewerStream = Stream.Null;

        public Stream ImageViewerStream
        {
            get => _imageViewerStream;
            set => Set(ref _imageViewerStream, value);
        }

        private Stream _audioPlayerStream = Stream.Null;

        public Stream AudioPlayerStream
        {
            get => _audioPlayerStream;
            set => Set(ref _audioPlayerStream, value);
        }

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

        #endregion

        #region ::Image::



        #endregion

        #region ::Audio::



        #endregion

        #region ::Viewer::

        public async Task RefreshViewerAsync()
        {
            var task = Task.Factory.StartNew(() =>
            {
                if (!File.Exists(_selectedItem.FullPath))
                {
                    Log.Warning("The selected file does not exist.");
                    return;
                }

                // Hide viewers.
                IsImageViewerVisible = false;
                IsAudioPlayerVisible = false;

                // Write a metadata.
                ClearMetadata();

                AppendMetadataLine($"* File Name : {_selectedItem.FileName}");
                AppendMetadataLine($"* Full Path : {_selectedItem.FullPath}");
                AppendMetadataLine($"* File Size : {UnitConverter.GetFileSizeString(_selectedItem.Size)}({_selectedItem.Size} Byte)");

                string extension = Path.GetExtension(_selectedItem.FileName);

                if (Settings.KnownExtensions.Any(p => p.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                {
                    // Dispose all streams.
                    if (_sourceStream != null && _sourceStream != Stream.Null)
                    {
                        _sourceStream.Dispose();
                    }

                    if (_imageViewerStream != null && _imageViewerStream != Stream.Null)
                    {
                        _imageViewerStream.Dispose();
                    }

                    if (_audioPlayerStream != null && _audioPlayerStream != Stream.Null)
                    {
                        _audioPlayerStream.Dispose();
                    }

                    if (CryptographyProvider.EXTENSIONS_ENCRYPTED.Any(p => p.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                    {
                        SourceStream = CryptographyProvider.GetRestoredFileStream(_selectedItem.FullPath);
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
                        ImageViewerStream = SourceStream;

                        // Show the image viewer.
                        IsImageViewerVisible = true;
                    }
                    else
                    {
                        // Refresh the audio player stream.
                        AudioPlayerStream = SourceStream;

                        // Show the audio player.
                        IsAudioPlayerVisible = true;
                    }
                }
                else
                {
                    AppendMetadataLine($"* This format is not supported.");
                }

                Log.Information("The viewer has been refreshed.");
                BuildMetadata();
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

            if (_imageViewerStream != null && _imageViewerStream != Stream.Null)
            {
                _imageViewerStream.Dispose();
                ImageViewerStream = Stream.Null;
            }

            if (_audioPlayerStream != null && _audioPlayerStream != Stream.Null)
            {
                _audioPlayerStream.Dispose();
                AudioPlayerStream = Stream.Null;
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
