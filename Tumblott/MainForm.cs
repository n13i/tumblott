using System;
using System.Windows.Forms;
using System.IO;
using Tumblott.Client.Tumblr;
using Tumblott.Forms;

namespace Tumblott
{
    public partial class MainForm : Form
    {
        private TumblrPosts posts = new TumblrPosts();

        private int currentView = 0;
        private int viewWidth = 240;
        private int viewHeight = 240;

        private PhotoForm photoForm;

        private MenuItem miLeftSoftKey;
        private MenuItem miRightSoftKey;

        private bool isLandscape = false;

        public MainForm()
        {
            InitializeComponent();

            Settings.Load();

            //this.MouseDown += new MouseEventHandler(MainForm_MouseDown);
            //webBrowser1.MouseDown += new MouseEventHandler(webBrowser1_MouseDown);
            // メニューの作成
            this.Menu = new MainMenu();
            miRightSoftKey = new MenuItem { Text = Messages.Menu };

            MenuItem miReload = new MenuItem { Text = "Load dashboard" };
            miReload.Click += new EventHandler(miReload_Click);

            MenuItem miSettings = new MenuItem { Text = Messages.Settings };
            miSettings.Click += new EventHandler(miSettings_Click);

            MenuItem miExit = new MenuItem { Text = Messages.Exit };
            miExit.Click += new EventHandler(miExit_Click);

            // debug menu
            MenuItem miDebug = new MenuItem { Text = "Debug" };

            //MenuItem miCookieTest = new MenuItem { Text = "Cookie" };
            //miCookieTest.Click += new EventHandler(miCookieTest_Click);
            //miDebug.MenuItems.Add(miCookieTest);

            //MenuItem miLogin = new MenuItem { Text = "Login" };
            //miLogin.Click += new EventHandler(miLogin_Click);
            //miDebug.MenuItems.Add(miLogin);

            MenuItem miCache = new MenuItem { Text = "Cache" };
            miCache.Click += new EventHandler(miCache_Click);
            miDebug.MenuItems.Add(miCache);

            MenuItem miPortrait = new MenuItem { Text = "Portrait" };
            miPortrait.Click += new EventHandler(miPortrait_Click);
            miDebug.MenuItems.Add(miPortrait);

            MenuItem miLandscape = new MenuItem { Text = "Landscape" };
            miLandscape.Click += new EventHandler(miLandscape_Click);
            miDebug.MenuItems.Add(miLandscape);
            
            //MenuItem miRegExTest = new MenuItem { Text = "RegEx" };
            //miRegExTest.Click += new EventHandler(miRegExTest_Click);
            //miDebug.MenuItems.Add(miRegExTest);

            miRightSoftKey.MenuItems.Add(miReload);
            if (Settings.DebugLog)
            {
                miRightSoftKey.MenuItems.Add(miDebug);
            }
            miRightSoftKey.MenuItems.Add(miSettings);
            miRightSoftKey.MenuItems.Add(miExit);

            miLeftSoftKey = new MenuItem { Text = Messages.Show };
            miLeftSoftKey.Click += new EventHandler(miShowPic_Click);

            Menu.MenuItems.Add(miLeftSoftKey);
            Menu.MenuItems.Add(miRightSoftKey);

            SetMenu(false);

            JobQueue.IsEmpty += new EventHandler(client_QueueIsEmpty);
            JobQueue.IsNotEmpty += new EventHandler(client_QueueIsNotEmpty);

            //LoginForm loginForm = new LoginForm();
            //loginForm.ShowDialog();

            viewWidth = postView.Size.Width;
            viewHeight = postView.Size.Height;

            photoForm = new PhotoForm();

            progressStatusBar.Visible = false;

            //loadingIndicator.Image = global::Tumblott.Properties.Resources.button_bg_loading;
            SetLoadingIndicator(false);
            //SetLoadingIndicator(true);

            this.Load += new EventHandler(MainForm_Load);

            statusPanel.Text = "0/0";

            TumblrPost post = new TumblrPost();
            post.Html = "<div>重そうで<br/>重くない<br/>少し重い<br/>Tumblrクライアント<br/><br/><b>ご注意</b><br/><br/>これはスナップショットバージョンであり，非常に不安定です。<br/>メニューから[Load dashboard]を選択すると，ダッシュボードを表示します。</div>";
            post.AvatarImage = global::Tumblott.Properties.Resources.tumblott_icon;
            // FIXME Infoの中身をPostViewに直接表示させたい
            //post.Info = "Welcome to Tumblott";
            postView.Text = "Tumblott\nVersion " + Utils.GetExecutingAssemblyVersion() + " (" + Utils.GetBuiltDateTime().ToString("yyyy/MM/dd") + ")";
            postView.Post = post;
        }

