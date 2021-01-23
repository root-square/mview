using MView.Bases;
using MView.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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

            Workspace.Instance.ActiveDocumentChanged += OnActiveDocumentChanged;
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

		#region ::CollectionChanged Event Subscriber::

		private async void OnActiveDocumentChanged(object sender, EventArgs e)
		{
			var task = Task.Run(() =>
			{
				if (Workspace.Instance.ActiveDocument != null)
				{
                    FileProperties = new FileProperties(Workspace.Instance.ActiveDocument.FilePath);
                }
				else
				{
					FileProperties = new FileProperties();
				}
			});

			await task;
		}

		#endregion
	}
}
