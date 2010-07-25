using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;

namespace Tumblott.TextViewer
{
    /// <summary>
    /// 簡単なテキストのレンダリングを行う
    /// </summary>
    public class TextRenderer : IDisposable
    {
        // メニュー画面もこれで作れるようにしたい(2010/04/29)

        private Image img;

        public Image RenderedImage
        {
            get { return img; }
        }

        public TextDocument Document { get; set; }

        public TextRenderer()
        {
            this.Document = new TextDocument();
        }

        public void Dispose()
        {
            this.Document.Dispose();
            this.Document = null;
        }

        /// <summary>
        /// レンダリングを行う
        /// </summary>
        /// <param name="width"></param>
        public void Render(int width)
        {
            // TODO 必要な部分だけレンダリングできるようにしたい

            // レイアウトの決定
            using (Image tmpImg = new Bitmap(32, 32, PixelFormat.Format16bppRgb565))
            {
                using (Graphics gtmp = Graphics.FromImage(tmpImg))
                {
                    this.Document.Layout(gtmp, width);
                }
            }

            // レイアウトに基づくサイズの Bitmap を作成
            if (img != null) { img.Dispose(); }
            img = new Bitmap(this.Document.Rect.Width, this.Document.Rect.Height, PixelFormat.Format16bppRgb565);

            // 実際の描画処理
            using (Graphics g = Graphics.FromImage(img))
            {
                using (SolidBrush brush = new SolidBrush(Color.White))
                {
                    g.FillRectangle(brush, 0, 0, img.Width, img.Height);
                    this.Document.Draw(g);
                }
            }
        }
    }

    // ----------------------------------------------------------------------
    public class TextDocument : IDisposable
    {
        public List<TextElement> Elements { get; set; }
        public Rectangle Rect { get; private set; }

        public TextDocument()
        {
            this.Elements = new List<TextElement>();
        }

        public void Dispose()
        {
            foreach (var el in this.Elements)
            {
                el.Dispose();
            }
            this.Elements = null;
        }

        public void AddElement(TextElement element)
        {
            this.Elements.Add(element);
        }

        public void Layout(Graphics g, int width)
        {
            Point p = new Point(0, 0);

            if (this.Elements.Count == 0)
            {
                // とりあえず適当なサイズにしておく
                this.Rect = new Rectangle(0, 0, width, 1);
                return;
            }

            TextElement lastElement = null;
            foreach (var element in this.Elements)
            {
                if (lastElement != null)
                {
                    p.X = lastElement.NextElementPosition.X;
                    p.Y = lastElement.NextElementPosition.Y;
                }

                element.Layout(g, width, p);

                lastElement = element;
            }

            // 最後の要素の描画位置から文章全体の高さを得る
            this.Rect = new Rectangle(0, 0, width, lastElement.Rect.Y + lastElement.Rect.Height);
        }

        public void Draw(Graphics g)
        {
            // 各要素を描画
            foreach (var element in this.Elements)
            {
                element.Draw(g);
            }
            //g.DrawRectangle(new Pen(Color.LightGreen), this.Rect);
        }

        public TextElement GetElementFromPoint(Point point)
        {
            foreach (var element in this.Elements)
            {
                if (element.IsLocatedAt(point))
                {
                    return element;
                }
            }

            return null;
        }
    }

    // ----------------------------------------------------------------------
    public class TextElement : IDisposable
    {
        public List<TextBand> Bands { get; set; }

        public Rectangle Rect { get; set; }
        public Point NextElementPosition { get; set; }

        public FontStyle FontStyle { get; set; }
        public float FontSize { get; set; }
        public Color Color { get; set; }
        public int LeftMargin { get; set; }

