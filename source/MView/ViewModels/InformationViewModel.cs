using MView.Bases;
using MView.Commands;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;

namespace MView.ViewModels
{
    public class InformationViewModel : ViewModelBase
    {
        #region ::Fields::

        private string _versionText = string.Empty;

        private ICommand _gitHubRepositoryCommand;
        private ICommand _opensourceLicensesCommand;

        #endregion

        #region ::Constructors::

        public InformationViewModel()
        {
            VersionText = $"Version {Assembly.GetExecutingAssembly().GetName().Version}";
        }

        #endregion

        #region ::Properties::

        public string VersionText
        {
            get
            {
                return _versionText;
            }
            set
            {
                _versionText = value;
                RaisePropertyChanged();
            }
        }

        public ICommand GitHubRepositoryCommand
        {
            get
            {
                return (_gitHubRepositoryCommand) ?? (_gitHubRepositoryCommand = new DelegateCommand(GitHubRepository));
            }
        }

        public ICommand OpensourceLicensesCommand
        {
            get
            {
                return (_opensourceLicensesCommand) ?? (_opensourceLicensesCommand = new DelegateCommand(OpensourceLicenses));
            }
        }

        #endregion

        #region ::Command Actions::

        public void GitHubRepository()
        {
            Process.Start("explorer.exe", "https://github.com/handbros/MView");
        }

        public void OpensourceLicenses()
        {
            Process.Start("explorer.exe", "https://github.com/handbros/MView/blob/master/OPENSOURCES.md");
        }

        #endregion
    }
}
