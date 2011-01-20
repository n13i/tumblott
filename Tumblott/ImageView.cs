using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Text;
using System.Windows.Forms;
using M2HQ.Utils;

namespace Tumblott.Forms
{
    /// <summary>
    /// グラブ＆スクロール可能な画像表示ボックス
    /// </summary>
    public partial class ImageView : UserControl
    {
        private Point tappedPoint = new Point { X = 0, Y = 0 };

        // 画面中心点から拡大後画像中心点へのベクトル
        private Point imagePosition = new Point { X = 0, Y = 0 };

        private Image image;
        private Image offBuf;
        private Image scaledImage;

        private bool isScrolling = false;
        private bool isZooming = false;
        private int zoomPercent = 100;
        // FIXME 元画像サイズを100%とした場合，本画像読込前(サムネイル表示)のスケーリングとの整合性がとれなくなる
        // 従来通り画面サイズいっぱい==100%とするべきか
        private float fillFactor = 1.0F;

        private Point zoomAnchorPoint;

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
            scroller.ZoomStopped += new EventHandler(scroller_ZoomStopped);
        }

        void scroller_Zoom(object sender, MouseEventArgs e)
        {
            Utils.DebugLog("scroller_Zoom");

            // (e.X, e.Y)はImageViewコントロール上でのタップ位置

            isZooming = true;

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

        void scroller_ZoomStopped(object sender, EventArgs e)
        {
            Utils.DebugLog("scroller_ZoomStopped");

            isZooming = false;
            // 全体をズームした画像を生成させる
            if (this.scaledImage != null)
            {
                this.scaledImage.Dispose();
                this.scaledImage = null;
            }
            Invalidate();
        }

        void Zoom(Point p, bool zoom)
        {
            if (zoom)
            {
                int currentZoomPercent = this.zoomPercent;
                this.zoomPercent += Settings.ZoomPercentDelta;

                Size imageSize = this.image.Size;
                Size currentScaledSize = new Size((int)(imageSize.Width * fillFactor * currentZoomPercent / 100), (int)(imageSize.Height * fillFactor * currentZoomPercent / 100));
                Size scaledSize = new Size((int)(imageSize.Width * fillFactor * zoomPercent / 100), (int)(imageSize.Height * fillFactor * zoomPercent / 100));

                // 拡大前・拡大後において
                // 元画像におけるタップ点の座標が
                // 変化しないようにimagePositionを調整する

                // まずはどれだけずれるか調べる

                // タップ点のImageView上における座標 = tp1
                Point tp1 = new Point();
                tp1.X = p.X;
                tp1.Y = p.Y;
                Utils.DebugLog("Zoom: tp1 = (" + tp1.X.ToString() + ", " + tp1.Y.ToString() + ")");

                // タップ点のscaledImage上における座標
                Point dp1 = new Point();
                dp1.X = (imagePosition.X + (this.Width / 2)) - (currentScaledSize.Width / 2);
                dp1.Y = (imagePosition.Y + (this.Height / 2)) - (currentScaledSize.Height / 2);
                Point ap1 = new Point();
                ap1.X = tp1.X - dp1.X;
                ap1.Y = tp1.Y - dp1.Y;
                Utils.DebugLog("Zoom: ap1 = (" + ap1.X.ToString() + ", " + ap1.Y.ToString() + ")");

                // タップ点の次回スケール後の画像における座標
                Point ap2 = new Point();
                ap2.X = (int)(ap1.X * (float)zoomPercent / currentZoomPercent);
                ap2.Y = (int)(ap1.Y * (float)zoomPercent / currentZoomPercent);
                Utils.DebugLog("Zoom: ap2 = (" + ap2.X.ToString() + ", " + ap2.Y.ToString() + ")");

                zoomAnchorPoint.X = ap2.X;
                zoomAnchorPoint.Y = ap2.Y;

                // タップ点とのずれを計算
                Point dp2 = new Point();
                dp2.X = (imagePosition.X + (this.Width / 2)) - (scaledSize.Width / 2);
                dp2.Y = (imagePosition.Y + (this.Height / 2)) - (scaledSize.Height / 2);
                Point tp2 = new Point();
                tp2.X = ap2.X + dp2.X;
                tp2.Y = ap2.Y + dp2.Y;

                // ずれを補正
                Point delta = new Point();
                delta.X = tp2.X - tp1.X;
                delta.Y = tp2.Y - tp1.Y;
                Utils.DebugLog("Zoom: delta = (" + delta.X.ToString() + ", " + delta.Y.ToString() + ")");

                imagePosition.X -= delta.X;
                imagePosition.Y -= delta.Y;

                Utils.DebugLog("Zoom: imagePosition = (" + imagePosition.X.ToString() + ", " + imagePosition.Y.ToString() + ")");
            }
            else
            {
                this.zoomPercent = 100;
                imagePosition.X = 0;
                imagePosition.Y = 0;
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

            if (image == null)
            {
                return;
            }

            Size scaledSize = new Size((int)(image.Width * fillFactor * zoomPercent / 100), (int)(image.Height * fillFactor * zoomPercent / 100));

            Point drawPosition = new Point();
            drawPosition.X = ((imagePosition.X + delta.X) + (this.Width / 2)) - (scaledSize.Width / 2);
            drawPosition.Y = ((imagePosition.Y + delta.Y) + (this.Height / 2)) - (scaledSize.Height / 2);

            // X方向Y方向それぞれデクリメント/インクリメント方向への移動可否
            bool stopXd = false, stopXi = false, stopYd = false, stopYi = false;

            // スクロールロックの方針としては
            // 画面端から画像が離れないように
            if (scaledSize.Width <= this.Width)
            {
                if (drawPosition.X < 0)
                {
                    stopXd = true;
                }
                if(drawPosition.X + scaledSize.Width > this.Width)
                {
                    stopXi = true;
                }
            }
            else
            {
                if (drawPosition.X > 0)
                {
                    stopXi = true;
                }
                if(drawPosition.X + scaledSize.Width < this.Width)
                {
                    stopXd = true;
                }
            }

            if (scaledSize.Height <= this.Height)
            {
                if (drawPosition.Y < 0)
                {
                    stopYd = true;
                }
                if(drawPosition.Y + scaledSize.Height > this.Height)
                {
                    stopYi = true;
                }
            }
            else
            {
                if (drawPosition.Y > 0)
                {
                    stopYi = true;
                }
                if(drawPosition.Y + scaledSize.Height < this.Height)
                {
                    stopYd = true;
                }
            }

            if ((delta.X > 0 && !stopXi) || (delta.X < 0 && !stopXd))
            {
                imagePosition.X += delta.X;
            }
            if ((delta.Y > 0 && !stopYi) || (delta.Y < 0 && !stopYd))
            {
                imagePosition.Y += delta.Y;
            }

            if ((stopXd || stopXi) && (stopYd || stopYi))
            {
                scroller.StopScroll();
            }

            Invalidate();
        }

        public void ResetPosition()
        {
            imagePosition.X = 0;
            imagePosition.Y = 0;
            //isMouseDown = false;
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
            g.FillRectangle(bBackground, 0, 0, this.Width, this.Height);

            if (image != null)
            {
                if (scaledImage == null)
                {
                    // 元画像全体をスケーリング
                    // 全体が画面内に収まるようにする

                    // とりあえず画像の横方向を画面に合わせてみる
                    fillFactor = this.Width / (float)image.Width;
                    // 縦方向がはみ出すかどうか？
                    if(image.Height * fillFactor > this.Height)
                    {
                        // はみ出すので，縦方向に合わせる
                        fillFactor = this.Height / (float)image.Height;
                    }

                    Rectangle srcRect = new Rectangle(0, 0, image.Width, image.Height);
                    Rectangle dstRect = new Rectangle(0, 0, (int)(srcRect.Width * fillFactor * zoomPercent / 100), (int)(srcRect.Height * fillFactor * zoomPercent / 100));
                    scaledImage = new Bitmap(dstRect.Width, dstRect.Height, PixelFormat.Format16bppRgb565);

                    using (Graphics gImg = Graphics.FromImage(scaledImage))
                    {
                        gImg.DrawImage(image, dstRect, srcRect, GraphicsUnit.Pixel);

                        /*
                        Pen p = new Pen(Color.Red);
                        gImg.DrawLine(p, dstRect.Width / 2, 0, dstRect.Width / 2, dstRect.Height);
                        gImg.DrawLine(p, 0, dstRect.Height / 2, dstRect.Width, dstRect.Height / 2);
                        p.Dispose();
                        */
                    }

                    //scaledImage = Utils.GetScaledImage(image, this.Width * zoomPercent / 100, this.Height * zoomPercent / 100, Utils.ScaleMode.Fit);
                }

                // imagePositionから実際の描画位置を計算
                Size scaledImageSize = new Size
                {
                    Width = (int)(image.Width * fillFactor * zoomPercent / 100),
                    Height = (int)(image.Height * fillFactor * zoomPercent / 100),
                };
                Utils.DebugLog("imagePosition = (" + imagePosition.X.ToString() + ", " + imagePosition.Y.ToString() + ")");
                Point drawPosition = new Point
                {
                    X = (imagePosition.X + (this.Width / 2)) - (scaledImageSize.Width / 2),
                    Y = (imagePosition.Y + (this.Height / 2)) - (scaledImageSize.Height / 2),
                };
                Utils.DebugLog("drawPosition = (" + drawPosition.X.ToString() + ", " + drawPosition.Y.ToString() + ")");
                
                //if (false)
                if (this.isZooming)
                {
                    // 表示される範囲のみスケーリング
                    // 元画像(this.image)のどの範囲を切り取るか計算する必要がある

                    // 元画像スケールでのオフセット座標を計算
                    Point dPos = new Point
                    {
                        X = -(int)(drawPosition.X / fillFactor * 100 / zoomPercent),
                        Y = -(int)(drawPosition.Y / fillFactor * 100 / zoomPercent),
                    };

                    Rectangle srcRect = new Rectangle(
                        dPos.X, dPos.Y,
                        (int)(this.Width / fillFactor * 100 / zoomPercent),
                        (int)(this.Height / fillFactor * 100 / zoomPercent));
                    Rectangle dstRect = new Rectangle(0, 0, this.Width, this.Height);
                    g.DrawImage(image, dstRect, srcRect, GraphicsUnit.Pixel);
                }
                else
                {
                    g.DrawImage(scaledImage, drawPosition.X, drawPosition.Y);

                    /*
                    using (Pen pen = new Pen(Color.Yellow))
                    {
                        g.DrawLine(pen, p.X + zoomAnchorPoint.X, 0, p.X + zoomAnchorPoint.X, this.Height);
                        g.DrawLine(pen, 0, p.Y + zoomAnchorPoint.Y, this.Width, p.Y + zoomAnchorPoint.Y);
                    }
                    */
                }

                /*
                using (Pen p = new Pen(Color.Blue))
                {
                    g.DrawLine(p, this.Width / 2, 0, this.Width / 2, this.Height);
                    g.DrawLine(p, 0, this.Height / 2, this.Width, this.Height / 2);
                }
                */

                // スクロールバーを描画
                if (isScrolling || scroller.IsScrolling)
                {
                    Size scaledSize = new Size((int)(image.Width * fillFactor * zoomPercent / 100), (int)(image.Height * fillFactor * zoomPercent / 100));

                    int vsbHeight = this.Height * this.Height / scaledSize.Height;
                    int vsbPos = -drawPosition.Y * this.Height / scaledSize.Height;
                    int hsbWidth = this.Width * this.Width / scaledSize.Width;
                    int hsbPos = -drawPosition.X * this.Width / scaledSize.Width;

#if false
                    Point[] hPoints = {
                                     new Point(this.Width-1, hsbPos-(int)(4*scaleFactor.Height)),
                                     new Point(this.Width-(int)(4*scaleFactor.Width), hsbPos),
                                     new Point(this.Width-(int)(4*scaleFactor.Width), hsbPos+hsbHeight),
                                     new Point(this.Width-1, hsbPos+hsbHeight+(int)(4*scaleFactor.Height)),
                                 };

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
#endif
                    SolidBrush bBack = new SolidBrush(Color.FromArgb(144, 144, 144));
                    SolidBrush bFore = new SolidBrush(Color.FromArgb(96, 96, 96));

                    g.FillRectangle(bBack, this.Width - (int)(4 * this.scaleFactor.Width), 0,      (int)(4 * this.scaleFactor.Width), this.Height);
                    g.FillRectangle(bFore, this.Width - (int)(4 * this.scaleFactor.Width), vsbPos, (int)(4 * this.scaleFactor.Width), vsbHeight + 1);

                    g.FillRectangle(bBack, 0,      this.Height - (int)(4 * this.scaleFactor.Height), this.Width, (int)(4 * this.scaleFactor.Height));
                    g.FillRectangle(bFore, hsbPos, this.Height - (int)(4 * this.scaleFactor.Height), hsbWidth + 1,   (int)(4 * this.scaleFactor.Height));

                    bBack.Dispose();
                    bFore.Dispose();
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

            bBackground.Dispose();
            g.Dispose();

            e.Graphics.DrawImage(offBuf, 0, 0);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // 何もしない
        }

        private void ImageView_MouseDown(object sender, MouseEventArgs e)
        {
            this.isScrolling = true;
        }

        private void ImageView_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void ImageView_MouseUp(object sender, MouseEventArgs e)
        {
            this.isScrolling = false;
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
            imagePosition.X = 0;
            imagePosition.Y = 0;

            zoomPercent = 100;

            scroller.StopScroll();

            Invalidate();
        }
    }
}