        private string text;
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                string tmp = value;
                tmp = tmp.Replace("\r", "");
                tmp = tmp.Replace("\n", "");
                tmp = tmp.Replace("\t", "");
                this.text = tmp;
            }
        }

        private Font font;

        public TextElement()
        {
            this.Bands = new List<TextBand>();
            this.Rect = new Rectangle();
            this.FontStyle = FontStyle.Regular;
            this.FontSize = 9F;
            this.Color = Color.FromArgb(68, 68, 68);
        }

        public void Dispose()
        {
            this.Bands = null;
        }

        public virtual void Layout(Graphics g, int maxWidth, Point pos)
        {
            this.Bands.Clear();

            Font font = new Font("Tahoma", this.FontSize, this.FontStyle);
            Color color = this.Color;

            this.font = font;

            // 高さの基準
            // FIXME 行間調整できるように
            SizeF baseSize = g.MeasureString("M", font);
            int baseHeight = (int)Math.Ceiling(baseSize.Height) + 2;

            int ipos = 0;
            Point p = new Point(pos.X, pos.Y);
            while (true)
            {
                // pos から何文字描画できるか
                int len = 0;
                while (len <= 0)
                {
                    len = GetStringLength(this.Text.Substring(ipos), maxWidth - p.X, g, font);
                    if (len == 0)
                    {
                        p.X = 0;
                        p.Y += baseHeight;
                    }
                }

                string substr = this.Text.Substring(ipos, len);

                SizeF size = g.MeasureString(substr, font);
                Size isize = new Size((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));

                TextBand band = new TextBand();
                band.Rect = new Rectangle(p.X, p.Y, isize.Width, isize.Height);
                band.Text = substr;

                this.Rect = new Rectangle(0, pos.Y, maxWidth, p.Y - pos.Y + baseHeight);

                if (len == this.Text.Substring(ipos).Length)
                {
                    // 全部描ける場合
                    p.X += isize.Width;
                }
                else
                {
                    // 折り返す場合
                    p.X = 0;
                    p.Y += baseHeight;
                }

                this.Bands.Add(band);
                //Utils.DebugLog(isize.Width);

                this.NextElementPosition = new Point(p.X, p.Y);

                ipos += len;
                if (ipos >= this.Text.Length) { break; }
            }
        }

        public void Draw(Graphics g)
        {
            foreach (var band in this.Bands)
            {
                band.Draw(g, this.font, this.Color);
            }
            //g.DrawRectangle(new Pen(Color.LightPink), this.Rect);
        }

        public bool IsLocatedAt(Point p)
        {
            foreach (var b in this.Bands)
            {
                if (b.Rect.Contains(p))
                {
                    return true;
                }
            }

            return false;
        }

        private static int GetStringLength(string str, int maxWidth, Graphics g, Font font)
        {
            int delta = str.Length;
            int vector = 1;
            int len = 0;

            while (true)
            {
                len += vector * delta;
                //Utils.DebugLog(len);

                float width = g.MeasureString(str.Substring(0, len), font).Width;

                // 画面幅に全部収まる場合
                if (width <= maxWidth && len == str.Length)
                {
                    break;
                }

                if (delta > 1)
                {
                    if (width > maxWidth)
                    {
                        vector = -1;
                    }
                    else
                    {
                        vector = 1;
                    }

                    delta /= 2;
                    if (delta < 1) { delta = 1; }
                }
                else
                {
                    // +方向へ
                    if (vector == 1 && width > maxWidth)
                    {
                        len = len - 1;
                        break;
                    }
                    else if(vector == -1 && width <= maxWidth)
                    {
                        break;
                    }
                }
            }

            return len;
        }
    }

    public class TextBreakElement : TextElement
    {
        public override void Layout(Graphics g, int maxWidth, Point pos)
        {
            Font font = new Font("Tahoma", this.FontSize, this.FontStyle);
            SizeF baseSize = g.MeasureString("M", font);
            int baseHeight = (int)Math.Ceiling(baseSize.Height) + 2;

            this.Rect = new Rectangle(0, pos.Y, 0, baseHeight);
            this.NextElementPosition = new Point(0, pos.Y + baseHeight);

            font.Dispose();
        }
    }

    public class TextLinkElement : TextElement
    {
        public string Url { get; set; }
        public TextLinkElement()
        {
            this.Color = Color.FromArgb(68, 68, 68);
            this.FontStyle = FontStyle.Underline;
        }
    }

    // ----------------------------------------------------------------------
    public class TextBand
    {
        public Rectangle Rect { get; set; }
        public string Text { get; set; }

        public void Draw(Graphics g, Font font, Color color)
        {
            using (SolidBrush brush = new SolidBrush(color))
            {
                g.DrawString(this.Text, font, brush, this.Rect.X, this.Rect.Y);
            }
        }
    }
}
