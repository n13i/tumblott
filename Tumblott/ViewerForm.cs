using System;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tumblott.Client.Tumblr;
using Tumblott.Forms;

namespace Tumblott
{
    public partial class ViewerForm : TumblottForm
    {
        private TumblrPosts posts = new TumblrPosts();

        private int currentView = 0;
        private int viewWidth = 240;
        private int viewHeight = 240;

        private PhotoForm photoForm;

        private MenuItem miLeftSoftKey;
        private MenuItem miRightSoftKey;
        private MenuItem miReload;

        private bool isLandscape = false;

        private bool isKeyDown = false;

        private SizeF scaleFactor;

        public enum Mode { Dashboard, Tumblelog };

        public ViewerForm()
        {
            InitializeComponent();
            this.Closing += new CancelEventHandler(ViewerForm_Closing);
            this.DialogResult = DialogResult.None;
            this.NextForm = "welcome";

            // メニューの作成
            this.Menu = new MainMenu();
            this.ContextMenu = new ContextMenu();

            miRightSoftKey = new MenuItem { Text = Messages.Menu };

            miReload = new MenuItem { Text = Messages.Reload };
            miReload.Click += new EventHandler(miReload_Click);

            MenuItem miSettings = new MenuItem { Text = Messages.Settings };
            miSettings.Click += new EventHandler(miSettings_Click);

            MenuItem miExit = new MenuItem { Text = Messages.Exit };
            miExit.Click += new EventHandler(miExit_Click);

            // debug menu
            MenuItem miDebug = new MenuItem { Text = "Debug" };

            MenuItem miCache = new MenuItem { Text = "Cache" };
            miCache.Click += new EventHandler(miCache_Click);
            miDebug.MenuItems.Add(miCache);

            MenuItem miPortrait = new MenuItem { Text = "Portrait" };
            miPortrait.Click += new EventHandler(miPortrait_Click);
            miDebug.MenuItems.Add(miPortrait);

            MenuItem miLandscape = new MenuItem { Text = "Landscape" };
            miLandscape.Click += new EventHandler(miLandscape_Click);
            miDebug.MenuItems.Add(miLandscape);

            MenuItem miDialogTest = new MenuItem { Text = "DialogBox" };
            miDialogTest.Click += new EventHandler(miDialogTest_Click);
            miDebug.MenuItems.Add(miDialogTest);

            MenuItem miResizeTest = new MenuItem { Text = "Resizable" };
            miResizeTest.Click += new EventHandler(miResizeTest_Click);
            miDebug.MenuItems.Add(miResizeTest);

            //MenuItem miRegExTest = new MenuItem { Text = "RegEx" };
            //miRegExTest.Click += new EventHandler(miRegExTest_Click);
            //miDebug.MenuItems.Add(miRegExTest);

            Menu.MenuItemCollection miTarget;
            if (Settings.ShowMenuBar)
            {
                miTarget = miRightSoftKey.MenuItems;
            }
            else
            {
                miTarget = this.ContextMenu.MenuItems;
            }

            miTarget.Add(miReload);
            if (Settings.DebugLog)
            {
                miTarget.Add(miDebug);
            }
            miTarget.Add(new MenuItem { Text = "-" });
            miTarget.Add(miSettings);
            miTarget.Add(new MenuItem { Text = "-" });
            miTarget.Add(miExit);

            miLeftSoftKey = new MenuItem { Text = Messages.Show };
            miLeftSoftKey.Click += new EventHandler(miShowPic_Click);

            if (Settings.ShowMenuBar)
            {
                this.Menu.MenuItems.Add(miLeftSoftKey);
                this.Menu.MenuItems.Add(miRightSoftKey);
                this.statusPanel.ShowMenuButton = false;
            }
            else
            {
                this.Menu = null;
                this.statusPanel.Click += new EventHandler(statusPanel_Click);
                this.loadingIndicator.Click += new EventHandler(statusPanel_Click);
                this.ContextMenu.Popup += new EventHandler(ContextMenu_Popup);
//                this.ContextMenu.Show(this, 
                this.statusPanel.ShowMenuButton = true;
            }

            SetMenu(false);

            JobQueue.IsEmpty += new EventHandler(client_QueueIsEmpty);
            JobQueue.IsNotEmpty += new EventHandler(client_QueueIsNotEmpty);

            //LoginForm loginForm = new LoginForm();
            //loginForm.ShowDialog();

            viewWidth = postView.Size.Width;
            viewHeight = postView.Size.Height;

            photoForm = new PhotoForm();

            progressStatusBar.SwitchVisibility(false);

            //loadingIndicator.Image = global::Tumblott.Properties.Resources.button_bg_loading;
            SetLoadingIndicator(false);
            //SetLoadingIndicator(true);

            this.Load += new EventHandler(MainForm_Load);

            statusPanel.Text = "0/0";

            //TumblrPost post = new TumblrPost();
            //post.Html = "<div><b>ご注意</b><br/><br/>これはスナップショットバージョンであり，非常に不安定です。<br/>メニューから[Load dashboard]を選択すると，ダッシュボードを表示します。</div>";
            //post.AvatarImage = global::Tumblott.Properties.Resources.tumblott_icon_64;
            // FIXME Infoの中身をPostViewに直接表示させたい
            //post.Info = "Welcome to Tumblott";
            //postView.Title = "Tumblott";
            //postView.Text = "Version " + Utils.GetExecutingAssemblyVersion() + " (" + Utils.GetBuiltDateTime().ToString("yyyy/MM/dd") + ")";
            //postView.Post = post;
        }

