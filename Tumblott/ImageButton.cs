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
    public class ImageButton : UserControl
    {
        private bool isPressed = false;

        private Image bgImg;
        private Image bgOnClickImg;
        private Image buttonImg;
        private Image offBuf;

        private SizeF scaleFactor = new SizeF(1, 1);

        public Image Image
        {
            get
            {
                return buttonImg;
            }
            set
            {
                buttonImg = value;
                if (this.Visible)
                {
                    Invalidate();
                }
            }
        }

        public new event EventHandler Click;

        public ImageButton()
        {
            bgImg = global::Tumblott.Properties.Resources.button_bg;
            bgOnClickImg = global::Tumblott.Properties.Resources.button_bg_onclick;
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            this.scaleFactor = factor;
            base.ScaleControl(factor, specified);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (offBuf == null)
            {
                offBuf = new Bitmap(this.Width, this.Height, PixelFormat.Format16bppRgb565);
            }

            Graphics g = Graphics.FromImage(offBuf);

            ImageAttributes imgAttr = new ImageAttributes();
            // CF では colorLow == colorHigh じゃないとだめ
            //imgAttr.SetColorKey(Color.FromArgb(48, 48, 48), Color.FromArgb(48, 48, 48));
            imgAttr.SetColorKey(Color.FromArgb(56, 84, 109), Color.FromArgb(56, 84, 109));

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

            if (isPressed)
            {
                g.DrawImage(bgOnClickImg, dstRect, srcRect, GraphicsUnit.Pixel);
            }
            else
            {
                g.DrawImage(bgImg, dstRect, srcRect, GraphicsUnit.Pixel);
            }

            if (buttonImg != null)
            {
                g.DrawImage(buttonImg, dstRect, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, imgAttr);
            }

            e.Graphics.DrawImage(offBuf, 0, 0);
            g.Dispose();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            isPressed = true;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isPressed = false;
            Invalidate();

            if (this.ClientRectangle.Contains(new Point(e.X, e.Y)) && e.Button == MouseButtons.Left)
            {
                EventHandler evh = Click;
                if (evh != null)
                {
                    evh(this, e);
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ImageButton
            // 
            this.Name = "ImageButton";
            this.ResumeLayout(false);
        }
    }
}
