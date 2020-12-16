using MahApps.Metro.IconPacks;
using MView.Bases;
using MView.Commands;
using MView.Entities;
using MView.Windows;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MView.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region ::Fields::

        private History _history = History.Instance;

        private bool _windowVisibility = true;

        private ObservableCollection<TaskRecordGroup> _taskHistory;

        #endregion

        #region ::Constructors::

        public MainViewModel()
        {
            _taskHistory = new ObservableCollection<TaskRecordGroup>();
            RefreshTaskHistoryItems();
        }

        #endregion

        #region ::Properties::

        public bool WindowVisibility
        {
            get
            {
                return _windowVisibility;
            }
            set
            {
                _windowVisibility = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<TaskRecordGroup> TaskHistory
        {
            get
            {
                return _taskHistory;
            }
            set
            {
                _taskHistory = value;
                RefreshTaskHistoryItems();
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ::Methods::

        private void RefreshTaskHistoryItems()
        {
            TaskRecordGroup today = new TaskRecordGroup("Today");

            TaskRecordGroup yesterday = new TaskRecordGroup("Yesterday");

            TaskRecordGroup lastWeek = new TaskRecordGroup("Last week");

            TaskRecordGroup lastMonth = new TaskRecordGroup("Last month");

            TaskRecordGroup old = new TaskRecordGroup("Old");

            DateTime currentTime = DateTime.Now;

            foreach (TaskRecord record in _history.TaskRecordList)
            {
                Debug.WriteLine("Record Catched.");

                TimeSpan timeDifference = currentTime - record.DateTime;
                if (timeDifference.Days < 1)
                {
                    today.Group.Add(record);
                    Debug.WriteLine("Input Today.");
                }
                else if (timeDifference.Days == 1)
                {
                    yesterday.Group.Add(record);
                    Debug.WriteLine("Input Yesterday.");
                }
                else if (timeDifference.Days > 7)
                {
                    lastWeek.Group.Add(record);
                }
                else if (currentTime.Month-1 == record.DateTime.Month)
                {
                    lastMonth.Group.Add(record);
                }
                else
                {
                    old.Group.Add(record);
                }
            }

            if (today.Group.Count > 0)
            {
                _taskHistory.Add(today);
            }

            if (yesterday.Group.Count > 0)
            {
                _taskHistory.Add(yesterday);
            }

            if (lastWeek.Group.Count > 0)
            {
                _taskHistory.Add(lastWeek);
            }

            if (lastMonth.Group.Count > 0)
            {
                _taskHistory.Add(lastMonth);
            }

            if (old.Group.Count > 0)
            {
                _taskHistory.Add(old);
            }
        }

        #endregion

        #region ::Commands::

        private ICommand _workspaceCommand;

        public ICommand WorkspaceCommand
        {
            get
            {
                return (_workspaceCommand) ?? (_workspaceCommand = new DelegateCommand(Workspace));
            }
        }

        private ICommand _toolboxCommand;

        public ICommand ToolboxCommand
        {
            get
            {
                return (_toolboxCommand) ?? (_toolboxCommand = new DelegateCommand(Toolbox));
            }
        }

        private ICommand _settingsCommand;

        public ICommand SettingsCommand
        {
            get
            {
                return (_settingsCommand) ?? (_settingsCommand = new DelegateCommand(Settings));
            }
        }

        private ICommand _informationCommand;

        public ICommand InformationCommand
        {
            get
            {
                return (_informationCommand) ?? (_informationCommand = new DelegateCommand(Information));
            }
        }

        #endregion

        #region ::Command Actions::

        public void Workspace()
        {
            Window window = new WorkspaceWindow();
            WindowVisibility = false;
            window.Show();
            WindowVisibility = true;
        }

        public void Toolbox()
        {
            Window window = new ToolboxWindow();
            WindowVisibility = false;
            window.Show();
            WindowVisibility = true;
        }

        public void Settings()
        {
            Window window = new SettingsWindow();
            WindowVisibility = false;
            window.ShowDialog();
            WindowVisibility = true;
        }

        public void Information()
        {
            Window window = new InformationWindow();
            WindowVisibility = false;
            window.Show();
            WindowVisibility = true;
        }

        #endregion
    }
}
