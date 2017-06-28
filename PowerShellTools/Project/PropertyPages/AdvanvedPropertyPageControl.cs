using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualStudioTools.Project;

namespace PowerShellTools.Project.PropertyPages
{
    public partial class AdvanvedPropertyPageControl : PropertyPageUserControl
    {
        public AdvanvedPropertyPageControl(CommonPropertyPage propertyPage) : base(propertyPage)
        {
            InitializeComponent();
        }

        private void AdvanvedPropertyPageControl_Load(object sender, System.EventArgs e)
        {
            var link = new LinkLabel.Link(0, 17);
            link.LinkData = "https://poshtools.com/powershell-pro-tools-for-visual-studio/";
            linkLabel1.Links.Add(link);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }
    }
}
