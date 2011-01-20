namespace Tumblott
{
    partial class PhotoForm
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
            this.loadingIndicator = new Tumblott.Forms.LoadingIndicator();
            this.imageView = new Tumblott.Forms.ImageView();
            this.SuspendLayout();
            // 
            // loadingIndicator
            // 
            this.loadingIndicator.BackColor = System.Drawing.Color.Black;
            this.loadingIndicator.Image = null;
            this.loadingIndicator.Location = new System.Drawing.Point(112, 126);
            this.loadingIndicator.Name = "loadingIndicator";
            this.loadingIndicator.Size = new System.Drawing.Size(16, 16);
            this.loadingIndicator.TabIndex = 2;
            // 
            // imageView
            // 
            this.imageView.BackColor = System.Drawing.Color.Black;
            this.imageView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageView.Image = null;
            this.imageView.Location = new System.Drawing.Point(0, 0);
            this.imageView.Name = "imageView";
            this.imageView.Size = new System.Drawing.Size(240, 268);
            this.imageView.TabIndex = 1;
            // 
            // PhotoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.loadingIndicator);
            this.Controls.Add(this.imageView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "PhotoForm";
            this.Text = "Photo";
            this.ResumeLayout(false);

        }

        #endregion

        private Tumblott.Forms.ImageView imageView;
        private Tumblott.Forms.LoadingIndicator loadingIndicator;

    }
}