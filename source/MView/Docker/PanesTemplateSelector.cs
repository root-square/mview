using AvalonDock.Layout;
using MView.ViewModels.File;
using MView.ViewModels.Tool;
using System.Windows;
using System.Windows.Controls;

namespace MView.Docker
{
	public class PanesTemplateSelector : DataTemplateSelector
	{
		public PanesTemplateSelector()
		{

		}


		public DataTemplate AudioFileViewTemplate
		{
			get;
			set;
		}

		public DataTemplate GeneralFileViewTemplate
		{
			get;
			set;
		}

		public DataTemplate ImageFileViewTemplate
		{
			get;
			set;
		}

		public DataTemplate JsonFileViewTemplate
		{
			get;
			set;
		}

		public DataTemplate SaveFileViewTemplate
		{
			get;
			set;
		}

		public DataTemplate ScriptFileViewTemplate
		{
			get;
			set;
		}
		public DataTemplate FilePropertiesViewTemplate
		{
			get;
			set;
		}

		public DataTemplate FileExplorerViewTemplate
		{
			get;
			set;
		}

		public DataTemplate ReportViewTemplate
		{
			get;
			set;
		}

		public DataTemplate ToolboxViewTemplate
		{
			get;
			set;
		}

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var itemAsLayoutContent = item as LayoutContent;

			if (item is AudioFileViewModel)
				return AudioFileViewTemplate;

			if (item is GeneralFileViewModel)
				return GeneralFileViewTemplate;

			if (item is ImageFileViewModel)
				return ImageFileViewTemplate;

			if (item is JsonFileViewModel)
				return JsonFileViewTemplate;

			if (item is SaveFileViewModel)
				return SaveFileViewTemplate;

			if (item is ScriptFileViewModel)
				return ScriptFileViewTemplate;

			if (item is FilePropertiesViewModel)
				return FilePropertiesViewTemplate;

			if (item is FileExplorerViewModel)
				return FileExplorerViewTemplate;

			if (item is ReportViewModel)
				return ReportViewTemplate;

			if (item is ToolboxViewModel)
				return ToolboxViewTemplate;

			return base.SelectTemplate(item, container);
		}
	}
}
