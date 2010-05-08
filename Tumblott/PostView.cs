using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Tumblott.Client.Tumblr;
using Tumblott.TextViewer;

namespace Tumblott.Forms
{
    // TumblrPostを表示するコントロール
    public partial class PostView : UserControl
    {
        private struct Velocity
        {
            public double X;
            public double Y;
        }

        private TumblrPost post;

        private bool isMouseDown;
        private Point firstTapPoint = new Point();
        private int totalMouseMove = 0;
        private Point mouseDown = new Point();
        private Point drawPos = new Point();

        private string selectedUrl = null;

        private Image renderedImg;
        private Image scaledImg;
        private Image avatarImg;
        private Size avatarImgSize = new Size(64, 64);
        private Image loadingImg;

        private Image offBuf;
        private Image frameTop;
        private Image frameBottom;
        private float imgScale = 1F;

        private Rectangle scrollArea = new Rectangle();

        private TextRenderer renderer = new TextRenderer();
        private Rectangle textViewArea = new Rectangle(); // テキストビューの実際の座標
        private Rectangle pictureViewArea = new Rectangle();

        private Velocity scrollVelocity = new Velocity();
        private Point tapPosition = new Point();
        private Point prevTapPostion = new Point();

        private Thread thread;

        // 画面切り替えアニメーション用
        private Image flipOffBuf;
        private bool isFlipping = false;
        private Point flipOffset;
        private Velocity flipVelocity;