        void miPortrait_Click(object sender, EventArgs e)
        {
            Utils.DebugLog("force Portrait");
            this.ClientSize = new System.Drawing.Size(240, 268);
            Portrait();
        }

        void miLandscape_Click(object sender, EventArgs e)
        {
            Utils.DebugLog("force Landscape");
            this.ClientSize = new System.Drawing.Size(320, 188);
            Landscape();
        }

        void MainForm_Load(object sender, EventArgs e)
        {
            loadingIndicator.Start();

            if (Settings.Email == null)
            {
                SettingsForm settingsForm = new SettingsForm();
                settingsForm.ShowDialog();
            }
        }

        void client_QueueIsNotEmpty(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action<bool>(SetLoadingIndicator), true);
        }

        void client_QueueIsEmpty(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action<bool>(SetLoadingIndicator), false);
        }

        private void SetLoadingIndicator(bool isLoading)
        {
            if (isLoading)
            {
                loadingIndicator.Visible = true;
            }
            else
            {
                loadingIndicator.Visible = false;
            }
        }

        private void SetMenu(bool hasShowPicMenu)
        {
            miLeftSoftKey.Enabled = hasShowPicMenu;
        }

        void miReload_Click(object sender, EventArgs e)
        {
            this.posts.Clear();
            this.currentView = 0;
            FetchDashboard(null);
        }

        void miExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void miSettings_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private void miCookieTest_Click(object sender, EventArgs e)
        {
            string ch = Utils.ParseCookie("redirect_to=%2Fiphone; expires=Sun, 08-Nov-2009 12:07:47 GMT; path=/; httponly,pfu=201326; expires=Mon, 08-Nov-2010 11:52:47 GMT; path=/; httponly,pfp=ZdmQhtHHkm4JoDTtZyGyMW4dxjwM6OYakYF03vcz; expires=Mon, 08-Nov-2010 11:52:47 GMT; path=/; httponly,pfe=1289217167; expires=Mon, 08-Nov-2010 11:52:47 GMT; path=/; httponly");
            MessageBox.Show(ch);
        }

        private void miLogin_Click(object sender, EventArgs e)
        {
            LoginForm loginform = new LoginForm();
            loginform.Show();
        }

        private void miCache_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Cache.EntryCount.ToString() + " items, " + Cache.Size.ToString() + "bytes");
        }

#if false
        private void miRegExTest_Click(object sender, EventArgs e)
        {
            //正規表現パターンとオプションを指定してRegexオブジェクトを作成
            System.Text.RegularExpressions.Regex r =
                new System.Text.RegularExpressions.Regex(
                    @"<(h[1-6])\b[^>]*>(.*?)</\1>",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                    | System.Text.RegularExpressions.RegexOptions.Singleline);

            //TextBox1.Text内で正規表現と一致する対象をすべて検索
            System.Text.RegularExpressions.MatchCollection mc = r.Matches("test <h2>foobar</h2> test");

            foreach (System.Text.RegularExpressions.Match m in mc)
            {
                //正規表現に一致したグループと位置を表示
                MessageBox.Show("タグ:" + m.Groups[1].Value
                    + "\nタグ内の文字列:" + m.Groups[2].Value
                    + "\nタグの位置:" + m.Groups[1].Index);
            }
        }
