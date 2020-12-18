using AvalonDock.Layout;
using MView.ViewModels.Data;
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


		public DataTemplate AudioDataViewTemplate
		{
			get;
			set;
		}

		public DataTemplate GeneralDataViewTemplate
		{
			get;
			set;
		}

		public DataTemplate ImageDataViewTemplate
		{
			get;
			set;
		}

		public DataTemplate JsonDataViewTemplate
		{
			get;
			set;
		}

		public DataTemplate SaveDataViewTemplate
		{
			get;
			set;
		}

		public DataTemplate ScriptDataViewTemplate
		{
			get;
			set;
		}

		public DataTemplate CryptographyManagerViewTemplate
		{
			get;
			set;
		}

		public DataTemplate DataManagerViewTemplate
		{
			get;
			set;
		}

		public DataTemplate FileAttributesViewTemplate
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

		public DataTemplate SaveDataManagerViewTemplate
		{
			get;
			set;
		}

		public DataTemplate ScriptManagerViewTemplate
		{
			get;
			set;
		}

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var itemAsLayoutContent = item as LayoutContent;

			if (item is AudioDataViewModel)
				return AudioDataViewTemplate;

			if (item is GeneralDataViewModel)
				return GeneralDataViewTemplate;

			if (item is ImageDataViewModel)
				return ImageDataViewTemplate;

			if (item is JsonDataViewModel)
				return JsonDataViewTemplate;

			if (item is SaveDataViewModel)
				return SaveDataViewTemplate;

			if (item is ScriptDataViewModel)
				return ScriptDataViewTemplate;

			if (item is CryptographyManagerViewModel)
				return CryptographyManagerViewTemplate;

			if (item is DataManagerViewModel)
				return DataManagerViewTemplate;

			if (item is FileAttributesViewModel)
				return FileAttributesViewTemplate;

			if (item is FileExplorerViewModel)
				return FileExplorerViewTemplate;

			if (item is ReportViewModel)
				return ReportViewTemplate;

			if (item is SaveDataManagerViewModel)
				return SaveDataManagerViewTemplate;

			if (item is ScriptManagerViewModel)
				return ScriptManagerViewTemplate;

			return base.SelectTemplate(item, container);
		}
	}
}
