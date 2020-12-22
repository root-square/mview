using MView.Bases;
using MView.Entities;
using System;
using System.Collections.Generic;
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

            Workspace.Instance.FileExplorer.SelectedItemChanged += new EventHandler(OnSelectedItemChanged);
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

		#region ::SelectedItemChanged Event Subscriber::

		private async void OnSelectedItemChanged(object sender, EventArgs e)
		{
			var task = Task.Run(() =>
			{
				DirectoryItem item = Workspace.Instance.FileExplorer.SelectedItem;

				if (item != null)
				{
					if (item.Type == DirectoryItemType.File)
					{
						FileProperties = new FileProperties(item.FullName);
					}
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
