using AvalonDock.Layout;
using System.Windows;
using System.Windows.Controls;

namespace MView.Docker
{
	public class PanesTemplateSelector : DataTemplateSelector
	{
		public PanesTemplateSelector()
		{

		}


		public DataTemplate FileViewTemplate
		{
			get;
			set;
		}

		public DataTemplate FileStatsViewTemplate
		{
			get;
			set;
		}

		public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
		{
			var itemAsLayoutContent = item as LayoutContent;

			//if (item is FileViewModel)
				return FileViewTemplate;

			//if (item is FileStatsViewModel)
				return FileStatsViewTemplate;

			return base.SelectTemplate(item, container);
		}
	}
}
