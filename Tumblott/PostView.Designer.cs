namespace Tumblott.Forms
{
    partial class PostView
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

        #region コンポーネント デザイナで生成されたコード

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PostView
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.Name = "PostView";
            this.Size = new System.Drawing.Size(240, 240);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PostView_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PostView_MouseDown);
            this.Resize += new System.EventHandler(this.PostView_Resize);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PostView_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

    }
}
