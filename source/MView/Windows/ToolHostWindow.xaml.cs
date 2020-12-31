using MView.ViewModels;
using System;
using System.Collections.Generic;
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
    public partial class ToolHostWindow : HandyControl.Controls.Window
    {
        public ToolHostWindow(Page toolPage)
        {
            InitializeComponent();
            ToolHostViewModel.Instance = new ToolHostViewModel(toolPage);
            this.DataContext = ToolHostViewModel.Instance;
        }
    }
}
