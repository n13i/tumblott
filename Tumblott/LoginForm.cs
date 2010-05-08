using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Tumblott
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            Menu = new MainMenu();

            MenuItem miOK = new MenuItem { Text = Messages.OK };
            miOK.Click += new EventHandler(miOK_Click);
            Menu.MenuItems.Add(miOK);

            MenuItem miExit = new MenuItem { Text = Messages.Exit };
            miExit.Click += new EventHandler(miExit_Click);
            Menu.MenuItems.Add(miExit);
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.emailTextBox.Text = Settings.Email;
            this.passwordTextBox.Text = Settings.Password;
            this.autoLoginCheckBox.Checked = Settings.IsAutoLogin;
        }

        void miOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void miExit_Click(object sender, EventArgs e)
        {
            // FIXME メインフォームをCloseするべき
            // sa: http://msdn.microsoft.com/ja-jp/windowsmobile/cc825305.aspx#4.11
            Application.Exit();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            Settings.Email = this.emailTextBox.Text;
            Settings.Password = this.passwordTextBox.Text;
            Settings.IsAutoLogin = this.autoLoginCheckBox.Checked;
            Settings.Save();
        }
    }
}
