using MView.Bases;
using MView.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MView.ViewModels.Tool
{
    public class FilePropertiesViewModel : ToolViewModelBase
    {
        #region ::Fields::

        public const string ToolContentId = "FileProperties";

		public FileProperties _fileProperties = new FileProperties();

        #endregion

        #region ::Constructors::

        public FilePropertiesViewModel() : base("File Properties")
        {
            ContentId = ToolContentId;

            Workspace.Instance.ActiveDocumentChanged += new EventHandler(OnActiveDocumentChanged);
        }

		#endregion

		#region ::Properties::

		public FileProperties FileProperties
        {
            get
            {
				return _fileProperties;
            }
            set
            {
				_fileProperties = value;
				RaisePropertyChanged();
            }
        }

		#endregion

		#region ::ActiveDocumentChanged Event Subscriber::

		private void OnActiveDocumentChanged(object sender, EventArgs e)
		{
			if (Workspace.Instance.ActiveDocument != null && Workspace.Instance.ActiveDocument.FilePath != null && System.IO.File.Exists(Workspace.Instance.ActiveDocument.FilePath))
			{
				FileProperties = new FileProperties(Workspace.Instance.ActiveDocument.FilePath);
			}
			else
			{
				FileProperties = new FileProperties();
			}
		}

		#endregion
	}
}
