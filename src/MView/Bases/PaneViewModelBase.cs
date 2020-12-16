using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace MView.Bases
{
	public class PaneViewModelBase : ViewModelBase
	{
		#region Fields

		private string _title = null;
		private string _contentId = null;
		private bool _isSelected = false;
		private bool _isActive = false;

		#endregion

		#region Constructors

		public PaneViewModelBase()
		{

		}

		#endregion

		#region Properties

		public string Title
		{
			get => _title;
			set
			{
				if (_title != value)
				{
					_title = value;
					RaisePropertyChanged();
				}
			}
		}

		public ImageSource IconSource { get; protected set; }

		public string ContentId
		{
			get => _contentId;
			set
			{
				if (_contentId != value)
				{
					_contentId = value;
					RaisePropertyChanged();
				}
			}
		}

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					RaisePropertyChanged();
				}
			}
		}

		public bool IsActive
		{
			get => _isActive;
			set
			{
				if (_isActive != value)
				{
					_isActive = value;
					RaisePropertyChanged();
				}
			}
		}

		#endregion
	}
}
