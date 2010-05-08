namespace Tumblott
{
    partial class SettingsForm
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabAccountPage = new System.Windows.Forms.TabPage();
            this.accountLabel = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.emailTextBox = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.emailLabel = new System.Windows.Forms.Label();
            this.tabGeneralPage = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.openLinkCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabNetworkPage = new System.Windows.Forms.TabPage();
            this.proxyOnlyImagesCheckBox = new System.Windows.Forms.CheckBox();
            this.proxyPasswordTextBox = new System.Windows.Forms.TextBox();
            this.proxyPasswordLabel = new System.Windows.Forms.Label();
            this.proxyUsernameTextBox = new System.Windows.Forms.TextBox();
            this.proxyUsernameLabel = new System.Windows.Forms.Label();
            this.proxyPortTextBox = new System.Windows.Forms.TextBox();
            this.proxyPortLabel = new System.Windows.Forms.Label();
            this.proxyServerTextBox = new System.Windows.Forms.TextBox();
            this.proxyServerLabel = new System.Windows.Forms.Label();
            this.proxyLabel = new System.Windows.Forms.Label();
            this.useProxyRadioButton = new System.Windows.Forms.RadioButton();
            this.noProxyRadioButton = new System.Windows.Forms.RadioButton();
            this.useDefaultRadioButton = new System.Windows.Forms.RadioButton();
            this.tabAboutPage = new System.Windows.Forms.TabPage();
            this.iconPictureBox = new System.Windows.Forms.PictureBox();
            this.debugButton = new System.Windows.Forms.Button();
            this.licenseLabel = new System.Windows.Forms.Label();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.webLinkLabel = new System.Windows.Forms.LinkLabel();
            this.projectLinkLabel = new System.Windows.Forms.LinkLabel();
            this.versionLabel = new System.Windows.Forms.Label();
            this.nameLabel = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabAccountPage.SuspendLayout();
            this.tabGeneralPage.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.tabNetworkPage.SuspendLayout();
            this.tabAboutPage.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.iconPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabAccountPage);
            this.tabControl.Controls.Add(this.tabGeneralPage);
            this.tabControl.Controls.Add(this.tabNetworkPage);
            this.tabControl.Controls.Add(this.tabAboutPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(240, 268);
            this.tabControl.TabIndex = 0;
            // 
            // tabAccountPage
            // 
            this.tabAccountPage.AutoScroll = true;
            this.tabAccountPage.Controls.Add(this.accountLabel);
            this.tabAccountPage.Controls.Add(this.passwordTextBox);
            this.tabAccountPage.Controls.Add(this.emailTextBox);
            this.tabAccountPage.Controls.Add(this.passwordLabel);
            this.tabAccountPage.Controls.Add(this.emailLabel);
            this.tabAccountPage.Location = new System.Drawing.Point(4, 21);
            this.tabAccountPage.Name = "tabAccountPage";
            this.tabAccountPage.Size = new System.Drawing.Size(232, 243);
            this.tabAccountPage.TabIndex = 3;
            this.tabAccountPage.Text = global::Tumblott.Messages.Account;
            // 
            // accountLabel
            // 
            this.accountLabel.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.accountLabel.Location = new System.Drawing.Point(4, 4);
            this.accountLabel.Name = "accountLabel";
            this.accountLabel.Size = new System.Drawing.Size(226, 32);
            this.accountLabel.TabIndex = 5;
            this.accountLabel.Text = "Please input your Tumblr account information.";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(18, 111);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Size = new System.Drawing.Size(192, 19);
            this.passwordTextBox.TabIndex = 4;
            // 
            // emailTextBox
            // 
            this.emailTextBox.Location = new System.Drawing.Point(18, 59);
            this.emailTextBox.Name = "emailTextBox";
            this.emailTextBox.Size = new System.Drawing.Size(192, 19);
            this.emailTextBox.TabIndex = 3;
            // 
            // passwordLabel
            // 
            this.passwordLabel.Location = new System.Drawing.Point(8, 92);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(100, 16);
            this.passwordLabel.TabIndex = 2;
            this.passwordLabel.Text = "Password";
            // 
            // emailLabel
            // 
            this.emailLabel.Location = new System.Drawing.Point(8, 40);
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size(100, 16);
            this.emailLabel.TabIndex = 1;
            this.emailLabel.Text = "Email address";
            // 
            // tabGeneralPage
            // 
            this.tabGeneralPage.AutoScroll = true;
            this.tabGeneralPage.Controls.Add(this.label2);
            this.tabGeneralPage.Controls.Add(this.numericUpDown1);
            this.tabGeneralPage.Controls.Add(this.openLinkCheckBox);
            this.tabGeneralPage.Controls.Add(this.label1);
            this.tabGeneralPage.Location = new System.Drawing.Point(4, 21);
            this.tabGeneralPage.Name = "tabGeneralPage";
            this.tabGeneralPage.Size = new System.Drawing.Size(232, 243);
            this.tabGeneralPage.TabIndex = 0;
            this.tabGeneralPage.Text = global::Tumblott.Messages.General;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(157, 19);
            this.label2.TabIndex = 3;
            this.label2.Text = "Posts are loaded at once";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Enabled = false;
            this.numericUpDown1.Location = new System.Drawing.Point(172, 49);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(48, 19);
            this.numericUpDown1.TabIndex = 2;
            // 
            // openLinkCheckBox
            // 
            this.openLinkCheckBox.Location = new System.Drawing.Point(8, 27);
            this.openLinkCheckBox.Name = "openLinkCheckBox";
            this.openLinkCheckBox.Size = new System.Drawing.Size(200, 16);
            this.openLinkCheckBox.TabIndex = 1;
            this.openLinkCheckBox.Text = "Always confirm when open links";
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(216, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Under Construction";
            // 
            // tabNetworkPage
            // 
            this.tabNetworkPage.AutoScroll = true;
            this.tabNetworkPage.Controls.Add(this.proxyOnlyImagesCheckBox);
            this.tabNetworkPage.Controls.Add(this.proxyPasswordTextBox);
            this.tabNetworkPage.Controls.Add(this.proxyPasswordLabel);
            this.tabNetworkPage.Controls.Add(this.proxyUsernameTextBox);
            this.tabNetworkPage.Controls.Add(this.proxyUsernameLabel);
            this.tabNetworkPage.Controls.Add(this.proxyPortTextBox);
            this.tabNetworkPage.Controls.Add(this.proxyPortLabel);
            this.tabNetworkPage.Controls.Add(this.proxyServerTextBox);
            this.tabNetworkPage.Controls.Add(this.proxyServerLabel);
            this.tabNetworkPage.Controls.Add(this.proxyLabel);
            this.tabNetworkPage.Controls.Add(this.useProxyRadioButton);
            this.tabNetworkPage.Controls.Add(this.noProxyRadioButton);
            this.tabNetworkPage.Controls.Add(this.useDefaultRadioButton);
            this.tabNetworkPage.Location = new System.Drawing.Point(4, 21);
            this.tabNetworkPage.Name = "tabNetworkPage";
            this.tabNetworkPage.Size = new System.Drawing.Size(232, 243);
            this.tabNetworkPage.TabIndex = 2;
            this.tabNetworkPage.Text = global::Tumblott.Messages.Network;
            // 
            // proxyOnlyImagesCheckBox
            // 
            this.proxyOnlyImagesCheckBox.Enabled = false;
            this.proxyOnlyImagesCheckBox.Location = new System.Drawing.Point(8, 23);
            this.proxyOnlyImagesCheckBox.Name = "proxyOnlyImagesCheckBox";
            this.proxyOnlyImagesCheckBox.Size = new System.Drawing.Size(216, 28);
            this.proxyOnlyImagesCheckBox.TabIndex = 12;
            this.proxyOnlyImagesCheckBox.Text = "Use proxy only for loading images\r\n(Useful with an image compression proxy)";
            // 
            // proxyPasswordTextBox
            // 
            this.proxyPasswordTextBox.Location = new System.Drawing.Point(86, 196);
            this.proxyPasswordTextBox.MaxLength = 1024;
            this.proxyPasswordTextBox.Name = "proxyPasswordTextBox";
            this.proxyPasswordTextBox.PasswordChar = '*';
            this.proxyPasswordTextBox.Size = new System.Drawing.Size(138, 19);
            this.proxyPasswordTextBox.TabIndex = 11;
            // 
            // proxyPasswordLabel
            // 
            this.proxyPasswordLabel.Location = new System.Drawing.Point(16, 199);
            this.proxyPasswordLabel.Name = "proxyPasswordLabel";
            this.proxyPasswordLabel.Size = new System.Drawing.Size(64, 16);
            this.proxyPasswordLabel.TabIndex = 10;
            this.proxyPasswordLabel.Text = "Password";
            // 
            // proxyUsernameTextBox
            // 
            this.proxyUsernameTextBox.Location = new System.Drawing.Point(86, 171);
            this.proxyUsernameTextBox.MaxLength = 1024;
            this.proxyUsernameTextBox.Name = "proxyUsernameTextBox";
            this.proxyUsernameTextBox.Size = new System.Drawing.Size(138, 19);
            this.proxyUsernameTextBox.TabIndex = 9;
            // 
            // proxyUsernameLabel
            // 
            this.proxyUsernameLabel.Location = new System.Drawing.Point(16, 174);
            this.proxyUsernameLabel.Name = "proxyUsernameLabel";
            this.proxyUsernameLabel.Size = new System.Drawing.Size(64, 16);
            this.proxyUsernameLabel.TabIndex = 8;
            this.proxyUsernameLabel.Text = "Username";
            // 
            // proxyPortTextBox
            // 
            this.proxyPortTextBox.Location = new System.Drawing.Point(86, 146);
            this.proxyPortTextBox.MaxLength = 5;
            this.proxyPortTextBox.Name = "proxyPortTextBox";
            this.proxyPortTextBox.Size = new System.Drawing.Size(38, 19);
            this.proxyPortTextBox.TabIndex = 7;
            // 
            // proxyPortLabel
            // 
            this.proxyPortLabel.Location = new System.Drawing.Point(16, 149);
            this.proxyPortLabel.Name = "proxyPortLabel";
            this.proxyPortLabel.Size = new System.Drawing.Size(64, 16);
            this.proxyPortLabel.TabIndex = 6;
            this.proxyPortLabel.Text = "Port";
            // 
            // proxyServerTextBox
            // 
            this.proxyServerTextBox.Location = new System.Drawing.Point(86, 121);
            this.proxyServerTextBox.MaxLength = 512;
            this.proxyServerTextBox.Name = "proxyServerTextBox";
            this.proxyServerTextBox.Size = new System.Drawing.Size(138, 19);
            this.proxyServerTextBox.TabIndex = 5;
            // 
            // proxyServerLabel
            // 
            this.proxyServerLabel.Location = new System.Drawing.Point(16, 124);
            this.proxyServerLabel.Name = "proxyServerLabel";
            this.proxyServerLabel.Size = new System.Drawing.Size(64, 16);
            this.proxyServerLabel.TabIndex = 4;
            this.proxyServerLabel.Text = "Server";
            // 
            // proxyLabel
            // 
            this.proxyLabel.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.proxyLabel.Location = new System.Drawing.Point(4, 4);
            this.proxyLabel.Name = "proxyLabel";
            this.proxyLabel.Size = new System.Drawing.Size(100, 16);
            this.proxyLabel.TabIndex = 0;
            this.proxyLabel.Text = "Proxy";
            // 
            // useProxyRadioButton
            // 
            this.useProxyRadioButton.Location = new System.Drawing.Point(8, 101);
            this.useProxyRadioButton.Name = "useProxyRadioButton";
            this.useProxyRadioButton.Size = new System.Drawing.Size(200, 16);
            this.useProxyRadioButton.TabIndex = 3;
            this.useProxyRadioButton.Text = global::Tumblott.Messages.UseTheProxyBelow;
            this.useProxyRadioButton.CheckedChanged += new System.EventHandler(this.useProxyRadioButton_CheckedChanged);
            // 
            // noProxyRadioButton
            // 
            this.noProxyRadioButton.Location = new System.Drawing.Point(8, 79);
            this.noProxyRadioButton.Name = "noProxyRadioButton";
            this.noProxyRadioButton.Size = new System.Drawing.Size(200, 16);
            this.noProxyRadioButton.TabIndex = 2;
            this.noProxyRadioButton.Text = global::Tumblott.Messages.DoNotUseTheProxy;
            // 
            // useDefaultRadioButton
            // 
            this.useDefaultRadioButton.Checked = true;
            this.useDefaultRadioButton.Location = new System.Drawing.Point(8, 57);
            this.useDefaultRadioButton.Name = "useDefaultRadioButton";
            this.useDefaultRadioButton.Size = new System.Drawing.Size(200, 16);
            this.useDefaultRadioButton.TabIndex = 1;
            this.useDefaultRadioButton.TabStop = true;
            this.useDefaultRadioButton.Text = global::Tumblott.Messages.UseDefaultSettings;
            // 
            // tabAboutPage
            // 
            this.tabAboutPage.AutoScroll = true;
            this.tabAboutPage.Controls.Add(this.iconPictureBox);
            this.tabAboutPage.Controls.Add(this.debugButton);
            this.tabAboutPage.Controls.Add(this.licenseLabel);
            this.tabAboutPage.Controls.Add(this.copyrightLabel);
            this.tabAboutPage.Controls.Add(this.webLinkLabel);
            this.tabAboutPage.Controls.Add(this.projectLinkLabel);
            this.tabAboutPage.Controls.Add(this.versionLabel);
            this.tabAboutPage.Controls.Add(this.nameLabel);
            this.tabAboutPage.Location = new System.Drawing.Point(4, 21);
            this.tabAboutPage.Name = "tabAboutPage";
            this.tabAboutPage.Size = new System.Drawing.Size(232, 243);
            this.tabAboutPage.TabIndex = 1;
            this.tabAboutPage.Text = global::Tumblott.Messages.About;
            // 
            // iconPictureBox
            // 
            this.iconPictureBox.Location = new System.Drawing.Point(8, 8);
            this.iconPictureBox.Name = "iconPictureBox";
            this.iconPictureBox.Size = new System.Drawing.Size(45, 45);
            this.iconPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.iconPictureBox.TabIndex = 7;
            this.iconPictureBox.TabStop = false;
            // 
            // debugButton
            // 
            this.debugButton.Location = new System.Drawing.Point(74, 197);
            this.debugButton.Name = "debugButton";
            this.debugButton.Size = new System.Drawing.Size(80, 24);
            this.debugButton.TabIndex = 6;
            this.debugButton.Text = "押すな";
            this.debugButton.Click += new System.EventHandler(this.debugButton_Click);
            // 
            // licenseLabel
            // 
            this.licenseLabel.Location = new System.Drawing.Point(8, 100);
            this.licenseLabel.Name = "licenseLabel";
            this.licenseLabel.Size = new System.Drawing.Size(216, 39);
            this.licenseLabel.TabIndex = 3;
            this.licenseLabel.Text = "This software is released under the MIT license.";
            // 
            // copyrightLabel
            // 
            this.copyrightLabel.Location = new System.Drawing.Point(8, 62);
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Size = new System.Drawing.Size(216, 32);
            this.copyrightLabel.TabIndex = 2;
            this.copyrightLabel.Text = "Copyright here";
            // 
            // webLinkLabel
            // 
            this.webLinkLabel.Location = new System.Drawing.Point(8, 148);
            this.webLinkLabel.Name = "webLinkLabel";
            this.webLinkLabel.Size = new System.Drawing.Size(187, 16);
            this.webLinkLabel.TabIndex = 4;
            this.webLinkLabel.TabStop = true;
            this.webLinkLabel.Text = "http://labs.m2hq.net/Tumblott/";
            // 
            // projectLinkLabel
            // 
            this.projectLinkLabel.Location = new System.Drawing.Point(8, 164);
            this.projectLinkLabel.Name = "projectLinkLabel";
            this.projectLinkLabel.Size = new System.Drawing.Size(200, 16);
            this.projectLinkLabel.TabIndex = 5;
            this.projectLinkLabel.TabStop = true;
            this.projectLinkLabel.Text = "http://code.google.com/p/tumblott/";
            // 
            // versionLabel
            // 
            this.versionLabel.Location = new System.Drawing.Point(61, 28);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(163, 25);
            this.versionLabel.TabIndex = 1;
            this.versionLabel.Text = "Version 0.0.0.0";
            // 
            // nameLabel
            // 
            this.nameLabel.Location = new System.Drawing.Point(61, 12);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(163, 16);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Tumblott";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabAccountPage.ResumeLayout(false);
            //this.tabAccountPage.PerformLayout();
            this.tabGeneralPage.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.tabNetworkPage.ResumeLayout(false);
            //this.tabNetworkPage.PerformLayout();
            this.tabAboutPage.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.iconPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabGeneralPage;
        private System.Windows.Forms.TabPage tabAboutPage;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label copyrightLabel;
        private System.Windows.Forms.LinkLabel webLinkLabel;
        private System.Windows.Forms.LinkLabel projectLinkLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label licenseLabel;
        private System.Windows.Forms.TabPage tabNetworkPage;
        private System.Windows.Forms.RadioButton useProxyRadioButton;
        private System.Windows.Forms.RadioButton noProxyRadioButton;
        private System.Windows.Forms.RadioButton useDefaultRadioButton;
        private System.Windows.Forms.Label proxyLabel;
        private System.Windows.Forms.Label proxyPortLabel;
        private System.Windows.Forms.TextBox proxyServerTextBox;
        private System.Windows.Forms.Label proxyServerLabel;
        private System.Windows.Forms.TextBox proxyPasswordTextBox;
        private System.Windows.Forms.Label proxyPasswordLabel;
        private System.Windows.Forms.TextBox proxyUsernameTextBox;
        private System.Windows.Forms.Label proxyUsernameLabel;
        private System.Windows.Forms.TextBox proxyPortTextBox;
        private System.Windows.Forms.Button debugButton;
        private System.Windows.Forms.TabPage tabAccountPage;
        private System.Windows.Forms.Label emailLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox emailTextBox;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label accountLabel;
        private System.Windows.Forms.CheckBox openLinkCheckBox;
        private System.Windows.Forms.PictureBox iconPictureBox;
        private System.Windows.Forms.CheckBox proxyOnlyImagesCheckBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;


    }
}