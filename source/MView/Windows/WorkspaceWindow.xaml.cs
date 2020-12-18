using AvalonDock.Layout.Serialization;
using MView.Commands;
using MView.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MView.Windows
{
    /// <summary>
    /// WorkspaceWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WorkspaceWindow : HandyControl.Controls.Window
    {
        public WorkspaceWindow()
        {
            InitializeComponent();
            this.DataContext = Workspace.Instance;

            this.Loaded += new RoutedEventHandler(WorkspaceLoaded);
            this.Unloaded += new RoutedEventHandler(WorkspaceUnloaded);
        }

		#region ::Layout Loader/Unloader::

		private void WorkspaceLoaded(object sender, RoutedEventArgs e)
        {
            // Load layout.
            var serializer = new XmlLayoutSerializer(dockManager);
            serializer.LayoutSerializationCallback += (s, args) =>
            {
                args.Content = args.Content;
            };

            string layoutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Settings.LayoutPath);

            if (!File.Exists(layoutPath))
            {
                FileManager.WriteTextFile(layoutPath, MView.Resources.avalondock_layout, Encoding.UTF8);
            }

            serializer.Deserialize(layoutPath);
        }

        private void WorkspaceUnloaded(object sender, RoutedEventArgs e)
        {
            // Save layout.
            string layoutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Settings.LayoutPath);

            if (!Directory.Exists(Path.GetDirectoryName(layoutPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(layoutPath));
            }

            var serializer = new XmlLayoutSerializer(dockManager);
            serializer.Serialize(layoutPath);
        }

		#endregion
	}
}
