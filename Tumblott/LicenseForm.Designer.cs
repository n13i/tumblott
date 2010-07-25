namespace Tumblott
{
    partial class LicenseForm
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
            this.postView1 = new Tumblott.Forms.PostView();
            this.SuspendLayout();
            // 
            // postView1
            // 
            this.postView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.postView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.postView1.HideHeader = true;
            this.postView1.Location = new System.Drawing.Point(0, 0);
            this.postView1.Name = "postView1";
            this.postView1.Size = new System.Drawing.Size(240, 268);
            this.postView1.TabIndex = 0;
            this.postView1.Title = null;
            // 
            // LicenseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.postView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "LicenseForm";
            this.Text = "License";
            this.Load += new System.EventHandler(this.LicenseForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Tumblott.Forms.PostView postView1;
    }
}