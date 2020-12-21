using AvalonDock.Themes;
using Microsoft.Win32;
using MView.Bases;
using MView.Commands;
using MView.Entities;
using MView.ViewModels.File;
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

        [NonSerialized]
        private Theme _selectedTheme = new Vs2013LightTheme();

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

        #endregion

        #region ::Constructors::

        public Workspace()
        {

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
            set
            {
                _report = value;
                RaisePropertyChanged();
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

        public Theme SelectedTheme
        {
            get 
            {
                return _selectedTheme;
            }
            set
            {
                _selectedTheme = value;
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

            if (fileToSave is AudioFileViewModel)
            {
                // TODO : Save action
            }
            else if (fileToSave is GeneralFileViewModel)
            {
                // TODO : Save action
            }
            else if (fileToSave is ImageFileViewModel)
            {
                // TODO : Save action
            }
            else if (fileToSave is JsonFileViewModel)
            {
                // TODO : Save action
            }
            else if (fileToSave is SaveFileViewModel)
            {
                // TODO : Save action
            }
            else if (fileToSave is ScriptFileViewModel)
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
                    fileViewModel = new AudioFileViewModel(filePath);
                    _files.Add(fileViewModel);
                    break;
                case ".m4a":
                    fileViewModel = new AudioFileViewModel(filePath);
                    _files.Add(fileViewModel);
                    break;
                case ".wav":
                    fileViewModel = new AudioFileViewModel(filePath);
                    _files.Add(fileViewModel);
                    break;
                case ".png":
                    fileViewModel = new ImageFileViewModel(filePath);
                    _files.Add(fileViewModel);
                    break;
                case ".json":
                    fileViewModel = new JsonFileViewModel(filePath);
                    _files.Add(fileViewModel);
                    break;
                case ".rpgsave":
                    fileViewModel = new SaveFileViewModel(filePath);
                    _files.Add(fileViewModel);
                    break;
                case ".script":
                    fileViewModel = new ScriptFileViewModel(filePath);
                    _files.Add(fileViewModel);
                    break;
                default:
                    fileViewModel = new GeneralFileViewModel(filePath);
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
