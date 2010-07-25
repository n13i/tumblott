using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Tumblott.Client.Tumblr;

namespace Tumblott
{
    public partial class LicenseForm : Form
    {
        private TumblrPost post;

        public LicenseForm()
        {
            InitializeComponent();

            this.Menu = new MainMenu();

            MenuItem miClose = new MenuItem { Text = Messages.Close };
            miClose.Click +=new EventHandler(miClose_Click);
            this.Menu.MenuItems.Add(miClose);

            this.post = new TumblrPost();
            this.post.Html = "<html>" + global::Tumblott.Properties.Resources.license.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\n", "<br/>") + "</html>";
        }


        void miClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LicenseForm_Load(object sender, EventArgs e)
        {
            this.postView1.Post = this.post;
        }
    }
}
