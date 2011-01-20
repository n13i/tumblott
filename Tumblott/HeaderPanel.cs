using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using M2HQ.Drawing;

namespace Tumblott
{
    public class HeaderPanel : Panel
    {
        public new string Text
        {
            get { return this.headerText; }
            set
            {
                this.headerText = value;
                if (this.offImg != null)
                {
                    this.offImg.Dispose();
                    this.offImg = null;
                }
                this.Invalidate();
            }
        }
        public string Description
        {
            get { return this.descriptionText; }
            set
            {
                this.descriptionText = value;
                if (this.offImg != null)
                {
                    this.offImg.Dispose();
                    this.offImg = null;
                }
                this.Invalidate();
            }
        }

        private string headerText;
        private string descriptionText;
        private Image offImg;

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.offImg == null)
            {
                this.offImg = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);

                using (Graphics g = Graphics.FromImage(this.offImg))
                {
                    Gradient.GradientFill.Fill(g, new Rectangle(0, 0, this.Width, this.Height), Color.FromArgb(80, 80, 80), Color.FromArgb(20, 20, 20), Gradient.FillDirection.TopToBottom);

                    Font f = new Font(Settings.FontName, 10, FontStyle.Bold);
                    Font f2 = new Font(Settings.FontName, 9, FontStyle.Bold);
                    SolidBrush br = new SolidBrush(this.ForeColor);
                    SizeF s = g.MeasureString(this.headerText, f);
                    Point p1 = new Point(), p2 = new Point();
                    if (this.descriptionText == null)
                    {
                        p1.X = (int)(this.Width - s.Width) / 2;
                        p1.Y = (int)(this.Height - s.Height) / 2;
                        p2.X = 0;
                        p2.Y = 0;
                    }
                    else
                    {
                        SizeF s2 = g.MeasureString(this.descriptionText, f2);
                        p1.X = (int)(this.Width - s.Width) / 2;
                        p1.Y = (int)(this.Height / 2 - s.Height) / 2;
                        p2.X = (int)(this.Width - s2.Width) / 2;
                        p2.Y = (int)(this.Height / 2 - s.Height) / 2 + this.Height / 2;
                    }
                    g.DrawString(this.headerText, f, br, p1.X, p1.Y);
                    if (this.descriptionText != null)
                    {
                        using (SolidBrush br2 = new SolidBrush(Color.FromArgb(192, 192, 192)))
                        {
                            g.DrawString(this.descriptionText, f2, br2, p2.X, p2.Y);
                        }
                    }
                    br.Dispose();
                    f2.Dispose();
                    f.Dispose();
                }
            }

            e.Graphics.DrawImage(this.offImg, 0, 0);
        }

        protected override void OnResize(EventArgs e)
        {
            if (this.offImg != null)
            {
                this.offImg.Dispose();
                this.offImg = null;
            }
            base.OnResize(e);
            this.Invalidate();
        }
    }
}
