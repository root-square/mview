using System.Diagnostics;
using System.Windows.Forms;

namespace MView.Forms
{
    public partial class InformationForm : Form
    {
        public InformationForm()
        {
            InitializeComponent();
        }

        private void repositoryLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/junimiso04/MView");
        }

        private void ossLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://raw.githubusercontent.com/junimiso04/MView/master/OPENSOURCES.md");
        }
    }
}
