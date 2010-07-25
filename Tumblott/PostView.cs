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
        private Point firstTappedPoint = new Point();
        private int totalMouseMove = 0;
        private Point tappedPoint = new Point();
        private Point drawPos = new Point();

        private string selectedUrl = null;

        private Image renderedImg;
        private Image scaledImg;
        private Image avatarImg;
        private Size avatarImgSize = new Size(32, 32);
        private Image loadingImg;

        private Image offBuf;
        private Image frameTop;
        private Image frameBottom;
        private SizeF scaleFactor = new SizeF(1, 1);

        private Rectangle scrollArea = new Rectangle();

        private bool isScrolling;

        private TextRenderer renderer = new TextRenderer();
        private Rectangle textViewArea = new Rectangle(); // テキストビューの実際の座標
        private Rectangle pictureViewArea = new Rectangle();

        private Point tapPosition = new Point();
        private Point prevTapPostion = new Point();

        private Thread thread;

        private Scroller scroller;

        // 画面切り替えアニメーション用
        private Image flipOffBuf;
        private bool isFlipping = false;
        private Point flipOffset;
        private Velocity flipVelocity;

        private Object lockObj = new Object();

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

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
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

                //this.ResetPosition();

                this.renderer.Document.Dispose();
                this.renderer.Document = new TextDocument();
                Parse(this.renderer.Document, post.Html);
                Invalidate();
            }
        }

        public bool HideHeader { get; set; }

        public event EventHandler ImageClicked;
        public event EventHandler LinkClicked;
        public event MouseEventHandler FlipRequested;

        public PostView()
        {
            InitializeComponent();

            this.ContextMenu = new ContextMenu();
            this.ContextMenu.Popup +=new EventHandler(ContextMenu_Popup);
            // コンテキストメニュー項目の作成は ContextMenu_Popup 内で行う

            post = null;

            isMouseDown = false;
            isScrolling = false;

            // ここでやるとレイアウト後のサイズが反映されない？
            //PrepareImages();

            this.HandleDestroyed += new EventHandler(PostView_HandleDestroyed);

            this.thread = new Thread(new ThreadStart(Animate));
            this.thread.IsBackground = true;
            this.thread.Start();

            this.scroller = new Scroller(this);
            this.scroller.Scroll += new MouseEventHandler(scroller_Scroll);
            this.scroller.ScrollStopped += new EventHandler(scroller_ScrollStopped);
            this.scroller.Flick += new MouseEventHandler(scroller_Flick);
        }

        void scroller_Flick(object sender, MouseEventArgs e)
        {
            Utils.DebugLog(e.X.ToString());
            if (this.FlipRequested != null)
            {
                this.FlipRequested(this, e);
            }
        }

        void scroller_Scroll(object sender, MouseEventArgs e)
        {
            this.Scroll(new Point(e.X, e.Y));
        }

        void scroller_ScrollStopped(object sender, EventArgs e)
        {
            Invalidate();
        }

        void PostView_HandleDestroyed(object sender, EventArgs e)
        {
            this.thread.Abort();
        }

        /// <summary>
        /// スクロールとその他アニメーション用スレッドメソッド
        /// </summary>
        private void Animate()
        {
            Point delta = new Point(0, 0);

            while (true)
            {
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
            this.scroller.StopScroll();
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
                tappedPoint.X = e.X;
                tappedPoint.Y = e.Y;
                isMouseDown = true;
                Invalidate();

                tapPosition.X = e.X;
                tapPosition.Y = e.Y;
                prevTapPostion.X = e.X;
                prevTapPostion.Y = e.Y;
                //scrollVelocity.X = 0;
                //scrollVelocity.Y = 0;

                firstTappedPoint.X = e.X;
                firstTappedPoint.Y = e.Y;
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

                totalMouseMove += (Math.Abs(e.X - tappedPoint.X) + Math.Abs(e.Y - tappedPoint.Y));

                isScrolling = true;
                //Scroll(new Point(e.X - tappedPoint.X, e.Y - tappedPoint.Y));

                tappedPoint.X = e.X;
                tappedPoint.Y = e.Y;
            }
        }

        private void PostView_MouseUp(object sender, MouseEventArgs e)
        {
            if (isMouseDown == true && totalMouseMove < (int)(3 * scaleFactor.Width)) // 要調整
            {
                if (pictureViewArea.Contains(firstTappedPoint.X, firstTappedPoint.Y))
                {
                    Utils.DebugLog("MouseUp: ImageClicked");
                    EventHandler evh = ImageClicked;
                    if (evh != null)
                    {
                        evh(this, e);
                    }
                }
                else if (textViewArea.Contains(firstTappedPoint.X, firstTappedPoint.Y))
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
            
            isMouseDown = false;
            isScrolling = false;

            Invalidate();
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

#if false
        private bool IsStopScroll(Point delta)
        {
            // FIXME offBuf使うのはどうかと
            return (drawPos.Y <= -(scrollArea.Height - offBuf.Height) || drawPos.Y >= 0);
        }
#endif

        // deltaで与えた分だけスクロールさせる
        private void Scroll(Point delta)
        {
            bool isScrollable = true;

            lock (lockObj)
            {
                drawPos.X += delta.X;
                drawPos.Y += delta.Y;

                if (drawPos.Y < -(scrollArea.Height - this.Height))
                {
                    drawPos.Y = -(scrollArea.Height - this.Height);
                    isScrollable = false;
                }
                if (drawPos.Y > 0)
                {
                    drawPos.Y = 0;
                    isScrollable = false;
                }
            }

            if (!isScrollable)
            {
                scroller.StopScroll();
            }

            Invalidate();
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            this.scaleFactor = factor;
            base.ScaleControl(factor, specified);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        private void PrepareImages(Graphics gr)
        {
            // レイアウトは96dpiで作成しスケーリングはscaleFactorに従う
            // ただし画像は192dpiでのサイズで用意

            // 100x16 @ 192dpi
            Image frameShadowImg = global::Tumblott.Properties.Resources.frame_shadow;

            this.frameTop = new Bitmap(this.Width - (int)(4 * scaleFactor.Width), (int)(frameShadowImg.Height * 0.5 * scaleFactor.Height), System.Drawing.Imaging.PixelFormat.Format16bppRgb565);

            Rectangle dstRect, srcRect;

            using (Graphics g = Graphics.FromImage(frameTop))
            {
                srcRect = new Rectangle(0, 0, frameShadowImg.Width, frameShadowImg.Height);
                for (int x = 0; x < this.frameTop.Width; x += (int)(frameShadowImg.Width * 0.5))
                {
                    dstRect = new Rectangle(x, 0, (int)(frameShadowImg.Width * 0.5 * scaleFactor.Width), (int)(frameShadowImg.Height * 0.5 * scaleFactor.Height));

                    g.DrawImage(frameShadowImg, dstRect, srcRect, GraphicsUnit.Pixel);
                }
            }

            loadingImg = global::Tumblott.Properties.Resources.image_loading;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (offBuf == null)
            {
                offBuf = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
                Utils.DebugLog("PostView: offBuf created, w=" + this.Width.ToString() + ", h=" + this.Height.ToString());
            }

            if (frameTop == null)
            {
                PrepareImages(e.Graphics);
            }

            Graphics g = Graphics.FromImage(offBuf);

            SolidBrush bBgColor = new SolidBrush(this.BackColor);
            SolidBrush bWhite = new SolidBrush(Color.White);
            SolidBrush bDescColor = new SolidBrush(Color.FromArgb(153, 153, 153));
            SolidBrush bAvatarBG = new SolidBrush(Color.Black);

            // まずは背景を塗りつぶす
            g.FillRectangle(bBgColor, 0, 0, this.Width, this.Height);

            // ヘッダ部の描画
            int headerHeight = 0;
            if (!this.HideHeader)
            {
                // アバター画像部
                g.FillRectangle(bAvatarBG, (int)(2 * scaleFactor.Width), (int)(4 * scaleFactor.Height) + drawPos.Y, (int)(this.avatarImgSize.Width * scaleFactor.Width), (int)(this.avatarImgSize.Height * scaleFactor.Height));

                Font font = new Font("Tahoma", 10F, FontStyle.Bold);
                SizeF size = g.MeasureString("X", font);
                g.DrawString(this.Title, font, bWhite, (2 + this.avatarImgSize.Width + 2) * scaleFactor.Width, (4 * scaleFactor.Height) + drawPos.Y);
                font.Dispose();

                font = new Font("Tahoma", 8F, FontStyle.Bold);
                //SizeF size = g.MeasureString("X", font);
                g.DrawString(this.Text, font, bDescColor, (2 + this.avatarImgSize.Width + 2) * scaleFactor.Width, (5 * scaleFactor.Height) + size.Height + drawPos.Y);
                font.Dispose();

                //int headerHeight = (int)(size.Height + (4*imgScale));
                headerHeight = (int)((4 + this.avatarImgSize.Height) * scaleFactor.Height);

                // ヘッダ部と白い領域の間
                headerHeight += this.frameTop.Height;
            }

            // 白い領域
            Rectangle drawArea = new Rectangle();
            drawArea.X = (int)(2 * scaleFactor.Width);
            drawArea.Y = headerHeight + (int)(2 * scaleFactor.Height);
            drawArea.Width = this.Width - (int)(4 * scaleFactor.Width);
            drawArea.Height = (int)(16 * scaleFactor.Height);
            // 白い領域内の描画エリア
            Rectangle drawInnerArea = new Rectangle();
            drawInnerArea.X = drawArea.X + (int)(8 * scaleFactor.Width);
            drawInnerArea.Y = drawArea.Y + (int)(8 * scaleFactor.Height);
            drawInnerArea.Width = drawArea.Width - (int)(16 * scaleFactor.Width);
            drawInnerArea.Height = drawArea.Height - (int)(16 * scaleFactor.Height);

            int pictureFramePadding = (int)(4 * scaleFactor.Width);

            int textPosY = 0;

            if (post != null)
            {
                if (post.Type == TumblrPost.Types.Photo)
                {
                    if (post.Image != null)
                    {
                        if (scaledImg == null)
                        {
                            scaledImg = Utils.GetScaledImage(post.Image, drawInnerArea.Width - pictureFramePadding * 2, this.Height - pictureFramePadding * 2 - drawInnerArea.Y, Utils.ScaleMode.Fit);
                        }
                        textPosY += scaledImg.Height + pictureFramePadding * 2;
                        drawArea.Height += scaledImg.Height + pictureFramePadding * 2;
                        drawInnerArea.Height += scaledImg.Height + pictureFramePadding * 2;
                    }
                    else
                    {
                        // 読み込み中画像のためのスペースを確保
                        textPosY += loadingImg.Height + pictureFramePadding * 2;
                        drawArea.Height += loadingImg.Height + pictureFramePadding * 2;
                        drawInnerArea.Height += loadingImg.Height + pictureFramePadding * 2;
                    }
                }

                if (post.AvatarImage != null)
                {
                    if (avatarImg == null)
                    {
                        avatarImg = Utils.GetScaledImage(post.AvatarImage, (int)(this.avatarImgSize.Width * scaleFactor.Width), (int)(this.avatarImgSize.Height * scaleFactor.Height), Utils.ScaleMode.Full);
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
            if (drawArea.Height < (int)(28 * scaleFactor.Height))
            {
                drawArea.Height = (int)(28 * scaleFactor.Height);
                drawInnerArea.Height = drawArea.Height - (int)(16 * scaleFactor.Height);
            }

            // ヘッダと描画部の間
            if (!this.HideHeader)
            {
                g.DrawImage(this.frameTop, (int)(2 * scaleFactor.Width), drawArea.Y - this.frameTop.Height + drawPos.Y);
            }
            // 描画部(白いエリア)
            g.FillRectangle(bWhite, drawArea.X, drawArea.Y + drawPos.Y, drawArea.Width, drawArea.Height);

            if (post != null)
            {
                // ヘッダ部にアバター画像を表示
                if (avatarImg != null && !this.HideHeader)
                {
                    g.DrawImage(avatarImg, (int)(2 * scaleFactor.Width), (int)(4 * scaleFactor.Height) + drawPos.Y);
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

                    pictureViewArea = new Rectangle(drawInnerArea.X, drawInnerArea.Y + drawPos.Y, img.Width + pictureFramePadding * 2, img.Height + pictureFramePadding * 2);
                    using (Pen p = new Pen(Color.LightGray))
                    {
                        g.DrawRectangle(p, pictureViewArea.X, pictureViewArea.Y, pictureViewArea.Width - 1, pictureViewArea.Height - 1);
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

            // フリックスレッド向けに値を設定
            scrollArea.X = 0;
            scrollArea.Y = 0;
            scrollArea.Width = this.Width;
            scrollArea.Height = headerHeight + drawArea.Height + (int)(12 * scaleFactor.Height) + (int)(16 * scaleFactor.Height); // 32 は若干の余裕

            // スクロールバーを描画
            if(isScrolling || scroller.IsScrolling)
            {
                int sbHeight = offBuf.Height * offBuf.Height / scrollArea.Height;
                int sbPosY = -drawPos.Y * offBuf.Height / scrollArea.Height;

                Point[] points = {
                                     new Point(this.Width-1, sbPosY-(int)(4*scaleFactor.Height)),
                                     new Point(this.Width-(int)(4*scaleFactor.Width), sbPosY),
                                     new Point(this.Width-(int)(4*scaleFactor.Width), sbPosY+sbHeight),
                                     new Point(this.Width-1, sbPosY+sbHeight+(int)(4*scaleFactor.Height)),
                                 };

                Pen p = new Pen(Color.Black);
                SolidBrush b = new SolidBrush(Color.Gray);
                //g.FillRectangle(b, this.Width - (int)(4 * imgScale), sbPosY, (int)(4 * imgScale), sbHeight);
                g.FillPolygon(b, points);
                g.DrawPolygon(p, points);

                p.Dispose();
                b.Dispose();
            }

            // フリップ動作中
            if (isFlipping && flipOffBuf != null)
            {
                g.DrawImage(flipOffBuf, flipOffset.X, flipOffset.Y);
            }

            // オフスクリーンバッファを画面へ描画
            e.Graphics.DrawImage(offBuf, 0, 0);

            if (Settings.DebugLog)
            {
                scroller.DrawStatus(e.Graphics);
            }

            bBgColor.Dispose();
            bWhite.Dispose();
            bDescColor.Dispose();
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
                // FIXME
                Utils.DebugLog(e);
                return;
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

        private void PostView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    Scroll(new Point(0, 10));
                    break;
                case Keys.Up:
                    Scroll(new Point(0, -10));
                    break;
            }
        }
    }
}