        void ViewerForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DialogResult == DialogResult.None)
            {
                this.DialogResult = DialogResult.Abort;
            }
        }

        void statusPanel_Click(object sender, EventArgs e)
        {
            this.ContextMenu.Show((Control)sender, new Point(0, 0));
        }

        void ContextMenu_Popup(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        void miDialogTest_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            DialogBox dlg = new DialogBox();
            dlg.Location = new Point((Screen.PrimaryScreen.Bounds.Width - dlg.Width) / 2, (Screen.PrimaryScreen.Bounds.Height - dlg.Height) / 2);
            dlg.ShowDialog();
        }

        void miResizeTest_Click(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
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
            this.loadingIndicator.Start();
            this.postView.StartTransit();

            if (Settings.Email == null)
            {
                SettingsForm settingsForm = new SettingsForm();
                settingsForm.ShowDialog();
            }

            if (Settings.Email != null)
            {
                this.posts.Clear();
                this.currentView = 0;
                FetchDashboard();
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
                this.miReload.Enabled = false;
            }
            else
            {
                loadingIndicator.Visible = false;
                this.miReload.Enabled = true;
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
            FetchDashboard();
        }

        void miExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            this.Close();
        }

        void miSettings_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private void miCache_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Cache.EntryCount.ToString() + " items, " + Cache.Size.ToString() + "bytes");
        }

        // TODO dashboardのreload
        private void FetchDashboard()
        {
            progressStatusBar.IsProgressMode = true;
            progressStatusBar.Text = Messages.Loading;

            // FIXME 現在既に読み込まれているPostsに対して画像取得等の処理が
            // 走っている場合の処理が必要(止めるとか)

            this.posts.FetchDashboard(
                per => { this.BeginInvoke(new Action<int>(UpdateProgressBar), per); },
                ps => { this.BeginInvoke(new Action<TumblrResult>(FetchDashboardCompleted), ps); }
            );
        }

        private void UpdateProgressBar(int per)
        {
            progressStatusBar.IsProgressMode = true;
            progressStatusBar.Value = per;
            if (per == 100)
            {
                progressStatusBar.Text = Messages.Processing;
            }
        }

        private void FetchDashboardCompleted(TumblrResult r)
        {
            if (r.IsError)
            {
                // 受信失敗
                progressStatusBar.Value = 0;
                progressStatusBar.IsProgressMode = false;
                progressStatusBar.Text = "";

                Utils.DebugLog("FetchDashboardCompleted: " + r.Text);

                DialogResult dr = MessageBox.Show(Messages.ConfirmRetryLoading, Messages.LoadError, MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                if (dr == DialogResult.Retry)
                {
                    FetchDashboard();
                }
                return;
            }
            else
            {
                // 受信成功

                // dashboard流量増大時(r.PostsCount == 0)の対応
                if (r.PostsCount == 0)
                {
                    // 次を読み込んでみる
                    // FIXME リトライ回数制限が必要
                    Utils.DebugLog("trying to load next page of dashboard ...");
                    // FIXME 10秒待つべき
                    FetchDashboard();
                }
            }

            foreach (TumblrPost p in this.posts)
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
            //this.BeginInvoke(new Action<TumblrPosts>(UpdatePosts), this.posts);
            UpdatePosts(this.posts);

            progressStatusBar.Value = 0;
            progressStatusBar.Text = "";
            progressStatusBar.IsProgressMode = false;
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
            // FIXME ResetPositionはリロード時のみにすべき
            //postView.ResetPosition();
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
                //postView.StartTransit();
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
                //postView.StartTransit();

                if (currentView == posts.Count - 1)
                {
                    // FIXME 当該ページ読み込み中に再度読みに行かないように(Client側で対応？)
                    FetchDashboard();
                }
            }
        }

        private void likeButton_Click(object sender, EventArgs e)
        {
            Utils.DebugLog("Like button clicked");
            if (currentView < posts.Count)
            {
                // Likeをトグルする
                bool isLiked = posts[currentView].IsLiked;
                Guid id = posts[currentView].Like(!isLiked, null, result =>
                {
                    Utils.DebugLog("LikeCompleted");
                    this.BeginInvoke(new Action<JobResult>(LikeCompleted), result);
                });
                this.progressStatusBar.AddNotify(id, posts[currentView].Image, (isLiked ? "Unlike..." : "Like..."), 0);
            }
        }

        private void LikeCompleted(JobResult result)
        {
            TumblrPost post = (TumblrPost)(result.Object);
            this.progressStatusBar.RemoveNotify(result.Guid);
        }

        private void reblogButton_Click(object sender, EventArgs e)
        {
            Utils.DebugLog("Reblog button clicked");
            if (currentView < posts.Count)
            {
                Guid id = posts[currentView].Reblog(null, result =>
                {
                    Utils.DebugLog("ReblogCompleted");
                    this.BeginInvoke(new Action<JobResult>(ReblogCompleted), result);
                });
                Utils.DebugLog("reblog job: guid=" + id.ToString());
                this.progressStatusBar.AddNotify(id, posts[currentView].Image, "Reblog...", 0);
            }
        }

        private void ReblogCompleted(JobResult result)
        {
            TumblrPost post = (TumblrPost)(result.Object);
            Utils.DebugLog("reblog job done: guid=" + result.Guid.ToString());
            this.progressStatusBar.RemoveNotify(result.Guid);
            //this.progressStatusBar.ChangeNotify(result.Guid, "Done", 1);
        }

        /// <summary>
        /// PostViewへ新しいPostをセット
        /// </summary>
        private void UpdateView()
        {
            postView.Post = posts[currentView];
            postView.Title = posts[currentView].Tumblelog;

            string notes = null, from = null;

            
            if(posts[currentView].NoteCount > 0)
            {
                notes = String.Format("{0:#,0} note{1}", this.posts[currentView].NoteCount, (this.posts[currentView].NoteCount > 1 ? "s" : ""));
            }
            if (posts[currentView].RebloggedFrom != null)
            {
                from = String.Format("reblogged {0}", this.posts[currentView].RebloggedFrom);
            }

            if (notes != null)
            {
                if (from != null)
                {
                    postView.Text = String.Format("{0} / {1}", from, notes);
                }
                else
                {
                    postView.Text = notes;
                }
            }
            else
            {
                if (from != null)
                {
                    postView.Text = from;
                }
                else
                {
                    postView.Text = null;
                }
            }

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

        private void ViewerForm_KeyDown(object sender, KeyEventArgs e)
        {
            // FIXME スクロール実装
            Utils.DebugLog("keydown: " + e.KeyCode);
            if (this.isKeyDown)
            {
                return;
            }
            switch (e.KeyCode)
            {
                case Keys.J:
                    nextButton_Click(sender, e);
                    break;
                case Keys.K:
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
                default:
                    break;
            }
            // FIXME 本来はキー個別に見るべき
            this.isKeyDown = true;
        }

        private void ViewerForm_KeyUp(object sender, KeyEventArgs e)
        {
            // FIXME カーソルキーはKeyDownが取れない？
            Utils.DebugLog("keyup: " + e.KeyCode);
            this.isKeyDown = false;
            switch (e.KeyCode)
            {
                case Keys.Right:
                    nextButton_Click(sender, e);
                    break;
                case Keys.Left:
                    prevButton_Click(sender, e);
                    break;
                case Keys.Down:
                    this.postView.Scroll(new Point(0, (int)(-50 * this.scaleFactor.Height)));
                    break;
                case Keys.Up:
                    this.postView.Scroll(new Point(0, (int)(+50 * this.scaleFactor.Height)));
                    break;
                default:
                    break;
            }
        }

        private void ViewerForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.DebugLog("keypress: " + e.KeyChar.ToString());
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

        private void postView_FlipRequested(object sender, MouseEventArgs e)
        {
            switch (e.X)
            {
                case -1:
                    prevButton_Click(sender, e);
                    break;
                case 1:
                    nextButton_Click(sender, e);
                    break;
                default:
                    break;
            }
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            this.scaleFactor = factor;
            base.ScaleControl(factor, specified);
        }
    }
}
