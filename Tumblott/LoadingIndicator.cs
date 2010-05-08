using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Tumblott.Forms
{
    public class LoadingIndicator : UserControl
    {
        private ImageList imgList = new ImageList();
        private int currentImg = 0;
        private bool isThreadRunning = false;
        private Thread t;

        private Image offImg;
        private Image bgImg;

        public Image Image
        {
            get
            {
                return bgImg;
            }
            set
            {
                bgImg = value;
            }
        }

        public LoadingIndicator()
        {
            //this.Visible = true;
            //this.HandleCreated += new EventHandler(LoadingIndicator_HandleCreated);
            this.HandleDestroyed += new EventHandler(LoadingIndicator_HandleDestroyed);

            imgList.Images.Add(global::Tumblott.Properties.Resources.loading_b0);
            imgList.Images.Add(global::Tumblott.Properties.Resources.loading_b1);
            imgList.Images.Add(global::Tumblott.Properties.Resources.loading_b2);
            imgList.Images.Add(global::Tumblott.Properties.Resources.loading_b3);
            imgList.Images.Add(global::Tumblott.Properties.Resources.loading_b4);
            imgList.Images.Add(global::Tumblott.Properties.Resources.loading_b5);
            //imgList.Images.Add(global::Tumblott.Properties.Resources.loading_b6);
            //imgList.Images.Add(global::Tumblott.Properties.Resources.loading_b7);
        }

        void LoadingIndicator_HandleCreated(object sender, EventArgs e)
        {
            // CFだとうまく呼ばれないっぽい？(Destroyedは呼ばれてるっぽい)
            Utils.DebugLog("created: " + this.ToString() + ", sender=" + sender.ToString());
            // FIXME dispose時に止めるべき
            Start();
        }

        void LoadingIndicator_HandleDestroyed(object sender, EventArgs e)
        {
            Utils.DebugLog("destroyed: " + this.ToString() + ", sender=" + sender.ToString());
            Stop();
        }

        private void Animate()
        {
            while (true)
            {
                // 適当な引数を渡さないとパラメータカウントが一致しませんと怒られる
                // FIXME たまにハンドルが作成されてないと怒られる
                // PhotoFormを閉じたときに発生
                try
                {
                    this.BeginInvoke(new Action<object>(SetNextImage), this);
                }
                catch (Exception e)
                {
                }
                Thread.Sleep(100);
            }
        }

        private void SetNextImage(object o)
        {
            currentImg++;
            if (currentImg >= imgList.Images.Count)
            {
                currentImg = 0;
            }
            Invalidate();
        }

        public void Start()
        {
            if (!isThreadRunning)
            {
                t = new Thread(new ThreadStart(Animate));
                t.IsBackground = true;
                t.Start();
                isThreadRunning = true;
            }
        }

        private void Stop()
        {
            if (isThreadRunning)
            {
                if (t != null)
                {
                    try
                    {
                        t.Abort();
                    }
                    catch (ThreadAbortException e)
                    {
                    }

                    t = null;
                }
                isThreadRunning = false;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (offImg == null)
            {
                offImg = new Bitmap(this.Width, this.Height, PixelFormat.Format16bppRgb565);
            }

            Graphics g = Graphics.FromImage(offImg);

            Image img = imgList.Images[currentImg];

            Rectangle dstRect = new Rectangle(0, 0, this.Width, this.Height);

            ImageAttributes imgAttr = new ImageAttributes();
            //imgAttr.SetColorKey(Color.FromArgb(48, 48, 48), Color.FromArgb(48, 48, 48));
            imgAttr.SetColorKey(Color.FromArgb(56, 84, 109), Color.FromArgb(56, 84, 109));

            if (bgImg != null)
            {
                g.DrawImage(bgImg, dstRect, new Rectangle(0, 0, bgImg.Width, bgImg.Height), GraphicsUnit.Pixel);
                g.DrawImage(img, dstRect, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttr);
                e.Graphics.DrawImage(offImg, 0, 0);
            }
            else if (this.BackColor.Equals(Color.Transparent))
            {
                e.Graphics.DrawImage(img, dstRect, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttr);
            }
            else
            {
                using (SolidBrush b = new SolidBrush(this.BackColor))
                {
                    g.FillRectangle(b, dstRect);
                    g.DrawImage(img, dstRect, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttr);
                    e.Graphics.DrawImage(offImg, 0, 0);
                }
            }

            g.Dispose();
        }
    }
}
