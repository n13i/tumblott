using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Tumblott.Forms
{
    public class StatusPanel : Panel
    {
        private string text;
        private Image offImg;

        public new string Text
        {
            set
            {
                text = value;
                if (this.Visible)
                {
                    Invalidate();
                }
            }
            get
            {
                return text;
            }
        }

        public StatusPanel()
        {
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            if (offImg == null)
            {
                offImg = new Bitmap(this.Width, this.Height, PixelFormat.Format16bppRgb565);
            }

            Graphics g = Graphics.FromImage(offImg);

            // パネル背景画像の描画
            Image bgImg = global::Tumblott.Properties.Resources.button_bg;

            // ImageButtonからコピー いずれ共通化すべき
            Rectangle dstRect = new Rectangle(0, 0, this.Width, this.Height);
            Rectangle srcRect = new Rectangle();
            float ratio = 1;
            if (this.Width > this.Height)
            {
                // 幅をあわせる
                ratio = (float)bgImg.Width / (float)this.Width;
            }
            else
            {
                // 高さをあわせる
                ratio = (float)bgImg.Height / (float)this.Height;
            }

            srcRect.Width = (int)(this.Width * ratio);
            srcRect.Height = (int)(this.Height * ratio);
            srcRect.X = (bgImg.Width - (int)(this.Width * ratio)) / 2;
            srcRect.Y = (bgImg.Height - (int)(this.Height * ratio)) / 2;

            g.DrawImage(bgImg, dstRect, srcRect, GraphicsUnit.Pixel);

            int offsetY = (int)(16 * (g.DpiY / 96));

            Font font = new Font(FontFamily.GenericSansSerif, 9F, FontStyle.Regular);
            SolidBrush b = new SolidBrush(this.ForeColor);
            RectangleF labelRect = new RectangleF(0, offsetY, this.Width, this.Height-offsetY);
            SizeF textSize = e.Graphics.MeasureString(text, font);

            g.DrawString(text, font, b, (labelRect.Width - textSize.Width)/2, (labelRect.Height - textSize.Height)/2 + offsetY);

            e.Graphics.DrawImage(offImg, 0, 0);
        }
    }
}
