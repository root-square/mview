using AvalonDock.Themes;
using Microsoft.Win32;
using MView.Bases;
using MView.Commands;
using MView.Entities;
using MView.ViewModels.File;
using MView.ViewModels.Tool;
using MView.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private FileExplorerViewModel _fileExplorer = null;
        private FilePropertiesViewModel _fileProperties = null;
        private ReportViewModel _report = null;
        private ToolboxViewModel _toolbox = null;

        // Status
        private TaskStatusType _status = TaskStatusType.Idle;
        private string _statusString = "Idle";

        // Commands
        private RelayCommand _openCommand = null;
        private DelegateCommand _saveCommand = null;
        private DelegateCommand _saveAsCommand = null;
        private DelegateCommand _saveAllCommand = null;
        private DelegateCommand _closeCommand = null;
        private DelegateCommand _exitCommand = null;
        private DelegateCommand _manualCommand = null;
        private DelegateCommand _informationCommand = null;

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
                    _tools = new ToolViewModelBase[] { FileExplorer, FileProperties, Report, Toolbox };
                return _tools;
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
            set
            {
                _fileExplorer = value;
                RaisePropertyChanged();
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

        public ToolboxViewModel Toolbox
        {
            get
            {
                if (_toolbox == null)
                    _toolbox = new ToolboxViewModel();

                return _toolbox;
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

        public TaskStatusType Status
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

        public string StatusString
        {
            get
            {
                return _statusString;
            }
            set
            {
                _statusString = value;
                RaisePropertyChanged();
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

        public ICommand SaveCommand
        {
            get
            {
                return (_saveCommand) ?? (_saveCommand = new DelegateCommand(OnSave));
            }
        }

        public ICommand SaveAsCommand
        {
            get
            {
                return (_saveAsCommand) ?? (_saveAsCommand = new DelegateCommand(OnSaveAs));
            }
        }

        public ICommand SaveAllCommand
        {
            get
            {
                return (_saveAllCommand) ?? (_saveAllCommand = new DelegateCommand(OnSaveAll));
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return (_closeCommand) ?? (_closeCommand = new DelegateCommand(OnClose));
            }
        }

        public ICommand ExitCommand
        {
            get
            {
                return (_exitCommand) ?? (_exitCommand = new DelegateCommand(OnExit));
            }
        }

        public ICommand ManualCommand
        {
            get
            {
                return (_manualCommand) ?? (_manualCommand = new DelegateCommand(OnClickManual));
            }
        }

        public ICommand InformationCommand
        {
            get
            {
                return (_informationCommand) ?? (_informationCommand = new DelegateCommand(OnClickInformation));
            }
        }

        #endregion

        #region ::Methods::

        internal void SetStatus(TaskStatusType type, string text)
        {
            Status = type;
            StatusString = text;
        }

        internal FileViewModelBase OpenFile(string filePath)
        {
            var fileViewModel = _files.FirstOrDefault(fm => fm.FilePath == filePath);
            if (fileViewModel != null)
                return fileViewModel;

            string extension = Path.GetExtension(filePath).ToLower();

            switch (extension)
            {
                case ".ogg":
                case ".rpgmvo":
                case ".ogg_":
                case ".m4a":
                case ".rpgmvm":
                case ".m4a_":
                case ".wav":
                case ".rpgmvw":
                case ".wav_":
                    fileViewModel = new AudioFileViewModel(filePath);
                    _files.Add(fileViewModel);
                    break;
                case ".png":
                case ".rpgmvp":
                case ".png_":
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

        internal void SaveFile(FileViewModelBase fileToSave, bool saveAsFlag = false)
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

        internal void CloseFile(FileViewModelBase fileToClose)
        {
            if (fileToClose.IsDirty)
            {
                var res = MessageBox.Show(string.Format("Save changes for file '{0}'?", fileToClose.FileName), "MView", MessageBoxButton.YesNoCancel);
                if (res == MessageBoxResult.Cancel)
                    return;

                if (res == MessageBoxResult.Yes)
                {
                    SaveFile(fileToClose);
                }
            }

            _files.Remove(fileToClose);
        }

        internal void CloseAllFiles()
        {
            List<FileViewModelBase> files = _files.ToList();
            foreach (FileViewModelBase file in files)
            {
                if (file != null)
                {
                    CloseFile(file);
                }
            }
        }

        #endregion

        #region ::Command Actions::

        private bool CanOpen(object parameter) => true;

        private void OnOpen(object parameter)
        {
            var dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.Filter = "HTML Index|index.html";

            if (dlg.ShowDialog().GetValueOrDefault())
            {
                DirectoryInfo directory = Directory.GetParent(dlg.FileName);
                DirectoryItem item = new DirectoryItem(directory, true, true);

                if (!FileExplorer.Nodes.Contains(item))
                {
                    FileExplorer.Nodes = new ObservableCollection<DirectoryItem>();
                    FileExplorer.SelectedNodes = new ObservableCollection<DirectoryItem>();
                    FileExplorer.Nodes.Add(new DirectoryItem(directory, true, true));

                    Report.AddReportWithIdentifier($"A new project has been opened.({directory.FullName})", ReportType.Information);
                }
                else
                {
                    Report.AddReportWithIdentifier($"The project is already open.({directory.FullName})", ReportType.Caution);
                }
            }
            else
            {
                Report.AddReportWithIdentifier("The OpenFileDialog has been canceled.", ReportType.Caution);
            }
        }

        private void OnSave()
        {
            if (ActiveDocument == null)
            {
                return;
            }

            SaveFile(ActiveDocument, false);
        }

        private void OnSaveAs()
        {
            if (ActiveDocument == null)
            {
                return;
            }

            SaveFile(ActiveDocument, true);
        }

        private void OnSaveAll()
        {
            foreach (FileViewModelBase file in _files)
            {
                if (file != null)
                {
                    SaveFile(file, false);
                }
            }
        }

        private void OnClose()
        {
            if (ActiveDocument == null)
            {
                return;
            }

            CloseFile(ActiveDocument);
        }

        private void OnExit()
        {
            Application.Current.Shutdown();
        }

        private void OnClickManual()
        {
            Process.Start("explorer.exe", "https://github.com/handbros/MView");
        }

        private void OnClickInformation()
        {
            Window window = new InformationWindow();
            window.ShowDialog();
        }

        #endregion
    }
}