#endif

        // TODO dashboardのreload
        private void FetchDashboard(Uri uri)
        {
            progressStatusBar.Visible = true;
            progressStatusBar.Text = Messages.LoadingDashboard;

            string dsbd_url;
            if (uri == null)
            {
                //dsbd_url = "http://www.tumblr.com/dashboard";
                //dsbd_url = "http://www.tumblr.com/show/photos";
                //dsbd_url = "http://www.tumblr.com/show/quotes";
                //dsbd_url = "http://www.tumblr.com/show/audio";
                dsbd_url = "http://www.tumblr.com/api/dashboard";
            }
            else
            {
                dsbd_url = uri.ToString();
            }
            
            // FIXME 現在既に読み込まれているPostsに対して画像取得等の処理が
            // 走っている場合の処理が必要(止めるとか)
            int start = 0;
            if (this.posts.Count != 0)
            {
                start = this.posts.Count - 1;
            }

            posts.FetchDashboard(start,
                per => { this.BeginInvoke(new Action<int>(UpdateProgressBar), per); },
                ps => { this.BeginInvoke(new Action<TumblrPosts>(FetchDashboardCompleted), ps); }
            );
        }

        private void UpdateProgressBar(int per)
        {
            progressStatusBar.Visible = true;
            progressStatusBar.Value = per;
            if (per == 100)
            {
                progressStatusBar.Text = "wait ...";
            }
        }

        private void FetchDashboardCompleted(TumblrPosts posts)
        {
            if (posts == null)
            {
                // 受信失敗？
                progressStatusBar.Value = 0;
                progressStatusBar.Visible = false;
                return;
            }

            foreach (TumblrPost p in posts)
            {
                if (true)
                {
                    // TODO 設定可能にする
                    p.GetImage(TumblrPost.ImageType.Avatar, TumblrPost.Priority.Normal, null,
                        result =>
                        {
                            Utils.DebugLog("GetImageCompleted");
                            this.BeginInvoke(new Action<JobResult>(GetImageCompleted), result);
                        });
                }
                if (p.Type == TumblrPost.Types.Photo && p.Image == null && p.ImageUri != null)
                {
                    p.GetImage(TumblrPost.ImageType.Normal, TumblrPost.Priority.Normal, null,
                        result => {
                            Utils.DebugLog("GetImageCompleted");
                            this.BeginInvoke(new Action<JobResult>(GetImageCompleted), result);
                        });
                }
            }
            this.BeginInvoke(new Action<TumblrPosts>(UpdatePosts), posts);

            progressStatusBar.Value = 0;
            progressStatusBar.Visible = false;
        }

        // UI thread
        private void GetImageCompleted(JobResult result)
        {
            TumblrPost post = (TumblrPost)(result.Object);
            if (posts[currentView].ImageUri == post.ImageUri)
            {
                UpdateView();
            }
        }

        private void UpdateStatus()
        {
            statusPanel.Text = String.Format("{0}/{1}", currentView+1, posts.Count);
        }

        private void UpdatePosts(TumblrPosts ps)
        {
            UpdateStatus();
            UpdateView();
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            if (currentView > 0)
            {
                currentView--;
                postView.StartFlip(PostView.FlipDirection.Right); // Right or Down
                postView.ResetPosition();
                UpdateStatus();
                UpdateView();
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (currentView < posts.Count - 1)
            {
                currentView++;
                postView.StartFlip(PostView.FlipDirection.Left); // Left or Up
                postView.ResetPosition();
                UpdateStatus();
                UpdateView();

                if (currentView == posts.Count - 1)
                {
                    // FIXME 当該ページ読み込み中に再度読みに行かないように(Client側で対応？)
                    FetchDashboard(posts.NextPageUri);
                }
            }
        }

        private void likeButton_Click(object sender, EventArgs e)
        {
            Utils.DebugLog("Like button clicked");
            if (currentView < posts.Count)
            {
                posts[currentView].Like(true, null, result =>
                {
                    Utils.DebugLog("LikeCompleted");
                    this.BeginInvoke(new Action<JobResult>(LikeCompleted), result);
                });
            }
        }

        private void LikeCompleted(JobResult result)
        {
            TumblrPost post = (TumblrPost)(result.Object);
        }

        private void reblogButton_Click(object sender, EventArgs e)
        {
            Utils.DebugLog("Reblog button clicked");
            if (currentView < posts.Count)
            {
                posts[currentView].Reblog(null, result =>
                {
                    Utils.DebugLog("ReblogCompleted");
                    this.BeginInvoke(new Action<JobResult>(ReblogCompleted), result);
                });
            }
        }

        private void ReblogCompleted(JobResult result)
        {
            TumblrPost post = (TumblrPost)(result.Object);
        }

        /// <summary>
        /// PostViewへ新しいPostをセット
        /// </summary>
        private void UpdateView()
        {
            postView.Post = posts[currentView];
            postView.Text = posts[currentView].Info;
            UpdateStatus();
            if (posts[currentView].Type == TumblrPost.Types.Photo)
            {
                SetMenu(true);
            }
            else
            {
                SetMenu(false);
            }
        }

        private void postView_ImageClicked(object sender, EventArgs e)
        {
            ShowPhotoForm();
        }

        private void miShowPic_Click(object sender, EventArgs e)
        {
            ShowPhotoForm();
        }

        private void ShowPhotoForm()
        {
            if (posts.Count == 0) { return; }

            if (posts[currentView].Type != TumblrPost.Types.Photo)
            {
                return;
            }

            photoForm.SetPost(posts[currentView]);
            photoForm.ShowDialog();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // FIXME スクロール実装
            Utils.DebugLog("keydown: " + e.KeyCode);
            switch (e.KeyCode)
            {
                case Keys.J:
                case Keys.Right:
                    nextButton_Click(sender, e);
                    break;
                case Keys.K:
                case Keys.Left:
                    prevButton_Click(sender, e);
                    break;
                case Keys.L:
                    likeButton_Click(sender, e);
                    break;
                case Keys.T:
                    reblogButton_Click(sender, e);
                    break;
                case Keys.Return:
                    miShowPic_Click(sender, e);
                    break;
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            //MessageBox.Show("resize");

            //if (Screen.PrimaryScreen.Bounds.Width > Screen.PrimaryScreen.Bounds.Height)
            if (this.Width >= this.Height)
            {
                Utils.DebugLog("re-layout Landscape");

                // タスクバーを消す
                //this.WindowState = FormWindowState.Maximized;
                //this.ControlBox = false;
                // FIXME ControlBoxを切り替えるとPortraitに戻らなくなる？(2010/05/02)
                // Resizeイベントが起こらなくなるみたい
                // → WindowsStateのMaximizedとTopMostだけにしてみた
                //    設定フォーム表示時やタスク切り替え時なんかは
                //    TopMostをfalseにしてやらないとない
                //    Activated/Deactivateイベントをハンドル？

                //this.WindowState = FormWindowState.Maximized;
                //this.TopMost = true;

                Landscape();
                isLandscape = true;
            }
            else
            {
                Utils.DebugLog("re-layout Portrait");

                // タスクバーを表示する
                //this.WindowState = FormWindowState.Normal;
                //this.TopMost = false;

                Portrait();
                isLandscape = false;
            }
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            /*
            if (isLandscape)
            {
                this.WindowState = FormWindowState.Maximized;
                this.TopMost = true;
            }
            */
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            /*
            if (isLandscape)
            {
                this.WindowState = FormWindowState.Normal;
                this.TopMost = false;
            }
            */
        }
    }
}
