using MView.Bases;
using MView.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MView.ViewModels.File
{
    public class ScriptFileViewModel : FileViewModelBase
    {
        #region ::Fields::

        private FileProperties _fileProperties = new FileProperties();

        #endregion

        #region ::Constructors::

        public ScriptFileViewModel(string filePath) : base(filePath)
        {
            Initialize(filePath);
        }

        #endregion

        #region ::Properties::

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
