using MView.Utilities.Indexing;
using MView.Utilities.Text;
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

        private ImageSource? _imageViewerSource = null;

        public ImageSource? ImageViewerSource
        {
            get => _imageViewerSource;
            set => Set(ref _imageViewerSource, value);
        }

        private bool _showMetadata = true;

        public bool ShowMetadata
        {
            get => _showMetadata;
            set => Set(ref _showMetadata, value);
        }

        private string? _metadata = "Metadata";

        public string? Metadata
        {
            get => _metadata;
            set => Set(ref _metadata, value);
        }

        #endregion

        #region ::Metadata::

        public void WriteMetadata(string text)
        {
            Metadata += text;
        }

        public void WriteMetadataWithLineFeed(string text)
        {
            if (!string.IsNullOrEmpty(_metadata))
            {
                Metadata += "\r\n";
            }

            Metadata += text;
        }

        public void ClearMetadata()
        {
            Metadata = string.Empty;
        }

        #endregion

        #region ::Image::



        #endregion

        #region ::Audio::



        #endregion

        #region ::Viewer::

        public async Task RefreshViewer()
        {
            /*var task = Task.Factory.StartNew(() =>
            {
                if (_selectedItem != null && _selectedItem.Type == IndexedItemType.File)
                {
                    // Refresh the metadata.
                    ClearMetadata();
                    WriteMetadataWithLineFeed($"* Name : {_selectedItem.Name}");
                    WriteMetadataWithLineFeed($"* Full Name : {_selectedItem.FullName}");
                    WriteMetadataWithLineFeed($"* File Size : {UnitConverter.GetFileSizeString(_selectedItem.Size)}({_selectedItem.Size} Byte)");

                    // Hide viewers.
                    IsImageViewerVisible = false;
                    IsAudioPlayerVisible = false;

                    if (Settings.KnownExtensions.Any(p => p.Equals(Path.GetExtension(_selectedItem.Name), StringComparison.OrdinalIgnoreCase)))
                    {
                        if (Settings.ImageExtensions.Any(p => p.Equals(Path.GetExtension(_selectedItem.Name), StringComparison.OrdinalIgnoreCase)))
                        {
                            // Dispose the image stream to release memories.
                            (ImageViewerSource as BitmapImage)?.StreamSource.Dispose();



                            IsImageViewerVisible = true;
                        }
                        else
                        {
                            IsAudioPlayerVisible = true;
                        }
                    }
                    else
                    {
                        WriteMetadataWithLineFeed($"* This format is not supported.");
                    }
                }
            });

            await task;*/
        }

        #endregion
    }
}