        private string text;
        public new string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                if (this.Visible)
                {
                    Invalidate();
                }
            }
        }

        public TumblrPost Post
        {
            set
            {
                post = value;
                if (renderedImg != null)
                {
                    renderedImg.Dispose();
                    renderedImg = null;
                }
                if (scaledImg != null)
                {
                    scaledImg.Dispose();
                    scaledImg = null;
                }
                if (avatarImg != null)
                {
                    avatarImg.Dispose();
                    avatarImg = null;
                }
                this.renderer.Document.Dispose();
                this.renderer.Document = new TextDocument();
                Parse(this.renderer.Document, post.Html);
                Invalidate();
            }
        }

        public event EventHandler ImageClicked;
        public event EventHandler LinkClicked;

        public PostView()
        {
            InitializeComponent();

            this.ContextMenu = new ContextMenu();
            this.ContextMenu.Popup +=new EventHandler(ContextMenu_Popup);
            // コンテキストメニュー項目の作成は ContextMenu_Popup 内で行う

            post = null;

            // ここでやるとレイアウト後のサイズが反映されない？
            //PrepareImages();

            this.HandleDestroyed += new EventHandler(PostView_HandleDestroyed);

            this.thread = new Thread(new ThreadStart(Scroll));
            this.thread.IsBackground = true;
            this.thread.Start();
        }

        void PostView_HandleDestroyed(object sender, EventArgs e)
        {
            this.thread.Abort();
        }

        /// <summary>
        /// スクロールとその他アニメーション用スレッドメソッド
        /// </summary>
        private void Scroll()
        {
            Point delta = new Point(0, 0);

            while (true)
            {
                if (isMouseDown)
                {
                    object o = new object();
                    lock (o)
                    {
                        scrollVelocity.X = tapPosition.X - prevTapPostion.X;
                        scrollVelocity.Y = tapPosition.Y - prevTapPostion.Y;
                        prevTapPostion.X = tapPosition.X;
                        prevTapPostion.Y = tapPosition.Y;
                    }
                }
                else
                {
                    if (scrollVelocity.Y != 0)
                    {
                        delta.X = 0;
                        delta.Y = (int)(scrollVelocity.Y);
                        this.BeginInvoke(new Action<Point>(ScrollTo), delta);
                        scrollVelocity.Y = scrollVelocity.Y * 0.95;
                    }
                }

                if (isFlipping)
                {
                    this.BeginInvoke(new Action<int>(DoFlip), 0);
                }

                Thread.Sleep(33); // expects 30fps
            }
        }

        public void ResetPosition()
        {
            drawPos.X = 0;
            drawPos.Y = 0;
            scrollVelocity.X = 0;
            scrollVelocity.Y = 0;
            Invalidate();
        }

        public enum FlipDirection { Left, Right, Up, Down };

        /// <summary>
        /// 画面切り替え動作を開始する
        /// </summary>
        /// <param name="dir"></param>
        public void StartFlip(FlipDirection dir)
        {
            if (flipOffBuf != null)
            {
                flipOffBuf.Dispose();
            }
            // 現在の描画内容をコピー
            flipOffBuf = new Bitmap(offBuf);

            flipOffset.X = 0;
            flipOffset.Y = 0;

            switch (dir)
            {
                case FlipDirection.Left:
                    flipVelocity.X = -0.3 * this.Width;
                    flipVelocity.Y = 0;
                    break;

                case FlipDirection.Right:
                    flipVelocity.X = 0.3 * this.Width;
                    flipVelocity.Y = 0;
                    break;

                case FlipDirection.Up:
                    flipVelocity.X = 0;
                    flipVelocity.Y = -0.3 * this.Height;
                    break;

                case FlipDirection.Down:
                    flipVelocity.X = 0;
                    flipVelocity.Y = 0.3 * this.Height;
                    break;
            }

            isFlipping = true;
        }

        private void DoFlip(int dummy)
        {
            if (isFlipping)
            {
                flipOffset.X += (int)(flipVelocity.X);
                flipOffset.Y += (int)(flipVelocity.Y);

                flipVelocity.X *= 0.8;
                flipVelocity.Y *= 0.8;
                if (flipVelocity.X > 0 && flipVelocity.X < 8) { flipVelocity.X = 8; }
                if (flipVelocity.Y > 0 && flipVelocity.Y < 8) { flipVelocity.Y = 8; }

                if (flipOffset.X < -this.Width || flipOffset.X > this.Width ||
                    flipOffset.Y < -this.Height || flipOffset.Y > this.Height)
                {
                    isFlipping = false;
                }
            }
            Invalidate();
        }

        private void PostView_MouseDown(object sender, MouseEventArgs e)
        {
            if (post != null && !isMouseDown)
            {
                mouseDown.X = e.X;
                mouseDown.Y = e.Y;
                isMouseDown = true;
                Invalidate();

                tapPosition.X = e.X;
                tapPosition.Y = e.Y;
                prevTapPostion.X = e.X;
                prevTapPostion.Y = e.Y;
                scrollVelocity.X = 0;
                scrollVelocity.Y = 0;

                firstTapPoint.X = e.X;
                firstTapPoint.Y = e.Y;
                totalMouseMove = 0;
            }
        }

        private void PostView_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                // スクロール用スレッドで利用
                tapPosition.X = e.X;
                tapPosition.Y = e.Y;

                totalMouseMove += (Math.Abs(e.X - mouseDown.X) + Math.Abs(e.Y - mouseDown.Y));

                ScrollTo(new Point(e.X - mouseDown.X, e.Y - mouseDown.Y));

                mouseDown.X = e.X;
                mouseDown.Y = e.Y;
            }
        }

        private void PostView_MouseUp(object sender, MouseEventArgs e)
        {
            if (isMouseDown == true && totalMouseMove < (int)(5 * imgScale)) // 要調整
            {
                if (pictureViewArea.Contains(firstTapPoint.X, firstTapPoint.Y))
                {
                    Utils.DebugLog("MouseUp: ImageClicked");
                    EventHandler evh = ImageClicked;
                    if (evh != null)
                    {
                        evh(this, e);
                    }
                }
                else if (textViewArea.Contains(firstTapPoint.X, firstTapPoint.Y))
                {
                    Point p = new Point();
                    p.X = e.X - this.textViewArea.X;
                    p.Y = e.Y - this.textViewArea.Y;
                    Utils.DebugLog("MouseUp: x=" + p.X.ToString() + ", y=" + p.Y.ToString());

                    TextElement el = this.renderer.Document.GetElementFromPoint(p);
                    if (el != null)
                    {
                        if (el.GetType() == typeof(TextLinkElement))
                        {
                            TextLinkElement link = (TextLinkElement)el;
                            //MessageBox.Show(link.Url);
                            // TODO LinkClickedみたいなイベントで渡す
                            OpenUrl(link.Url);
                        }
                    }
                }
            }
            
            Invalidate();
            isMouseDown = false;
        }

        void ContextMenu_Popup(object sender, EventArgs e)
        {
            isMouseDown = false;

            if (textViewArea.Contains(tapPosition.X, tapPosition.Y))
            {
                Point p = new Point();
                p.X = tapPosition.X - this.textViewArea.X;
                p.Y = tapPosition.Y - this.textViewArea.Y;
                TextElement el = this.renderer.Document.GetElementFromPoint(p);
                if (el != null)
                {
                    if (el.GetType() == typeof(TextLinkElement))
                    {
                        TextLinkElement link = (TextLinkElement)el;

                        // コンテキストメニューを作成
                        this.ContextMenu.MenuItems.Clear();

                        MenuItem miUrl = new MenuItem { Text = link.Url, Enabled = false };
                        this.ContextMenu.MenuItems.Add(miUrl);

                        this.ContextMenu.MenuItems.Add(new MenuItem { Text = "-" });

                        MenuItem miCopyUrl = new MenuItem { Text = Messages.CopyURL };
                        miCopyUrl.Click += new EventHandler(miCopyUrl_Click);
                        this.ContextMenu.MenuItems.Add(miCopyUrl);

                        MenuItem miOpenUrl = new MenuItem { Text = Messages.Open };
                        miOpenUrl.Click += new EventHandler(miOpenUrl_Click);
                        this.ContextMenu.MenuItems.Add(miOpenUrl);

                        //MessageBox.Show(link.Url);
                        // コンテキストメニューが表示された時点でのリンク先URLを保持しておく
                        // [URLをコピー]とか選んだ時点でこのURLをクリップボードにセット
                        selectedUrl = link.Url;
                    }
                }
            }
        }

        void miOpenUrl_Click(object sender, EventArgs e)
        {
            OpenUrl(selectedUrl);
        }

        void miCopyUrl_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(selectedUrl);
        }

        private void OpenUrl(string url)
        {
            if (Settings.IsConfirmWhenOpenLinks)
            {
                DialogResult dr = MessageBox.Show("Open " + url + " ?", "Open URL", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (dr == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(url, null);
                }
            }
            else
            {
                System.Diagnostics.Process.Start(url, null);
            }
        }

        private void ScrollTo(Point delta)
        {
            drawPos.X += delta.X;
            drawPos.Y += delta.Y;
            if (drawPos.Y < -(scrollArea.Height - this.Height)) { drawPos.Y = -(scrollArea.Height - this.Height); }
            if (drawPos.Y > 0) { drawPos.Y = 0; }

            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        private void PrepareImages(Graphics gr)
        {
            float ratio = gr.DpiX / 192F;
            
            Image frameTLImg = global::Tumblott.Properties.Resources.frame_top_left;
            Image frameTRImg = global::Tumblott.Properties.Resources.frame_top_right;
            Image frameBLImg = global::Tumblott.Properties.Resources.frame_bottom_left;
            Image frameBRImg = global::Tumblott.Properties.Resources.frame_bottom_right;

            frameTop = new Bitmap(this.Width, (int)(frameTLImg.Height * ratio), System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            frameBottom = new Bitmap(this.Width, (int)(frameBLImg.Height*ratio), System.Drawing.Imaging.PixelFormat.Format16bppRgb565);

            Graphics g;
            SolidBrush bBgColor = new SolidBrush(Color.FromArgb(44, 71, 98));
            SolidBrush bWhite = new SolidBrush(Color.White);
            Rectangle dstRect, srcRect;

            using (g = Graphics.FromImage(frameTop))
            {
                g.FillRectangle(bBgColor, 0, 0, frameTop.Width, frameTop.Height);
                g.FillRectangle(bWhite, (int)(4 * ratio), (int)(20 * ratio), frameTop.Width - (int)(8 * ratio), frameTop.Height - (int)(20 * ratio));

                srcRect = new Rectangle(0, 0, frameTLImg.Width, frameTLImg.Height);
                dstRect = new Rectangle(0, 0, (int)(frameTLImg.Width * ratio), (int)(frameTLImg.Height * ratio));
                g.DrawImage(frameTLImg, dstRect, srcRect, GraphicsUnit.Pixel);

                srcRect = new Rectangle(0, 0, frameTRImg.Width, frameTRImg.Height);
                dstRect = new Rectangle(this.Width - (int)(frameTRImg.Width * ratio), 0, (int)(frameTRImg.Width * ratio), (int)(frameTRImg.Height * ratio));
                g.DrawImage(frameTRImg, dstRect, srcRect, GraphicsUnit.Pixel);
            }

            using (g = Graphics.FromImage(frameBottom))
            {
                g.FillRectangle(bBgColor, 0, 0, frameBottom.Width, frameBottom.Height);
                g.FillRectangle(bWhite, (int)(4 * ratio), 0, frameBottom.Width - (int)(8 * ratio), frameBottom.Height - (int)(4 * ratio));

                srcRect = new Rectangle(0, 0, frameBLImg.Width, frameBLImg.Height);
                dstRect = new Rectangle(0, 0, (int)(frameBLImg.Width * ratio), (int)(frameBLImg.Height * ratio));
                g.DrawImage(frameBLImg, dstRect, srcRect, GraphicsUnit.Pixel);

                srcRect = new Rectangle(0, 0, frameBRImg.Width, frameBRImg.Height);
                dstRect = new Rectangle(this.Width - (int)(frameBRImg.Width * ratio), 0, (int)(frameBRImg.Width * ratio), (int)(frameBRImg.Height * ratio));
                g.DrawImage(frameBRImg, dstRect, srcRect, GraphicsUnit.Pixel);
            }

            this.imgScale = ratio;

            loadingImg = global::Tumblott.Properties.Resources.loading_b0;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (offBuf == null)
            {
                offBuf = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            }

            if (frameTop == null)
            {
                PrepareImages(e.Graphics);
            }

            Graphics g = Graphics.FromImage(offBuf);

            //SolidBrush bBgColor = new SolidBrush(Color.FromArgb(44, 71, 98));
            SolidBrush bBgColor = new SolidBrush(this.BackColor);
            SolidBrush bWhite = new SolidBrush(Color.White);
            SolidBrush bAvatarBG = new SolidBrush(this.BackColor);

            // まずは背景を塗りつぶす
            g.FillRectangle(bBgColor, 0, 0, this.Width, this.Height);

            // ヘッダ部の描画
            // アバター画像部
            g.FillRectangle(bAvatarBG, (int)(4 * imgScale), (int)(4 * imgScale) + drawPos.Y, (int)(this.avatarImgSize.Width * imgScale), (int)(this.avatarImgSize.Height * imgScale));
            Font font = new Font("MS UI Gothic", 9F, FontStyle.Regular);
            //SizeF size = g.MeasureString("X", font);
            g.DrawString(text, font, bWhite, (4+64+4)*imgScale, (4*imgScale) + drawPos.Y);

            //int headerHeight = (int)(size.Height + (4*imgScale));
            int headerHeight = (int)((4+64+4)*imgScale);

            // 白い領域
            Rectangle drawArea = new Rectangle();
            drawArea.X = (int)(4*imgScale);
            drawArea.Y = headerHeight + (int)(4*imgScale);
            drawArea.Width = this.Width - (int)(8*imgScale);
            drawArea.Height = (int)(32*imgScale);
            // 白い領域内の描画エリア
            Rectangle drawInnerArea = new Rectangle();
            drawInnerArea.X = drawArea.X + (int)(16*imgScale);
            drawInnerArea.Y = drawArea.Y + (int)(16*imgScale);
            drawInnerArea.Width = drawArea.Width - (int)(32*imgScale);
            drawInnerArea.Height = drawArea.Height - (int)(32*imgScale);

            int pictureFramePadding = (int)(8 * imgScale);

            int textPosY = 0;

            if(post != null)
            {
                if (post.Type == TumblrPost.Types.Photo)
                {
                    if (post.Image != null)
                    {
                        if (scaledImg == null)
                        {
                            scaledImg = Utils.GetScaledImage(post.Image, drawInnerArea.Width - pictureFramePadding*2, this.Height - pictureFramePadding*2 - drawInnerArea.Y, Utils.ScaleMode.Fit);
                        }
                        textPosY += scaledImg.Height + pictureFramePadding*2;
                        drawArea.Height += scaledImg.Height + pictureFramePadding*2;
                        drawInnerArea.Height += scaledImg.Height + pictureFramePadding*2;
                    }
                    else
                    {
                        // 読み込み中画像のためのスペースを確保
                        textPosY += loadingImg.Height + pictureFramePadding*2;
                        drawArea.Height += loadingImg.Height + pictureFramePadding*2;
                        drawInnerArea.Height += loadingImg.Height + pictureFramePadding*2;
                    }
                }

                if (post.AvatarImage != null)
                {
                    if (avatarImg == null)
                    {
                        avatarImg = Utils.GetScaledImage(post.AvatarImage, (int)(this.avatarImgSize.Width * imgScale), (int)(this.avatarImgSize.Height * imgScale), Utils.ScaleMode.Full);
                    }
                }

                if (renderedImg == null)
                {
                    // テキストのレンダリング
                    // FIXME 別スレッドでやりたい(レンダリング中につっかえるのを避けたい)
                    this.renderer.Render(drawInnerArea.Width);
                    renderedImg = this.renderer.RenderedImage;
                }

                if (renderedImg != null)
                {
                    drawArea.Height += renderedImg.Height;
                    drawInnerArea.Height += renderedImg.Height;
                }
            }

            // 小さい場合，吹き出しの形が崩れないサイズにする
            if (drawArea.Height < (int)(56 * imgScale))
            {
                drawArea.Height = (int)(56 * imgScale);
                drawInnerArea.Height = drawArea.Height - (int)(32 * imgScale);
            }

            g.FillRectangle(bWhite, drawArea.X, drawArea.Y + drawPos.Y, drawArea.Width, drawArea.Height);
            //g.DrawImage(frameTop, 0, drawArea.Y - (int)(20*imgScale) + drawPos.Y);
            //g.DrawImage(frameBottom, 0, drawArea.Y + drawArea.Height + (int)(4*imgScale) - frameBottom.Height + drawPos.Y);

            if (post != null)
            {
                // ヘッダ部にアバター画像を表示
                if (avatarImg != null)
                {
                    g.DrawImage(avatarImg, (int)(4*imgScale), (int)(4*imgScale) + drawPos.Y);
                }
                // Photo を表示
                if (post.Type == TumblrPost.Types.Photo)
                {
                    Image img = null;
                    if (scaledImg != null)
                    {
                        img = scaledImg;
                    }
                    else
                    {
                        img = loadingImg;
                    }

                    pictureViewArea = new Rectangle(drawInnerArea.X, drawInnerArea.Y + drawPos.Y, img.Width + pictureFramePadding*2, img.Height + pictureFramePadding*2);
                    using (Pen p = new Pen(Color.LightGray))
                    {
                        g.DrawRectangle(p, pictureViewArea.X, pictureViewArea.Y, pictureViewArea.Width-1, pictureViewArea.Height-1);
                    }
                    g.DrawImage(img, pictureViewArea.X + pictureFramePadding, pictureViewArea.Y + pictureFramePadding);
                }
                else
                {
                    pictureViewArea = new Rectangle(0, 0, 0, 0);
                }

                // テキストがレンダリングされた画像を描画
                textViewArea = new Rectangle(drawInnerArea.X, drawInnerArea.Y + textPosY + drawPos.Y, renderedImg.Width, renderedImg.Height);
                g.DrawImage(renderedImg, textViewArea.X, textViewArea.Y);
                /*
                using (Pen p = new Pen(Color.Red))
                {
                    g.DrawRectangle(p, textViewArea);
                }
                */
            }

            // フリップ動作中
            if (isFlipping && flipOffBuf != null)
            {
                g.DrawImage(flipOffBuf, flipOffset.X, flipOffset.Y);
            }

            // オフスクリーンバッファを画面へ描画
            e.Graphics.DrawImage(offBuf, 0, 0);

            scrollArea.X = 0;
            scrollArea.Y = 0;
            scrollArea.Width = this.Width;
            scrollArea.Height = headerHeight + drawArea.Height + (int)(24*imgScale) + (int)(32*imgScale); // 32 は若干の余裕

            bBgColor.Dispose();
            bWhite.Dispose();
            bAvatarBG.Dispose();
            g.Dispose();
        }

        private void Parse(TextDocument doc, string html)
        {
            string xml = html;

            xml = Utils.ReplaceCharacterEntityReferences(xml);
            xml = Utils.RemoveWhiteSpaces(xml);

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(xml);
            }
            catch (Exception e)
            {
                Utils.DebugLog(e);
            }
            xmlDoc.Normalize();
            TraverseNodes(doc, xmlDoc.DocumentElement, null);
        }

        private void TraverseNodes(TextDocument doc, XmlNode node, XmlNode parent)
        {
            if (node.HasChildNodes)
            {
                if (node.Name == "p" || node.Name == "tr")
                {
                    doc.AddElement(new TextBreakElement());
                    doc.AddElement(new TextBreakElement());
                }

                foreach (XmlNode n in node.ChildNodes)
                {
                    TraverseNodes(doc, n, node);
                }
            }
            else
            {
                // 子ノードを持たない要素
                if (node.NodeType == XmlNodeType.Text)
                {
                    TextElement e;
                    if (parent.Name == "a")
                    {
                        XmlNode href = parent.Attributes.GetNamedItem("href");
                        if (href != null)
                        {
                            e = new TextLinkElement { Url = href.Value };
                            if (parent.ParentNode.Name == "b")
                            {
                                e.FontStyle |= FontStyle.Bold;
                            }
                        }
                        else
                        {
                            e = new TextElement();
                        }
                    }
                    else
                    {
                        e = new TextElement();
                    }

                    if (parent.Name == "b" || parent.Name == "strong")
                    {
                        e.FontStyle = FontStyle.Bold;
                    }

                    e.Text = node.Value;
                    doc.AddElement(e);
                }
                else if (node.NodeType == XmlNodeType.Element)
                {
                    //Utils.DebugLog(node.Name);

                    if (node.Name == "br")
                    {
                        doc.AddElement(new TextBreakElement());
                    }
                }
            }
        }

        private void PostView_Resize(object sender, EventArgs e)
        {
            //MessageBox.Show("resize");

            if (offBuf != null)
            {
                offBuf.Dispose();
                offBuf = null;
            }
            if (frameTop != null)
            {
                frameTop.Dispose();
                frameTop = null;
            }
            if (frameBottom != null)
            {
                frameBottom.Dispose();
                frameBottom = null;
            }
            if (renderedImg != null)
            {
                renderedImg.Dispose();
                renderedImg = null;
            }
            if (scaledImg != null)
            {
                scaledImg.Dispose();
                scaledImg = null;
            }

            ResetPosition();

            Invalidate();
        }
    }
}
