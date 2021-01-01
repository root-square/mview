using MView.ViewModels.ToolPage;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MView.Pages
{
    /// <summary>
    /// ResourceEncrypterPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ResourceEncrypterPage : Page
    {
        public ResourceEncrypterPage()
        {
            InitializeComponent();
            this.DataContext = new ResourceEncrypterViewModel();
        }
    }
}
