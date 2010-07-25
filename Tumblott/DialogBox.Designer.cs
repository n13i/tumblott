namespace Tumblott
{
    partial class DialogBox
    {
        /// <summary>
        /// 必要なデザイナー変数です。
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

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogBox));
            this.imageButton1 = new Tumblott.Forms.ImageButton();
            this.imageButton2 = new Tumblott.Forms.ImageButton();
            this.SuspendLayout();
            // 
            // imageButton1
            // 
            this.imageButton1.Image = null;
            this.imageButton1.Location = new System.Drawing.Point(9, 75);
            this.imageButton1.Name = "imageButton1";
            this.imageButton1.Size = new System.Drawing.Size(79, 36);
            this.imageButton1.TabIndex = 0;
            // 
            // imageButton2
            // 
            this.imageButton2.Image = null;
            this.imageButton2.Location = new System.Drawing.Point(94, 75);
            this.imageButton2.Name = "imageButton2";
            this.imageButton2.Size = new System.Drawing.Size(92, 35);
            this.imageButton2.TabIndex = 1;
            // 
            // DialogBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(192, 117);
            this.ControlBox = false;
            this.Controls.Add(this.imageButton2);
            this.Controls.Add(this.imageButton1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DialogBox";
            this.Text = "DialogBox";
            this.ResumeLayout(false);

        }

        #endregion

        private Tumblott.Forms.ImageButton imageButton1;
        private Tumblott.Forms.ImageButton imageButton2;
    }
}