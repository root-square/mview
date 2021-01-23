using System;
using System.Collections.Generic;
using System.Text;

namespace MView.Bases
{
	public class ToolViewModelBase : PaneViewModelBase
	{
		#region ::Fields::

		private bool _isVisible = true;

		#endregion

		#region ::Constructor::

		public ToolViewModelBase(string name)
		{
			Name = name;
			Title = name;
		}

		#endregion

		#region ::Properties::

		public string Name { get; private set; }

		public bool IsVisible
		{
			get => _isVisible;
			set
			{
				if (_isVisible != value)
				{
					_isVisible = value;
					RaisePropertyChanged(nameof(IsVisible));
				}
			}
		}

		#endregion
	}
}
