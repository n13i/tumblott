using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Tumblott.Forms
{
    /// <summary>
    /// グラブ＆スクロール可能な画像表示ボックス
    /// </summary>
    public partial class ImageView : UserControl
    {
        private Point mouseDown = new Point { X = 0, Y = 0 };
        //private bool isMouseDown = false;

        private Point imagePos = new Point { X = 0, Y = 0 };

        private Image image;
        private Image offBuf;
        private Image scaledImage;

        private enum DragMode { None, Scroll, Zoom };
        private DragMode dragMode = DragMode.None;

        private int zoomPercent = 100;

        public Image Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                if (scaledImage != null)
                {
                    scaledImage.Dispose();
                    scaledImage = null;
                }

                imagePos.X = 0;
                imagePos.Y = 0;
                //isMouseDown = false;
                dragMode = DragMode.None;
                mouseDown.X = 0;
                mouseDown.Y = 0;
                zoomPercent = 100;

                Invalidate();
            }
        }

        public ImageView()
        {
            InitializeComponent();

            // この時点では this.Width/this.Height がレイアウト前のままっぽい
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (offBuf == null)
            {
                // FIXME 要Dispose
                offBuf = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            }

            Graphics g = Graphics.FromImage(offBuf);
            SolidBrush b = new SolidBrush(Color.Black);

            // スクロール中のみスクロールバー表示が欲しい

            if (image != null)
            {
                if (scaledImage == null)
                {
                    scaledImage = Utils.GetScaledImage(image, this.Width * zoomPercent/100, this.Height * zoomPercent/100, Utils.ScaleMode.Full);
                }

                g.DrawImage(scaledImage, imagePos.X, imagePos.Y);
                if (dragMode == DragMode.Scroll)
                {
                    SolidBrush bBase = new SolidBrush(Color.WhiteSmoke);
                    Pen pFrame = new Pen(Color.DarkGray);

                    int hsbWidth = 8;
                    int hsbHeight = (int)(((float)this.Height / (float)scaledImage.Height) * this.Height);
                    int hsbPos = (int)(((float)this.Height / (float)scaledImage.Height) * -imagePos.Y);

                    int vsbHeight = 8;
                    int vsbWidth = (int)(((float)this.Width / (float)scaledImage.Width) * this.Width);
                    int vsbPos = (int)(((float)this.Width / (float)scaledImage.Width) * -imagePos.X);

                    g.FillRectangle(bBase, this.Width - hsbWidth, hsbPos, hsbWidth, hsbHeight);
                    g.DrawRectangle(pFrame, this.Width - hsbWidth, hsbPos, hsbWidth, hsbHeight);
                    g.FillRectangle(bBase, vsbPos, this.Height - vsbHeight, vsbWidth, vsbHeight);
                    g.DrawRectangle(pFrame, vsbPos, this.Height - vsbHeight, vsbWidth, vsbHeight);

                    bBase.Dispose();
                    pFrame.Dispose();
                }
                else if (dragMode == DragMode.Zoom)
                {
                    using (SolidBrush bText = new SolidBrush(Color.White))
                    {
                        g.DrawString(zoomPercent.ToString(), this.Font, bText, 0, 0);
                    }
                }
            }
            else
            {
                g.FillRectangle(b, 0, 0, this.Width, this.Height);
            }

            g.Dispose();

            e.Graphics.DrawImage(offBuf, 0, 0);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // 何もしない
        }

        private void ImageView_MouseDown(object sender, MouseEventArgs e)
        {
            if (image != null)
            {
                mouseDown.X = e.X;
                mouseDown.Y = e.Y;
                // FIXME 一時的に無効に
                //if (e.X > this.Width - this.Width / 5)
                //{
                //    dragMode = DragMode.Zoom;
                //}
                //else
                {
                    dragMode = DragMode.Scroll;
                }
                Invalidate();
            }
        }

        private void ImageView_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragMode == DragMode.None) { return; }

            if(dragMode == DragMode.Scroll)
            {
                int moveX = e.X - mouseDown.X;
                int moveY = e.Y - mouseDown.Y;

                imagePos.X += moveX;
                imagePos.Y += moveY;

                if (imagePos.X < this.Width - scaledImage.Width)
                {
                    imagePos.X = this.Width - scaledImage.Width;
                }
                if (imagePos.X > 0)
                {
                    imagePos.X = 0;
                }
                if (imagePos.Y < this.Height - scaledImage.Height)
                {
                    imagePos.Y = this.Height - scaledImage.Height;
                }
                if (imagePos.Y > 0)
                {
                    imagePos.Y = 0;
                }

                Invalidate();

                mouseDown.X = e.X;
                mouseDown.Y = e.Y;
            }
            else if (dragMode == DragMode.Zoom)
            {
                int moveY = e.Y - mouseDown.Y;
                zoomPercent += moveY;
                if (zoomPercent < 0) { zoomPercent = 1; }

                if (scaledImage != null)
                {
                    scaledImage.Dispose();
                    scaledImage = null;
                }

                Invalidate();

                mouseDown.X = e.X;
                mouseDown.Y = e.Y;
            }
        }

        private void ImageView_MouseUp(object sender, MouseEventArgs e)
        {
            dragMode = DragMode.None;
            Invalidate();
        }

        private void ImageView_Resize(object sender, EventArgs e)
        {
            if (offBuf != null)
            {
                offBuf.Dispose();
                offBuf = null;
            }
            if (scaledImage != null)
            {
                scaledImage.Dispose();
                scaledImage = null;
            }

            // 表示位置リセット
            imagePos.X = 0;
            imagePos.Y = 0;

            zoomPercent = 100;

            Invalidate();
        }
    }
}
