using MView.Bases;
using MView.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MView.ViewModels.Tool
{
    public class ReportViewModel : ToolViewModelBase
    {
        #region ::Fields::

        public const string ToolContentId = "Report";

        private string _report = $"MView Version {Assembly.GetExecutingAssembly().GetName().Version}\r\n";

        #endregion

        #region ::Constructors::

        public ReportViewModel() : base("Report")
        {
            ContentId = ToolContentId;
        }

        #endregion

        #region ::Properties::

        public string Report
        {
            get => _report;
            set
            {
                _report = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ::Methods::

        public void AddReport(string text)
        {
            Report += $"{text}\r\n";
        }

        public void AddReportWithIdentifier(string text, ReportType type = ReportType.Information)
        {
            Report += $"{DateTime.Now.ToString("HH:mm:ss")} [{type}] {text}\r\n";
        }

        #endregion
    }
}
