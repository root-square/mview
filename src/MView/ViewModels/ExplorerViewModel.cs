using Caliburn.Micro;
using MView.Utilities;
using MView.Utilities.Indexing;
using Ookii.Dialogs.Wpf;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MView.ViewModels
{
    public class ExplorerViewModel : PropertyChangedBase
    {
        private ViewerViewModel Viewer { get; } = IoC.Get<ViewerViewModel>();

        public BindableCollection<IndexedItem> IndexedItems { get; set; } = new BindableCollection<IndexedItem>();

        private BindableCollection<IndexedItem> _selectedItems = new BindableCollection<IndexedItem>();

        public BindableCollection<IndexedItem> SelectedItems
        {
            get => _selectedItems;
            set => Set(ref _selectedItems, value);
        }

        private IndexedItem? _selectedItem = null;

        public IndexedItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value != null)
                {
                    Set(ref _selectedItem, value);
                    RefreshViewerAsync().ConfigureAwait(false).GetAwaiter();
                    RefreshMetadataAsync().ConfigureAwait(false).GetAwaiter();
                }
            }
        }

        public async Task<bool> OpenAsync(string[] itemsToIndex, bool indexAllFiles = false)
        {

        }

        public async void OpenFilesAsync()
        {

        }

        public async void OpenFoldersAsync()
        {

        }

        public async void SelectAsync()
        {
            var task = Task.Factory.StartNew(() =>
            {
                foreach (var item in SelectedItems)
                {
                    item.IsSelected = true;
                }
            });

            await task.ConfigureAwait(false);
        }

        public async void SelectAllAsync()
        {
            var task = Task.Factory.StartNew(() =>
            {
                foreach (var item in IndexedItems)
                {
                    item.IsSelected = true;
                }
            });

            await task.ConfigureAwait(false);
        }

        public async void DeselectAsync()
        {
            var task = Task.Factory.StartNew(() =>
            {
                foreach (var item in SelectedItems)
                {
                    item.IsSelected = false;
                }
            });

            await task.ConfigureAwait(false);
        }

        public async void DeselectAllAsync()
        {
            var task = Task.Factory.StartNew(() =>
            {
                foreach (var item in IndexedItems)
                {
                    item.IsSelected = true;
                }
            });

            await task.ConfigureAwait(false);
        }

        public async void ReverseSelectionAsync()
        {
            var task = Task.Factory.StartNew(() =>
            {
                foreach (var item in IndexedItems)
                {
                    item.IsSelected = !item.IsSelected;
                }
            });

            await task.ConfigureAwait(false);
        }

        public async void DeleteAsync()
        {
            var task = Task.Factory.StartNew(() =>
            {
                List<IndexedItem> targetItems = SelectedItems.ToList();

                foreach (var item in targetItems)
                {
                    SelectedItem = null;
                    SelectedItems.Remove(item);
                    IndexedItems.Remove(item);
                }
            });

            await task.ConfigureAwait(false);
        }

        public async void DeleteAllAsync()
        {
            var task = Task.Factory.StartNew(() =>
            {
                SelectedItem = null;
                SelectedItems.Clear();
                IndexedItems.Clear();
                TODO
            });

            await task.ConfigureAwait(false);
        }
    }
}
