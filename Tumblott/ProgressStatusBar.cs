using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Tumblott
{
    public partial class ProgressStatusBar : UserControl, IDisposable
    {
        private string text;
        private int value;

        private Image offImg;
        private Graphics offImgGr;

        public int Value
        {
            set
            {
                this.value = value;
                Invalidate();
            }
        }

        public override string Text
        {
            set
            {
                text = value;
                Invalidate();
            }
        }

        public Color ProgressColor { get; set; }

        public ProgressStatusBar()
        {
            InitializeComponent();
        }

        public new void Dispose()
        {
            if (offImgGr != null)
            {
                offImgGr.Dispose();
            }
            if (offImg != null)
            {
                offImg.Dispose();
            }
            base.Dispose();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.offImg != null)
            {
                this.offImgGr.Dispose();
                this.offImgGr = null;
                this.offImg.Dispose();
                this.offImg = null;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {     
            // 何もしない
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            float ratio = e.Graphics.DpiX / 192F; 
            
            SolidBrush bBack = new SolidBrush(this.BackColor);
            SolidBrush bBar = new SolidBrush(this.ProgressColor);
            SolidBrush bText = new SolidBrush(this.ForeColor);

            if (offImg == null)
            {
                // FIXME サイズ変更時に再度作成が必要

                // コンストラクタの時点では this.Width と this.Height が正しくないので
                // OnPaint 時に行う
                offImg = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
                offImgGr = Graphics.FromImage(offImg);
            }

            // 背景とプログレスバー部分を塗る
            offImgGr.FillRectangle(bBack, 0, 0, this.Width, this.Height);
            //offImgGr.DrawImage(global::Tumblott.Properties.Resources.progress_back,
            //    new Rectangle(0, 0, this.Width, this.Height),
            //    new Rectangle(0, 0, global::Tumblott.Properties.Resources.progress_back.Width, global::Tumblott.Properties.Resources.progress_back.Height),
            //    GraphicsUnit.Pixel);
            offImgGr.FillRectangle(bBar, 0, 0, this.Width * value / 100, this.Height);
            //offImgGr.DrawImage(global::Tumblott.Properties.Resources.progress_fore,
            //    new Rectangle(0, 0, this.Width * value / 100, this.Height),
            //    new Rectangle(0, 0, global::Tumblott.Properties.Resources.progress_fore.Width, global::Tumblott.Properties.Resources.progress_fore.Height),
            //    GraphicsUnit.Pixel);

            // テキスト描画
            SizeF textSize = offImgGr.MeasureString(text, this.Font);
            offImgGr.DrawString(text, this.Font, bText,
                (float)(this.Width - Math.Floor(textSize.Width)) / 2,
                //ratio * 4,
                (float)(this.Height - Math.Floor(textSize.Height)) / 2);

            e.Graphics.DrawImage(offImg, 0, 0);
        }
    }
}
