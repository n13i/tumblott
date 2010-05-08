using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Tumblott
{
    public partial class SettingsForm : Form
    {
        private bool isSaveSettings = true;

        public SettingsForm()
        {
            InitializeComponent();

            this.Closing += new CancelEventHandler(SettingsForm_Closing);

            Menu = new MainMenu();

            MenuItem miOK = new MenuItem { Text = Messages.OK };
            miOK.Click += new EventHandler(miOK_Click);
            Menu.MenuItems.Add(miOK);

            MenuItem miCancel = new MenuItem { Text = Messages.Cancel };
            miCancel.Click += new EventHandler(miCancel_Click);
            Menu.MenuItems.Add(miCancel);
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            isSaveSettings = true;

            this.Text = Messages.SettingsWindowTitle;

            // ------------------------------------------------------------------ [About] page
            Assembly asm = Assembly.GetExecutingAssembly();

            // Version
            this.versionLabel.Text = "Ver." + Utils.GetExecutingAssemblyVersion() + " (" + Utils.GetBuiltDateTime().ToString("yyyy/MM/dd") + ")";

            // Copyright
            object copyright = Attribute.GetCustomAttribute(asm, typeof(AssemblyCopyrightAttribute));
            this.copyrightLabel.Text = ((AssemblyCopyrightAttribute)copyright).Copyright;

            // icon
            this.iconPictureBox.Image = global::Tumblott.Properties.Resources.tumblott_icon;

            // ------------------------------------------------------------------ [Account] page
            this.emailTextBox.Text = Settings.Email;
            this.passwordTextBox.Text = Settings.Password;

            // Translations
            this.emailLabel.Text = Messages.EmailAddress;
            this.passwordLabel.Text = Messages.Password;
            this.accountLabel.Text = Messages.AccountSetting;

            // ------------------------------------------------------------------ [General] page
            this.openLinkCheckBox.Checked = Settings.IsConfirmWhenOpenLinks;

            // Translations
            this.openLinkCheckBox.Text = Messages.OpenLinkConfirm;

            // ------------------------------------------------------------------ [Network] page
            switch (Settings.Proxy)
            {
                case Settings.ProxyMode.Default:
                    this.useDefaultRadioButton.Checked = true;
                    this.noProxyRadioButton.Checked = false;
                    this.useProxyRadioButton.Checked = false;
                    break;
                case Settings.ProxyMode.NoUse:
                    this.useDefaultRadioButton.Checked = false;
                    this.noProxyRadioButton.Checked = true;
                    this.useProxyRadioButton.Checked = false;
                    break;
                case Settings.ProxyMode.Use:
                    this.useDefaultRadioButton.Checked = false;
                    this.noProxyRadioButton.Checked = false;
                    this.useProxyRadioButton.Checked = true;
                    break;
                default:
                    break;
            }
            this.proxyServerTextBox.Text = Settings.ProxyServer;
            this.proxyPortTextBox.Text = Settings.ProxyPort.ToString();
            this.proxyUsernameTextBox.Text = Settings.ProxyUsername;
            this.proxyPasswordTextBox.Text = Settings.ProxyPassword;

            EnableProxySettings(this.useProxyRadioButton.Checked);

            // Translations
            this.proxyLabel.Text = Messages.Proxy;
            this.proxyServerLabel.Text = Messages.Server;
            this.proxyPortLabel.Text = Messages.Port;
            this.proxyUsernameLabel.Text = Messages.Username;
            this.proxyPasswordLabel.Text = Messages.Password;
            this.proxyOnlyImagesCheckBox.Text = Messages.ProxyOnlyImages;
        }

        private void useProxyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            EnableProxySettings(this.useProxyRadioButton.Checked);
        }

        private void EnableProxySettings(bool isEnable)
        {
            this.proxyServerTextBox.Enabled = isEnable;
            this.proxyPortTextBox.Enabled = isEnable;
            this.proxyUsernameTextBox.Enabled = isEnable;
            this.proxyPasswordTextBox.Enabled = isEnable;
        }

        void SettingsForm_Closing(object sender, CancelEventArgs e)
        {
            if (isSaveSettings)
            {
                // タスクバーの [OK] で閉じるとき / ソフトキーの OK で閉じるとき

                Settings.Email = this.emailTextBox.Text;
                Settings.Password = this.passwordTextBox.Text;

                Settings.IsConfirmWhenOpenLinks = this.openLinkCheckBox.Checked;

                if (this.useDefaultRadioButton.Checked)
                {
                    Settings.Proxy = Settings.ProxyMode.Default;
                }
                else if (this.noProxyRadioButton.Checked)
                {
                    Settings.Proxy = Settings.ProxyMode.NoUse;
                }
                else if (this.useProxyRadioButton.Checked)
                {
                    Settings.Proxy = Settings.ProxyMode.Use;
                }

                Settings.ProxyServer = this.proxyServerTextBox.Text;
                Settings.ProxyPort = int.Parse(this.proxyPortTextBox.Text);
                Settings.ProxyUsername = this.proxyUsernameTextBox.Text;
                Settings.ProxyPassword = this.proxyPasswordTextBox.Text;

                Settings.Save();
            }
        }

        void miCancel_Click(object sender, EventArgs e)
        {
            isSaveSettings = false;
            this.Close();
        }

        private void miOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void debugButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Settings.AppDataPath);
        }
    }
}
