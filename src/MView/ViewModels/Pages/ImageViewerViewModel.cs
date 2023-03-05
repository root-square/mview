using Caliburn.Micro;
using MView.Utilities.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView.ViewModels.Pages
{
    public class ImageViewerViewModel : Screen
    {
        private IndexedItem? _item = null;

        public IndexedItem? Item
        {
            get => _item;
            set => Set(ref _item, value);
        }

        public void Set(IndexedItem? item)
        {
            Item = item;
        }
    }
}
