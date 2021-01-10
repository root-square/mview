using MView.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MView.Windows
{
    /// <summary>
    /// ToolHostWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ToolHostWithExplorerWindow : HandyControl.Controls.Window
    {
        public ToolHostWithExplorerWindow(Page toolPage, double width = 600, double height = 700)
        {
            ToolHostWithExplorerViewModel.Instance = new ToolHostWithExplorerViewModel(toolPage, width, height);
            this.DataContext = ToolHostWithExplorerViewModel.Instance;
            InitializeComponent();
        }
    }
}
