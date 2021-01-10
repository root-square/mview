using MView.Bases;
using MView.Core;
using MView.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace MView.ViewModels.File
{
    public class JsonFileViewModel : FileViewModelBase
    {
        #region ::Fields::

        private ObservableCollection<JsonItem> _items = new ObservableCollection<JsonItem>();
        private object _selectedItem;

        private FileProperties _fileProperties = new FileProperties();

        #endregion

        #region ::Constructors::

        public JsonFileViewModel(string filePath) : base(filePath)
        {
            Initialize(filePath);
        }

        #endregion

        #region ::Properties::

        public ObservableCollection<JsonItem> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                RaisePropertyChanged();
            }
        }

        public object SelectedItem
        {
            set
            {
                _selectedItem = value;
                RaisePropertyChanged("SelectedItemType");
                RaisePropertyChanged("SelectedItemPath");
                RaisePropertyChanged("SelectedItemValue");
            }
        }

        public string SelectedItemType
        {
            get
            {
                JsonItem item = (JsonItem)_selectedItem;
                return item?.Type.ToString();
            }
        }

        public string SelectedItemPath
        {
            get
            {
                JsonItem item = (JsonItem)_selectedItem;
                return item?.Path;
            }
        }

        public string SelectedItemValue
        {
            get
            {
                JsonItem item = (JsonItem)_selectedItem;
                return item?.Value.ToString();
            }
        }

        public string FileSizeString
        {
            get
            {
                return _fileProperties.Size;
            }
        }

        #endregion

        #region ::Methods::

        private async void Initialize(string filePath)
        {
            var task = Task.Run(() =>
            {
                try
                {
                    string jsonString = FileManager.ReadTextFile(filePath, Encoding.UTF8);
                    JToken json = JToken.Parse(jsonString);
                    _items.Add(new JsonItem(json, true));

                    _fileProperties = new FileProperties(filePath);
                }
                catch (Exception ex)
                {
                    Workspace.Instance.Report.AddReportWithIdentifier($"{ex.Message}\r\n{ex.StackTrace}", ReportType.Warning);
                }
            });

            await task;
        }

        #endregion
    }
}
