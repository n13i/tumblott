using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using M2HQ.Utils;

namespace Tumblott
{
    public partial class ScrollList : UserControl
    {
        private List<ScrollListItem> items;
        private Scroller scroller;
        private int positionY = 0;

        private SizeF scaleFactor;
        private Image offImg;

        public int SelectedIndex = -1;
        public event EventHandler SelectedIndexChanged;

        public ScrollList()
        {
            InitializeComponent();
            this.items = new List<ScrollListItem>();
            scroller = new Scroller(this);
            scroller.Scroll += new MouseEventHandler(scroller_Scroll);
            scroller.ScrollStopped += new EventHandler(scroller_ScrollStopped);
            scroller.Tapped += new MouseEventHandler(scroller_Tapped);
        }

        void scroller_Tapped(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
            Utils.DebugLog("Tapped " + e.X + ", " + e.Y);

            int idx = 0;
            int y = this.positionY;
            foreach (var item in this.items)
            {
                if (y <= e.Y && e.Y < y + item.Height)
                {
                    if (item.IsSelected)
                    {
                        this.SelectedIndex = idx;
                        if (this.SelectedIndexChanged != null)
                        {
                            this.SelectedIndexChanged(this, new EventArgs());
                        }
                    }
                    item.IsSelected = true;
                }
                else
                {
                    item.IsSelected = false;
                }
                y += item.Height;
                idx++;
            }
            Invalidate();
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            this.scaleFactor = factor;
            base.ScaleControl(factor, specified);
        }

        void scroller_Scroll(object sender, MouseEventArgs e)
        {
            if (this.items == null)
            {
                return;
            }

            if (this.items.Count <= 0)
            {
                return;
            }

            this.positionY += e.Y;
            int listHeight = this.items.Count * this.items[0].Height;

            //Utils.DebugLog("positionY = " + this.positionY);

            if (this.positionY > 0)
            {
                this.scroller.StopScroll();
                this.positionY = 0;
            }
            if (this.positionY < -listHeight + this.Height)
            {
                this.scroller.StopScroll();
                this.positionY = -listHeight + this.Height;
            }
            if (listHeight < this.Height)
            {
                this.positionY = 0;
            }

            Invalidate();
        }

        void scroller_ScrollStopped(object sender, EventArgs e)
        {
        }

        public void Clear()
        {
            foreach (var item in this.items)
            {
                item.Dispose();
            }
            this.items.Clear();
            this.positionY = 0;
            Invalidate();
        }

        public void Add(ScrollListItem item)
        {
            this.items.Add(item);
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // 何もしない
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (offImg == null)
            {
                this.offImg = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            }

            using (Graphics g = Graphics.FromImage(this.offImg))
            {
                using (SolidBrush b = new SolidBrush(this.BackColor))
                {
                    g.FillRectangle(b, 0, 0, this.Width, this.Height);
                }

                int y = this.positionY;
                foreach (var item in this.items)
                {
                    // visibleなものだけ描画させたい
                    if (y >= -item.Height && y < this.Height)
                    {
                        item.Draw(g, 0, y);
                    }
                    y += item.Height;
                }
            }
            e.Graphics.DrawImage(offImg, 0, 0);
        }

        protected override void OnResize(EventArgs e)
        {
            if (offImg != null)
            {
                offImg.Dispose();
                offImg = null;
            }
            base.OnResize(e);
        }

        public new void Dispose()
        {
            Utils.DebugLog("ChannelList: Dispose");
            if (this.scroller != null)
            {
                this.scroller.Dispose();
            }
            base.Dispose();
        }
    }

    public abstract class ScrollListItem : IDisposable
    {
        public int Width
        {
            get { return this.size.Width; }
            set
            {
                if (this.size.Width != value)
                {
                    this.size.Width = value;
                    ClearImage();
                }
            }
        }
        public int Height
        {
            get { return this.size.Height; }
            set
            {
                if (this.size.Height != value)
                {
                    this.size.Height = value;
                    ClearImage();
                }
            }
        }
        public SizeF ScaleFactor;
        public bool IsSelected
        {
            get { return this.selected; }
            set
            {
                if (this.selected != value)
                {
                    this.selected = value;
                    ClearImage();
                }
            }
        }

        private bool selected = false;
        private Size size;
        public Image img;

        public ScrollListItem()
        {
            this.size = new Size();
        }

        private void ClearImage()
        {
            if (this.img != null)
            {
                this.img.Dispose();
                this.img = null;
            }
        }

        public abstract void Draw(Graphics g, int x, int y);

        public void Dispose()
        {
            if (img != null)
            {
                img.Dispose();
                img = null;
            }
        }
    }
}
