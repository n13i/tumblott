using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Tumblott
{
    class NotifyItem
    {
        public string Text;
        public Image Image;
        public int Timeout;
        public Guid Guid;
    }

    /*
     * priority
     * timeout
     * notification用途にも使えるようにしたい
     */
    public partial class ProgressStatusBar : UserControl, IDisposable
    {
        private SizeF scaleFactor;

        private string text;
        private int value;

        private Image offImg;
        private Graphics offImgGr;

        private List<NotifyItem> notifyList;

        private bool isProgressMode = false;

        public bool IsProgressMode
        {
            set
            {
                this.isProgressMode = value;
                this.SwitchVisibility(value);
            }
            get
            {
                return this.isProgressMode;
            }
        }

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
            this.notifyList = new List<NotifyItem>();
        }

        public void AddNotify(Guid guid, Image img, string text, int timeout)
        {
            Image scaledImg = null;
            if (img != null)
            {
                scaledImg = Utils.GetScaledImage(img, (int)(16 * this.scaleFactor.Width), (int)(16 * this.scaleFactor.Height), Utils.ScaleMode.Fit);
            }

            this.SwitchVisibility(true);
            this.notifyList.Add(new NotifyItem { Guid = guid, Image = scaledImg, Text = text, Timeout = timeout });
            Utils.DebugLog("notifylist add: guid = " + guid);
            this.Invalidate();
        }

        public void ChangeNotify(Guid guid, string text, int timeout)
        {
            for(int i = 0; i < this.notifyList.Count; i++)
            {
                if (this.notifyList[i].Guid.Equals(guid))
                {
                    this.notifyList[i].Text = text;
                    this.notifyList[i].Timeout = timeout;
                    this.Invalidate();
                    break;
                }
            }
        }

        public void RemoveNotify(Guid guid)
        {
            for(int i = 0; i < this.notifyList.Count; i++)
            {
                if (this.notifyList[i].Guid.Equals(guid))
                {
                    Utils.DebugLog("notifylist remove: guid = " + guid);
                    if (this.notifyList[i].Image != null)
                    {
                        this.notifyList[i].Image.Dispose();
                    }
                    this.notifyList.RemoveAt(i);
                    this.Invalidate();
                    break;
                }
            }

            if (this.notifyList.Count == 0)
            {
                this.SwitchVisibility(false);
            }
        }

        public void SwitchVisibility(bool visible)
        {
            if (visible)
            {
                this.Visible = true;
            }
            else
            {
                if (this.notifyList.Count == 0 && !this.isProgressMode)
                {
                    this.Visible = false;
                    this.Invalidate();
                }
            }
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

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            this.scaleFactor = factor;
            base.ScaleControl(factor, specified);
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

            if (this.notifyList.Count > 0)
            {
                for (int i = 0; i < this.notifyList.Count; i++)
                {
                    Image notifyImg = this.notifyList[i].Image;
                    string notifyText = this.notifyList[i].Text;
                    if (notifyImg != null)
                    {
                        offImgGr.DrawImage(notifyImg, (int)((1 + i * 60) * this.scaleFactor.Width), (this.Height - notifyImg.Height) / 2);
                    }
                    textSize = offImgGr.MeasureString(notifyText, this.Font);
                    offImgGr.DrawString(this.notifyList[i].Text, this.Font, bText, (int)(18 + i * 60) * this.scaleFactor.Width, (float)(this.Height - Math.Floor(textSize.Height)) / 2);
                }
            }

            e.Graphics.DrawImage(offImg, 0, 0);
        }
    }
}
