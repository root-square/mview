using MView.Bases;
using MView.Core;
using MView.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MView.ViewModels.File
{
    public class JsonFileViewModel : FileViewModelBase
    {
        #region ::Fields::

        private ObservableCollection<JsonItem> _items = new ObservableCollection<JsonItem>();
        private object _selectedItem;
        private JsonItemType _selectedItemType;
        private string _selectedItemPath;
        private string _selectedItemFullPath;
        private string _seledtedItemValue;

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
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                RefreshSelectedItemInformation();
            }
        }

        public JsonItemType SelectedItemType
        {
            get
            {
                return _selectedItemType;
            }
            set
            {
                _selectedItemType = value;
                RaisePropertyChanged();
            }
        }

        public string SelectedItemPath
        {
            get
            {
                return _selectedItemPath;
            }
            set
            {
                _selectedItemPath = value;
                RaisePropertyChanged();
            }
        }

        public string SelectedItemFullPath
        {
            get
            {
                return _selectedItemFullPath;
            }
            set
            {
                _selectedItemFullPath = value;
                RaisePropertyChanged();
            }
        }

        public string SelectedItemValue
        {
            get
            {
                return _seledtedItemValue;
            }
            set
            {
                _seledtedItemValue = value;
                RaisePropertyChanged();
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
            Workspace.Instance.SetStatus(TaskStatusType.Loading, $"Loading a file... ({filePath})");

            JsonItem item = new JsonItem();

            var task = Task.Run(() =>
            {
                try
                {
                    // Preload json items.
                    string jsonString = FileManager.ReadTextFile(filePath, Encoding.UTF8);
                    JToken json = JToken.Parse(jsonString);
                    item = new JsonItem(json, null, true);

                    _fileProperties = new FileProperties(filePath);
                }
                catch (Exception ex)
                {
                    Workspace.Instance.Report.AddReportWithIdentifier($"{ex.Message}\r\n{ex.StackTrace}", ReportType.Warning);
                }
            });

            await task;

            _items.Add(item);

            Workspace.Instance.SetStatus(TaskStatusType.Completed, $"Completed.");
        }

        private void RefreshSelectedItemInformation()
        {
            JsonItem item = (JsonItem)_selectedItem;

            if (item != null)
            {
                SelectedItemType = item.Type;
                SelectedItemPath = item.Path;
                SelectedItemValue = item.Value.ToString();
                SelectedItemFullPath = item.FullPath;

            }
        }

        #endregion
    }
}
