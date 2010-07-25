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
        private Point tappedPoint = new Point { X = 0, Y = 0 };

        private Point imagePos = new Point { X = 0, Y = 0 };

        private Image image;
        private Image offBuf;
        private Image scaledImage;

        private enum DragMode { None, Scroll, Zoom };
        private DragMode dragMode = DragMode.None;

        private int zoomPercent = 100;

        //private Point imagePosOffset;
        private Scroller scroller;

        private SizeF scaleFactor = new SizeF { Width = 1, Height = 1 };

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

                Invalidate();
            }
        }

        public ImageView()
        {
            InitializeComponent();

            // この時点では this.Width/this.Height がレイアウト前のままっぽい

            scroller = new Scroller(this);
            scroller.Scroll += new MouseEventHandler(scroller_Flick);
            scroller.ScrollStopped += new EventHandler(scroller_FlickStopped);
            scroller.Zoom += new MouseEventHandler(scroller_Zoom);
        }

        void scroller_Zoom(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                Utils.DebugLog("zoom reset");
                this.Zoom(new Point(e.X, e.Y), false);
            }
            else
            {
                Utils.DebugLog("zoom in");
                this.Zoom(new Point(e.X, e.Y), true);
            }
        }

        void Zoom(Point p, bool zoom)
        {
            if (zoom)
            {
                this.zoomPercent += 5;
            }
            else
            {
                this.zoomPercent = 100;
            }
            Utils.DebugLog("zoom percent = " + this.zoomPercent.ToString());

            if (this.scaledImage != null)
            {
                this.scaledImage.Dispose();
                this.scaledImage = null;
            }
            Invalidate();
        }

        void scroller_FlickStopped(object sender, EventArgs e)
        {
            Invalidate();
        }

        void scroller_Flick(object sender, MouseEventArgs e)
        {
            Scroll(new Point(e.X, e.Y));
        }

        void Scroll(Point delta)
        {
            // FIXME NullReferenceExceptionで落ちることがある

            if (scaledImage == null)
            {
                return;
            }
            
            imagePos.X += delta.X;
            imagePos.Y += delta.Y;

            bool stopX = false, stopY = false;

            if (imagePos.X < this.Width - scaledImage.Width)
            {
                imagePos.X = this.Width - scaledImage.Width;
                stopX = true;
            }
            if (imagePos.X > 0)
            {
                imagePos.X = 0;
                stopX = true;
            }
            if (imagePos.Y < this.Height - scaledImage.Height)
            {
                imagePos.Y = this.Height - scaledImage.Height;
                stopY = true;
            }
            if (imagePos.Y > 0)
            {
                imagePos.Y = 0;
                stopY = true;
            }

            if (stopX && stopY)
            {
                scroller.StopScroll();
            }

            Invalidate();
        }

        public void ResetPosition()
        {
            imagePos.X = 0;
            imagePos.Y = 0;
            //isMouseDown = false;
            dragMode = DragMode.None;
            tappedPoint.X = 0;
            tappedPoint.Y = 0;
            zoomPercent = 100;
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            this.scaleFactor = factor;
            base.ScaleControl(factor, specified);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (offBuf == null)
            {
                // FIXME 要Dispose
                offBuf = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            }

            Graphics g = Graphics.FromImage(offBuf);
            SolidBrush bBackground = new SolidBrush(Color.Black);

            if (image != null)
            {
                if (scaledImage == null)
                {
                    scaledImage = Utils.GetScaledImage(image, this.Width * zoomPercent/100, this.Height * zoomPercent/100, Utils.ScaleMode.Full);
                }

                g.DrawImage(scaledImage, imagePos.X, imagePos.Y);

                // スクロールバーを描画
                if (dragMode == DragMode.Scroll || scroller.IsScrolling)
                {
                    int hsbHeight = this.Height * this.Height / scaledImage.Height;
                    int hsbPos = -imagePos.Y * this.Height / scaledImage.Height;

                    Point[] hPoints = {
                                     new Point(this.Width-1, hsbPos-(int)(4*scaleFactor.Height)),
                                     new Point(this.Width-(int)(4*scaleFactor.Width), hsbPos),
                                     new Point(this.Width-(int)(4*scaleFactor.Width), hsbPos+hsbHeight),
                                     new Point(this.Width-1, hsbPos+hsbHeight+(int)(4*scaleFactor.Height)),
                                 };

                    int vsbHeight = this.Width * this.Width / scaledImage.Width;
                    int vsbPos = -imagePos.X * this.Width / scaledImage.Width;

                    Point[] vPoints = {
                                     new Point(vsbPos-(int)(4*scaleFactor.Width), this.Height-1),
                                     new Point(vsbPos, this.Height-(int)(4*scaleFactor.Height)),
                                     new Point(vsbPos+vsbHeight, this.Height-(int)(4*scaleFactor.Height)),
                                     new Point(vsbPos+vsbHeight+(int)(4*scaleFactor.Width), this.Height-1),
                                 };

                    Pen p = new Pen(Color.Black);
                    SolidBrush b = new SolidBrush(Color.Gray);

                    g.FillPolygon(b, hPoints);
                    g.DrawPolygon(p, hPoints);

                    g.FillPolygon(b, vPoints);
                    g.DrawPolygon(p, vPoints);

                    p.Dispose();
                    b.Dispose();
                }
                /*
                else if (dragMode == DragMode.Zoom)
                {
                    using (SolidBrush bText = new SolidBrush(Color.White))
                    {
                        g.DrawString(zoomPercent.ToString(), this.Font, bText, 0, 0);
                    }
                }
                */
            }
            else
            {
                g.FillRectangle(bBackground, 0, 0, this.Width, this.Height);
            }

            g.Dispose();

            e.Graphics.DrawImage(offBuf, 0, 0);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // 何もしない
        }

#if false
        private void ImageView_MouseDown(object sender, MouseEventArgs e)
        {
            if (image != null)
            {
                tappedPoint.X = e.X;
                tappedPoint.Y = e.Y;
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
                tappedPoint.X = e.X;
                tappedPoint.Y = e.Y;
            }
            else if (dragMode == DragMode.Zoom)
            {
                int moveY = e.Y - tappedPoint.Y;
                zoomPercent += moveY;
                if (zoomPercent < 0) { zoomPercent = 1; }

                if (scaledImage != null)
                {
                    scaledImage.Dispose();
                    scaledImage = null;
                }

                Invalidate();

                tappedPoint.X = e.X;
                tappedPoint.Y = e.Y;
            }
        }

        private void ImageView_MouseUp(object sender, MouseEventArgs e)
        {
            dragMode = DragMode.None;
            Invalidate();
        }
#endif

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

            scroller.StopScroll();

            Invalidate();
        }
    }
}
