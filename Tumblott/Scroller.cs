using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace M2HQ.Utils
{
    // ドラッグでのスクロール，フリック，長タップでのズームなどを実現させたいクラス
    // 参考: YOPViewer.NETのDragClickクラス
    public class Scroller : IDisposable
    {
        private struct Velocity { public double X; public double Y; }

        private Control target;

        private Timer scrollTimer;
        private Timer zoomTimer;
        private Point previous2TappedPoint;
        private Point previousTappedPoint;
        private Point currentTappedPoint;
        private Point mouseMoveDelta;
        private Point mouseMoveFlick;
        private Point mouseMoveTotal;
        private Velocity scrollVelocity;
        private bool isMouseDown;
        private bool isFlicked;
        private int zoomWait;
        private bool isZooming;

        private const int zoomWaitThreshold = 10;

        public event MouseEventHandler Scroll;
        public event EventHandler ScrollStopped;
        public event MouseEventHandler Flick;
        public event MouseEventHandler Zoom;
        public event EventHandler ZoomStopped;
        public event MouseEventHandler Tapped;

        public bool IsScrolling { get { return this.scrollTimer.Enabled; } }

        public Scroller(Control target)
        {
            this.target = target;

            this.target.MouseDown += new MouseEventHandler(target_MouseDown);
            this.target.MouseMove += new MouseEventHandler(target_MouseMove);
            this.target.MouseUp += new MouseEventHandler(target_MouseUp);

            scrollTimer = new Timer { Interval = 33 };
            scrollTimer.Tick += new EventHandler(scrollTimer_Tick);

            zoomTimer = new Timer { Interval = 33 };
            zoomTimer.Tick += new EventHandler(zoomTimer_Tick);

            zoomWait = 0;
            isZooming = false;
            isMouseDown = false;
        }

        // for debug
        public void DrawStatus(Graphics g)
        {
#if false
            if (Settings.DebugLog)
            {
                g.DrawString("md=" + this.isMouseDown.ToString() + ", fl=" + this.scrollTimer.Enabled.ToString() +
                    ", v=" + this.scrollVelocity.X.ToString() + "," + this.scrollVelocity.Y.ToString() +
                    ", d=" + this.mouseMoveDelta.X.ToString() + "," + this.mouseMoveDelta.Y.ToString(),
                    new Font("Tahoma", 8F, FontStyle.Bold), new SolidBrush(Color.Black), 0, 0);
            }
#endif
        }

        void scrollTimer_Tick(object sender, EventArgs e)
        {
            if (this.Scroll != null)
            {
                this.Scroll(target, new MouseEventArgs(Control.MouseButtons, 0, (int)scrollVelocity.X, (int)scrollVelocity.Y, 0));
            }
            scrollVelocity.X *= 0.95;
            scrollVelocity.Y *= 0.95;
            if (Math.Abs(scrollVelocity.X) < 1 && Math.Abs(scrollVelocity.Y) < 1)
            {
                scrollTimer.Enabled = false;
                if (this.ScrollStopped != null)
                {
                    this.ScrollStopped(target, new EventArgs());
                }
            }
        }

        void zoomTimer_Tick(object sender, EventArgs e)
        {
            if (this.Zoom != null && this.CheckZoomThreshold())
            {
                if (zoomWait < zoomWaitThreshold)
                {
                    zoomWait++;
                }
                else
                {
                    //Utils.DebugLog("currentTappedPoint = (" + currentTappedPoint.X.ToString() + ", " + currentTappedPoint.Y.ToString() + ")");
                    // ズームイン
                    this.Zoom(target, new MouseEventArgs(MouseButtons.Left, 0, this.currentTappedPoint.X, this.currentTappedPoint.Y, 0));
                    isZooming = true;
                }
            }
        }

        // タップ・ドラッグ開始
        void target_MouseDown(object sender, MouseEventArgs e)
        {
            zoomTimer.Enabled = true;
            this.StopScroll();
            mouseMoveDelta.X = 0;
            mouseMoveDelta.Y = 0;
            mouseMoveFlick.X = 0;
            mouseMoveFlick.Y = 0;
            mouseMoveTotal.X = 0;
            mouseMoveTotal.Y = 0;
            //currentTappedPoint = target.PointToScreen(new Point(e.X, e.Y));
            currentTappedPoint = new Point(e.X, e.Y);
            previousTappedPoint = currentTappedPoint;
            previous2TappedPoint = currentTappedPoint;
            isMouseDown = true;
            isFlicked = false;
        }

        // ドラッグ
        void target_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                //currentTappedPoint = target.PointToScreen(new Point(e.X, e.Y));
                currentTappedPoint = new Point(e.X, e.Y);
                //mouseMoveDelta.X = currentTappedPoint.X - previousTappedPoint.X;
                //mouseMoveDelta.Y = currentTappedPoint.Y - previousTappedPoint.Y;
                // WM6.5デバイスでMouseUp時にMouseMove→MouseUpされる→ベロシティ0になるみたいなので2つ前の値を使ってみるテスト (2010/07/07)
                mouseMoveDelta.X = currentTappedPoint.X - previous2TappedPoint.X;
                mouseMoveDelta.Y = currentTappedPoint.Y - previous2TappedPoint.Y;

                // フリック移動量
                mouseMoveFlick.X += currentTappedPoint.X - previousTappedPoint.X;
                mouseMoveFlick.Y += currentTappedPoint.Y - previousTappedPoint.Y;
                // TODO ドラッグ方向が逆になったらリセットしたほうがいいかも

                // タップされてからの合計移動量
                mouseMoveTotal.X += Math.Abs(currentTappedPoint.X - previousTappedPoint.X);
                mouseMoveTotal.Y += Math.Abs(currentTappedPoint.Y - previousTappedPoint.Y);

                if (!isFlicked && this.Scroll != null)
                {
                    // スクロールは前回値との差分で行う
                    this.Scroll(target, new MouseEventArgs(Control.MouseButtons, 0,
                        currentTappedPoint.X - previousTappedPoint.X,
                        currentTappedPoint.Y - previousTappedPoint.Y, 0));
                }

                // FIXME 画面サイズで決まるような定数どうしようね　動作の可否をここで決めないで上に投げるとか？
                if (!isFlicked && this.Flick != null && Math.Abs(mouseMoveFlick.X) > 80 && Math.Abs(mouseMoveFlick.Y) < 50)
                {
                    if (mouseMoveFlick.X > 0)
                    {
                        this.Flick(target, new MouseEventArgs(Control.MouseButtons, 0, -1, 0, 0));
                    }
                    else
                    {
                        this.Flick(target, new MouseEventArgs(Control.MouseButtons, 0, 1, 0, 0));
                    }
                    mouseMoveFlick.X = 0;
                    mouseMoveFlick.Y = 0;
                    // MouseUpするまでは次のフリック動作を行わない
                    isFlicked = true;
                }

                previous2TappedPoint = previousTappedPoint;
                previousTappedPoint = currentTappedPoint;
            }
            //Utils.DebugLog("d={" + mouseMoveDelta.X.ToString() + ", " + mouseMoveDelta.Y.ToString() + "}");
        }

        // リリース
        void target_MouseUp(object sender, MouseEventArgs e)
        {
            // 最後のMoveとUpの座標は同じになるっぽい (2010/05/15)
            //currentTappedPoint = target.PointToScreen(new Point(e.X, e.Y));
            //mouseMoveDelta.X = currentTappedPoint.X - previousTappedPoint.X;
            //mouseMoveDelta.Y = currentTappedPoint.Y - previousTappedPoint.Y;
            //mouseMoveDelta.X = currentTappedPoint.X - previous2TappedPoint.X;
            //mouseMoveDelta.Y = currentTappedPoint.Y - previous2TappedPoint.Y;

            if (this.Tapped != null && mouseMoveTotal.X < 10 && mouseMoveTotal.Y < 10)
            {
                this.Tapped(target, new MouseEventArgs(MouseButtons.Left, 1, e.X, e.Y, 0));
            }

            zoomTimer.Enabled = false;
            if (this.zoomWait >= zoomWaitThreshold)
            {
                // ズーム動作開始していた場合，ズーム停止を通知
                this.ZoomStopped(target, new EventArgs());
            }
            if (this.isZooming && this.zoomWait < zoomWaitThreshold && this.CheckZoomThreshold())
            {
                // 既にズームしており，ズーム動作開始前で，スクロール閾値未満ならズームリセット
                this.Zoom(target, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0));
                this.isZooming = false;
                this.ZoomStopped(target, new EventArgs());
            }
            zoomWait = 0;

            if (isMouseDown)
            {
                isMouseDown = false;

                if (!isFlicked && (Math.Abs(mouseMoveDelta.X) > 0 || Math.Abs(mouseMoveDelta.Y) > 0))
                {
                    scrollVelocity.X = (int)mouseMoveDelta.X;
                    scrollVelocity.Y = (int)mouseMoveDelta.Y;
                    scrollTimer.Enabled = true;
                }
            }

            //Utils.DebugLog("c={" + currentTappedPoint.X.ToString() + ", " + currentTappedPoint.Y.ToString() + "}");
            //Utils.DebugLog("p={" + previousTappedPoint.X.ToString() + ", " + previousTappedPoint.Y.ToString() + "}");
            //Utils.DebugLog("d={" + mouseMoveDelta.X.ToString() + ", " + mouseMoveDelta.Y.ToString() + "}");
        }

        bool CheckZoomThreshold()
        {
            return this.mouseMoveTotal.X + this.mouseMoveTotal.Y < 20;
        }

        public void StopScroll()
        {
            scrollTimer.Enabled = false;
            scrollVelocity.X = 0;
            scrollVelocity.Y = 0;
            if (ScrollStopped != null)
            {
                ScrollStopped(target, new EventArgs());
            }
        }

        public void Dispose()
        {
            this.Scroll = null;
            this.ScrollStopped = null;
            this.Flick = null;
            this.Zoom = null;
            this.ZoomStopped = null;
            this.Tapped = null;
        }
    }
}
