using Caliburn.Micro;
using HL.Manager;
using HL.Xshtd;
using HL.Xshtd.interfaces;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using LZStringCSharp;
using MView.Extensions;
using MView.Utilities;
using MView.Utilities.Indexing;
using MView.Utilities.Text;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MView.ViewModels.Pages
{
    public class CodeViewerViewModel : Screen
    {
        private IndexedItem? _item = null;

        public IndexedItem? Item
        {
            get => _item;
            set => Set(ref _item, value);
        }

        private IHighlightingDefinition? _highlighting = null;

        public IHighlightingDefinition? Highlighting
        {
            get => _highlighting;
            set => Set(ref _highlighting, value);
        }

        private TextDocument? _document = null;

        public TextDocument? Document
        {
            get => _document;
            set => Set(ref _document, value);
        }

        public async Task RefreshThemeAsync()
        {
            var task = Task.Run(() =>
            {
                if (IoC.Get<Settings>().UseDarkTheme)
                {
                    ThemedHighlightingManager.Instance.SetCurrentTheme("VS2019_Dark");
                }
                else
                {
                    ThemedHighlightingManager.Instance.SetCurrentTheme("Light");
                }

                string extension = Path.GetExtension(_item?.FullPath) ?? ".txt";
                Highlighting = ThemedHighlightingManager.Instance.GetDefinitionByExtension(extension);
            });

            await task;
        }

        public async Task SetContentAsync(IndexedItem? item)
        {
            // Note: If the file is too large, it will not be loaded.
            var lineCount = File.ReadLines(item?.FullPath!).Count();

            if ((item?.Size / lineCount) > 100000)
            {
                throw new InvalidOperationException("The file is too large to load.");
            }

            Item = item;
            Document = new TextDocument(await TextManager.ReadTextFileAsync(item?.FullPath!, Encoding.UTF8));

            await RefreshThemeAsync();
        }
    }
}
