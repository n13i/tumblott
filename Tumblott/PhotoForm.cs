using System;
using System.Windows.Forms;
using Tumblott.Client.Tumblr;

namespace Tumblott
{
    /// <summary>
    /// Photo 拡大表示用フォーム
    /// </summary>
    public partial class PhotoForm : Form
    {
        // TODO
        // ズームサイズ変更 (Fill, Maximize, 1:1)
        // 回転

        private TumblrPost post;

        private bool isImageLoaded = false;

        public PhotoForm()
        {
            InitializeComponent();

            Menu = new MainMenu();

            MenuItem miClose = new MenuItem { Text = Messages.Close };
            miClose.Click += new EventHandler(miClose_Click);
            Menu.MenuItems.Add(miClose);

            this.Load += new EventHandler(PhotoForm_Load);
        }

        void PhotoForm_Load(object sender, EventArgs e)
        {
            if (isImageLoaded)
            {
                loadingIndicator.Visible = false;
            }
            else
            {
                loadingIndicator.Visible = true;
                loadingIndicator.Start();
            }
        }

        public void SetPost(TumblrPost post)
        {
            this.post = post;
            imageView.Image = null;
            loadingIndicator.Visible = true;
            isImageLoaded = false;
            LoadImage();
        }

        void miClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadImage()
        {
            if (post.LargeImage == null)
            {
                // GetImageを呼ぶとMainFormのLoadingIndicatorが動作する
                post.GetImage(TumblrPost.ImageType.Large, TumblrPost.Priority.High, null, p => { this.BeginInvoke(new Action<TumblrPost>(LoadImageDone), (TumblrPost)(((JobResult)p).Object)); });
            }
            else
            {
                LoadImageDone(post);
            }
        }

        private void LoadImageDone(TumblrPost post)
        {
            Utils.DebugLog("LoadImageDone");

            loadingIndicator.Visible = false;
            imageView.Image = post.LargeImage;

            isImageLoaded = true;
        }
    }
}
