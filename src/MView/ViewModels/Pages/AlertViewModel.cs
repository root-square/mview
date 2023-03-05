using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using MView.Utilities.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib.Ape;

namespace MView.ViewModels.Pages
{
    public class AlertViewModel : Screen
    {
        private IndexedItem? _item = null;

        public IndexedItem? Item
        {
            get => _item;
            set => Set(ref _item, value);
        }

        private PackIconKind _icon = PackIconKind.Alert;

        public PackIconKind Icon
        {
            get => _icon;
            set => Set(ref _icon, value);
        }

        private string _text = string.Empty;

        public string Text
        {
            get => _text;
            set => Set(ref _text, value);
        }

        public void SetContent(IndexedItem? item, PackIconKind icon, string text)
        {
            Item = item;
            Icon = icon;
            Text = text;
        }
    }
}
