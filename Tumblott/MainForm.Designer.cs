namespace Tumblott
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.buttonsPanel = new System.Windows.Forms.UserControl();
            this.nextButton = new Tumblott.Forms.ImageButton();
            this.reblogButton = new Tumblott.Forms.ImageButton();
            this.statusPanel = new Tumblott.Forms.StatusPanel();
            this.loadingIndicator = new Tumblott.Forms.LoadingIndicator();
            this.likeButton = new Tumblott.Forms.ImageButton();
            this.prevButton = new Tumblott.Forms.ImageButton();
            this.progressStatusBar = new Tumblott.ProgressStatusBar();
            this.postView = new Tumblott.Forms.PostView();
            this.buttonsPanel.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonsPanel
            // 
            this.buttonsPanel.Controls.Add(this.nextButton);
            this.buttonsPanel.Controls.Add(this.reblogButton);
            this.buttonsPanel.Controls.Add(this.statusPanel);
            this.buttonsPanel.Controls.Add(this.likeButton);
            this.buttonsPanel.Controls.Add(this.prevButton);
            this.buttonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonsPanel.Location = new System.Drawing.Point(0, 236);
            this.buttonsPanel.Name = "buttonsPanel";
            this.buttonsPanel.Size = new System.Drawing.Size(240, 32);
            this.buttonsPanel.TabIndex = 4;
            // 
            // nextButton
            // 
            this.nextButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.nextButton.Image = global::Tumblott.Properties.Resources.button_next;
            this.nextButton.Location = new System.Drawing.Point(192, 0);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(48, 32);
            this.nextButton.TabIndex = 3;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // reblogButton
            // 
            this.reblogButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.reblogButton.Image = global::Tumblott.Properties.Resources.button_reblog;
            this.reblogButton.Location = new System.Drawing.Point(144, 0);
            this.reblogButton.Name = "reblogButton";
            this.reblogButton.Size = new System.Drawing.Size(48, 32);
            this.reblogButton.TabIndex = 2;
            this.reblogButton.Click += new System.EventHandler(this.reblogButton_Click);
            // 
            // statusPanel
            // 
            this.statusPanel.Controls.Add(this.loadingIndicator);
            this.statusPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.statusPanel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.statusPanel.Location = new System.Drawing.Point(96, 0);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(48, 32);
            this.statusPanel.TabIndex = 4;
            // 
            // loadingIndicator
            // 
            this.loadingIndicator.BackColor = System.Drawing.Color.Black;
            this.loadingIndicator.Image = global::Tumblott.Properties.Resources.button_bg_loading;
            this.loadingIndicator.Location = new System.Drawing.Point(16, 0);
            this.loadingIndicator.Name = "loadingIndicator";
            this.loadingIndicator.Size = new System.Drawing.Size(16, 16);
            this.loadingIndicator.TabIndex = 2;
            // 
            // likeButton
            // 
            this.likeButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.likeButton.Image = global::Tumblott.Properties.Resources.button_like;
            this.likeButton.Location = new System.Drawing.Point(48, 0);
            this.likeButton.Name = "likeButton";
            this.likeButton.Size = new System.Drawing.Size(48, 32);
            this.likeButton.TabIndex = 1;
            this.likeButton.Click += new System.EventHandler(this.likeButton_Click);
            // 
            // prevButton
            // 
            this.prevButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.prevButton.Image = global::Tumblott.Properties.Resources.button_prev;
            this.prevButton.Location = new System.Drawing.Point(0, 0);
            this.prevButton.Name = "prevButton";
            this.prevButton.Size = new System.Drawing.Size(48, 32);
            this.prevButton.TabIndex = 0;
            this.prevButton.Click += new System.EventHandler(this.prevButton_Click);
            // 
            // progressStatusBar
            // 
            this.progressStatusBar.BackColor = System.Drawing.Color.Black;
            this.progressStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressStatusBar.ForeColor = System.Drawing.Color.Gainsboro;
            this.progressStatusBar.Location = new System.Drawing.Point(0, 220);
            this.progressStatusBar.Name = "progressStatusBar";
            this.progressStatusBar.ProgressColor = System.Drawing.Color.SteelBlue;
            this.progressStatusBar.Size = new System.Drawing.Size(240, 16);
            this.progressStatusBar.TabIndex = 9;
            // 
            // postView
            // 
            this.postView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.postView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.postView.Location = new System.Drawing.Point(0, 0);
            this.postView.Name = "postView";
            this.postView.Size = new System.Drawing.Size(240, 236);
            this.postView.TabIndex = 8;
            this.postView.ImageClicked += new System.EventHandler(this.postView_ImageClicked);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.progressStatusBar);
            this.Controls.Add(this.postView);
            this.Controls.Add(this.buttonsPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Location = new System.Drawing.Point(0, 26);
            this.Name = "MainForm";
            this.Text = "Tumblott";
            this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.buttonsPanel.ResumeLayout(false);
            this.statusPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion


        protected void Portrait()
        {
            this.buttonsPanel.SuspendLayout();
            this.progressStatusBar.SuspendLayout();

            // FIXME PanelがAutoScaleされない(SuspendLayout/ResumeLayoutが必要？)
            // → PanelをUserControlに変更
            //    AutoScaleMode, AutoScaleDimensionsを両方指定
            //    SuspendLayout/ResumeLayoutを行う
            //    (ResumeLayoutはUserControl内のButtonの変更後に行うこと)
            this.buttonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonsPanel.Location = new System.Drawing.Point(0, 236);
            this.buttonsPanel.Size = new System.Drawing.Size(240, 32);
            this.buttonsPanel.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.buttonsPanel.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);

            this.nextButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.nextButton.Location = new System.Drawing.Point(192, 0);
            this.nextButton.Size = new System.Drawing.Size(48, 32);

            this.reblogButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.reblogButton.Location = new System.Drawing.Point(144, 0);
            this.reblogButton.Size = new System.Drawing.Size(48, 32);

            this.statusPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.statusPanel.Location = new System.Drawing.Point(96, 0);
            this.statusPanel.Size = new System.Drawing.Size(48, 32);

            this.loadingIndicator.Location = new System.Drawing.Point(16, 0);
            this.loadingIndicator.Size = new System.Drawing.Size(16, 16);

            this.likeButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.likeButton.Location = new System.Drawing.Point(48, 0);
            this.likeButton.Size = new System.Drawing.Size(48, 32);

            this.prevButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.prevButton.Location = new System.Drawing.Point(0, 0);
            this.prevButton.Size = new System.Drawing.Size(48, 32);

            this.progressStatusBar.Location = new System.Drawing.Point(0, 220);
            this.progressStatusBar.Size = new System.Drawing.Size(240, 16);
            this.progressStatusBar.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.progressStatusBar.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);

            this.postView.Location = new System.Drawing.Point(0, 0);
            this.postView.Size = new System.Drawing.Size(240, 236);

            this.buttonsPanel.ResumeLayout();
            this.progressStatusBar.ResumeLayout();
        }

        protected void Landscape()
        {
            this.buttonsPanel.SuspendLayout();
            this.progressStatusBar.SuspendLayout();

            this.buttonsPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonsPanel.Location = new System.Drawing.Point(192, 0);
            this.buttonsPanel.Size = new System.Drawing.Size(48, 268);
            this.buttonsPanel.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.buttonsPanel.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);

            this.nextButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.nextButton.Location = new System.Drawing.Point(0, 128);
            this.nextButton.Size = new System.Drawing.Size(48, 32);

            this.reblogButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.reblogButton.Location = new System.Drawing.Point(0, 96);
            this.reblogButton.Size = new System.Drawing.Size(48, 32);

            this.statusPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.statusPanel.Location = new System.Drawing.Point(0, 64);
            this.statusPanel.Size = new System.Drawing.Size(48, 32);
            
            this.loadingIndicator.Location = new System.Drawing.Point(16, 0);
            this.loadingIndicator.Size = new System.Drawing.Size(16, 16);

            this.likeButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.likeButton.Location = new System.Drawing.Point(0, 32);
            this.likeButton.Size = new System.Drawing.Size(48, 32);

            this.prevButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.prevButton.Location = new System.Drawing.Point(0, 0);
            this.prevButton.Size = new System.Drawing.Size(48, 32);
            
            this.progressStatusBar.Location = new System.Drawing.Point(0, 220);
            this.progressStatusBar.Size = new System.Drawing.Size(240, 16);
            this.progressStatusBar.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.progressStatusBar.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);

            this.postView.Location = new System.Drawing.Point(0, 0);
            this.postView.Size = new System.Drawing.Size(240, 236);

            this.buttonsPanel.ResumeLayout();
            this.progressStatusBar.ResumeLayout();
        }

        private Tumblott.Forms.ImageButton prevButton;
        private System.Windows.Forms.UserControl buttonsPanel;
        private Tumblott.Forms.ImageButton nextButton;
        private Tumblott.Forms.ImageButton reblogButton;
        private Tumblott.Forms.ImageButton likeButton;
        private Tumblott.Forms.StatusPanel statusPanel;
        private Tumblott.Forms.LoadingIndicator loadingIndicator;
        private Tumblott.Forms.PostView postView;
        private ProgressStatusBar progressStatusBar;


    }
}

