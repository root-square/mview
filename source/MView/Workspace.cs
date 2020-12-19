using AvalonDock.Themes;
using Microsoft.Win32;
using MView.Bases;
using MView.Commands;
using MView.Entities;
using MView.ViewModels.Data;
using MView.ViewModels.Tool;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MView
{
    public class Workspace : ViewModelBase
    {
        #region ::Singleton Members::

        [NonSerialized]
        private static Workspace _instance;

        public static Workspace Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Workspace();
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        #endregion

        #region ::Fields::

        private Tuple<string, Theme> selectedTheme;

        public event EventHandler ActiveDocumentChanged;

        private ObservableCollection<FileViewModelBase> _files = new ObservableCollection<FileViewModelBase>();
        private ReadOnlyObservableCollection<FileViewModelBase> _readonlyFiles = null;
        private ToolViewModelBase[] _tools = null;

        private FileViewModelBase _activeDocument = null;
        private CryptographyManagerViewModel _cryptographyManager = null;
        private DataManagerViewModel _dataManager = null;
        private FileExplorerViewModel _fileExplorer = null;
        private FilePropertiesViewModel _fileProperties = null;
        private ReportViewModel _report = null;
        private SaveDataManagerViewModel _saveDataManager = null;
        private ScriptManagerViewModel _scriptManager = null;

        private RelayCommand _openCommand = null;

        private string _status = "Status";

        #endregion

        #region ::Constructors::

        public Workspace()
        {
            Themes = new List<Tuple<string, Theme>>
            {
                new Tuple<string, Theme>(nameof(GenericTheme), new GenericTheme()),
                new Tuple<string, Theme>(nameof(Vs2013BlueTheme),new Vs2013BlueTheme()),
                new Tuple<string, Theme>(nameof(Vs2013DarkTheme),new Vs2013DarkTheme()),
                new Tuple<string, Theme>(nameof(Vs2013LightTheme),new Vs2013LightTheme()),
            };

            SelectedTheme = Themes[3];
        }

        #endregion

        #region ::Properties::

        public ReadOnlyObservableCollection<FileViewModelBase> Files
        {
            get
            {
                if (_readonlyFiles == null)
                    _readonlyFiles = new ReadOnlyObservableCollection<FileViewModelBase>(_files);

                return _readonlyFiles;
            }
        }

        public IEnumerable<ToolViewModelBase> Tools
        {
            get
            {
                if (_tools == null)
                    _tools = new ToolViewModelBase[] { CryptographyManager, DataManager, FileExplorer, FileProperties, Report, SaveDataManager, ScriptManager };
                return _tools;
            }
        }

        public CryptographyManagerViewModel CryptographyManager
        {
            get
            {
                if (_cryptographyManager == null)
                    _cryptographyManager = new CryptographyManagerViewModel();

                return _cryptographyManager;
            }
        }

        public DataManagerViewModel DataManager
        {
            get
            {
                if (_dataManager == null)
                    _dataManager = new DataManagerViewModel();

                return _dataManager;
            }
        }

        public FilePropertiesViewModel FileProperties
        {
            get
            {
                if (_fileProperties == null)
                    _fileProperties = new FilePropertiesViewModel();

                return _fileProperties;
            }
        }

        public FileExplorerViewModel FileExplorer
        {
            get
            {
                if (_fileExplorer == null)
                    _fileExplorer = new FileExplorerViewModel();

                return _fileExplorer;
            }
        }

        public ReportViewModel Report
        {
            get
            {
                if (_report == null)
                    _report = new ReportViewModel();

                return _report;
            }
        }

        public SaveDataManagerViewModel SaveDataManager
        {
            get
            {
                if (_saveDataManager == null)
                    _saveDataManager = new SaveDataManagerViewModel();

                return _saveDataManager;
            }
        }

        public ScriptManagerViewModel ScriptManager
        {
            get
            {
                if (_scriptManager == null)
                    _scriptManager = new ScriptManagerViewModel();

                return _scriptManager;
            }
        }

        public ICommand OpenCommand
        {
            get
            {
                if (_openCommand == null)
                {
                    _openCommand = new RelayCommand((p) => OnOpen(p), (p) => CanOpen(p));
                }

                return _openCommand;
            }
        }

        public FileViewModelBase ActiveDocument
        {
            get
            {
                return _activeDocument;
            }
            set
            {
                if (_activeDocument != value)
                {
                    _activeDocument = value;
                    RaisePropertyChanged();
                    if (ActiveDocumentChanged != null)
                        ActiveDocumentChanged(this, EventArgs.Empty);
                }
            }
        }

        public List<Tuple<string, Theme>> Themes { get; set; }

        public Tuple<string, Theme> SelectedTheme
        {
            get 
            {
                return selectedTheme;
            }
            set
            {
                selectedTheme = value;
                RaisePropertyChanged();
            }
        }

        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ::Methods::

        internal void Close(FileViewModelBase fileToClose)
        {
            if (fileToClose.IsDirty)
            {
                var res = MessageBox.Show(string.Format("Save changes for file '{0}'?", fileToClose.FileName), "MView", MessageBoxButton.YesNoCancel);
                if (res == MessageBoxResult.Cancel)
                    return;
                if (res == MessageBoxResult.Yes)
                {
                    Save(fileToClose);
                }
            }

            _files.Remove(fileToClose);
        }

        internal void Save(FileViewModelBase fileToSave, bool saveAsFlag = false)
        {
            if (fileToSave.FilePath == null || saveAsFlag)
            {
                var dlg = new SaveFileDialog();
                if (dlg.ShowDialog().GetValueOrDefault())
                    fileToSave.FilePath = dlg.SafeFileName;
            }

            if (fileToSave.FilePath == null)
            {
                return;
            }

            if (fileToSave is AudioDataViewModel)
            {
                // TODO : Save action
            }
            else if (fileToSave is GeneralDataViewModel)
            {
                // TODO : Save action
            }
            else if (fileToSave is ImageDataViewModel)
            {
                // TODO : Save action
            }
            else if (fileToSave is JsonDataViewModel)
            {
                // TODO : Save action
            }
            else if (fileToSave is SaveDataViewModel)
            {
                // TODO : Save action
            }
            else if (fileToSave is ScriptDataViewModel)
            {
                // TODO : Save action
            }

            ActiveDocument.IsDirty = false;
        }

        internal FileViewModelBase Open(string filePath)
        {
            var fileViewModel = _files.FirstOrDefault(fm => fm.FilePath == filePath);
            if (fileViewModel != null)
                return fileViewModel;

            string extension = Path.GetExtension(filePath).ToLower();

            switch (extension)
            {
                case ".ogg":
                    fileViewModel = new AudioDataViewModel(filePath);
                    _files.Add(fileViewModel);
                    break;
                case ".m4a":
                    fileViewModel = new AudioDataViewModel(filePath);
                    _files.Add(fileViewModel);
                    break;
                case ".wav":
                    fileViewModel = new AudioDataViewModel(filePath);
                    _files.Add(fileViewModel);
                    break;
                case ".png":
                    fileViewModel = new ImageDataViewModel(filePath);
                    _files.Add(fileViewModel);
                    break;
                case ".json":
                    fileViewModel = new JsonDataViewModel(filePath);
                    _files.Add(fileViewModel);
                    break;
                case ".rpgsave":
                    fileViewModel = new SaveDataViewModel(filePath);
                    _files.Add(fileViewModel);
                    break;
                case ".script":
                    fileViewModel = new ScriptDataViewModel(filePath);
                    _files.Add(fileViewModel);
                    break;
                default:
                    fileViewModel = new GeneralDataViewModel(filePath);
                    _files.Add(fileViewModel);
                    break;
            }
            return fileViewModel;
        }

        #endregion

        #region ::Command Actions::

        private bool CanOpen(object parameter) => true;

        private void OnOpen(object parameter)
        {
            var dlg = new OpenFileDialog();

            if (dlg.ShowDialog().GetValueOrDefault())
            {
                var fileViewModel = Open(dlg.FileName);
                ActiveDocument = fileViewModel;
            }
        }

        #endregion
    }
}
