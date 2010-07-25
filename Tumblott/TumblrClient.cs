using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Tumblott.Client.Tumblr
{
    /*
     * Tumblr client class
     * static
     */
    public sealed class TumblrClient
    {
        private bool isLoggedIn = false;
        private string cookie;

        private static readonly TumblrClient instance = new TumblrClient();

        public static TumblrClient Instance
        {
            get { return instance; }
        }

        private TumblrClient()
        {
        }

        /// <summary>
        /// ログイン
        /// APIを使用する場合，不要。
        /// </summary>
        /// <returns></returns>
#if false
        private bool Login()
        {
            if (isLoggedIn) return true;

            // FIXME API使用の場合は不要
            isLoggedIn = true;
            return true;

            Utils.DebugLog("Login");

            string ret;

            string email = Settings.Email;
            string password = Settings.Password;

            string data = "email=" + Utils.UrlEncode(email, Encoding.UTF8) + "&password=" + Utils.UrlEncode(password, Encoding.UTF8) + "&redirect_to=" + Utils.UrlEncode("/iphone", Encoding.UTF8);

            // Cookie があれば取得
            this.cookie = null;
            try
            {
                using (var sr = new StreamReader(Path.Combine(Settings.ExePath, "cookie.txt")))
                {
                    this.cookie = sr.ReadLine();
                }
            }
            catch
            {
            }

            //MessageBox.Show(this.cookie);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.tumblr.com/login");
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; ja; rv:1.9.2) Gecko/20100115 Firefox/3.6 (.NET CLR 3.5.30729)";
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded"; // required
            request.ContentLength = data.Length; // 指定しないと GetRequestStream で InvalidOperationException
            if (this.cookie != null)
            {
                request.Headers.Add("Cookie", this.cookie);
            }
            request.Timeout = Settings.HttpTimeout;
            IWebProxy proxy = Settings.GetProxy();
            if (proxy != null)
            {
                request.Proxy = proxy;
            }
            // cf. http://msdn.microsoft.com/ja-jp/library/system.net.httpwebrequest.allowwritestreambuffering%28VS.80%29.aspx
            request.AllowWriteStreamBuffering = true;

            Utils.DebugLog("post data: " + data);
            Stream reqStream = request.GetRequestStream();
            StreamWriter writer = new StreamWriter(reqStream);
            writer.Write(data);
            writer.Close();
            reqStream.Close();

            HttpWebResponse response = null;
            Stream responseStream = null;
            StreamReader reader = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();

                string setCookie = Utils.ParseCookie(response.GetResponseHeader("Set-Cookie"));
                if (setCookie != null && setCookie != "")
                {
                    // ログイン時に有効な Cookie を渡していた場合は Set-Cookie は返らない
                    this.cookie = setCookie;
                }

                responseStream = response.GetResponseStream();
                reader = new StreamReader(responseStream);
                ret = reader.ReadToEnd();

                Utils.DebugLog(ret);

                if (ret.IndexOf("<form action=\"/login\"") == -1)
                {
                    Utils.DebugLog("Login succeeded");
                    isLoggedIn = true;
                }
                else
                {
                    Utils.DebugLog("Login failed");
                    MessageBox.Show("login failed");
                    isLoggedIn = false;
                }
            }
            catch (WebException e)
            {
                Utils.DebugLog(e);
                MessageBox.Show(e.ToString());
                if (response != null)
                {
                    if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        // FIXME エラー処理必要
                    }
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (responseStream != null)
                {
                    responseStream.Close();
                }
            }

            // Cookie の保存
            try
            {
                using (var sw = new StreamWriter(Path.Combine(Settings.ExePath, "cookie.txt")))
                {
                    sw.WriteLine(this.cookie);
                }
            }
            catch
            {
            }

            return isLoggedIn;
        }
#endif

        public static HttpWebResponse Fetch(Uri uri)
        {
            return Request(uri, "GET", null);
        }

        public static HttpWebResponse Request(Uri uri, string method, string postData)
        {
            /*
            if (!instance.isLoggedIn)
            {
                if (!instance.Login())
                {
                    return null;
                }
            }
            */

            if (uri.ToString().Substring(0, 7) != "http://")
            {
                return null;
            }

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            //request.Headers.Add("Cookie", instance.cookie);
            request.Timeout = Settings.HttpTimeout;
            IWebProxy proxy = Settings.GetProxy();
            if (proxy != null)
            {
                request.Proxy = proxy;
            }

            if (method == "POST")
            {
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded"; // required
                request.ContentLength = postData.Length; // 指定しないと GetRequestStream で InvalidOperationException

                if (postData != null)
                {
                    // FIXME タイムアウト等でのWebExceptionをキャッチ
                    try
                    {
                        Stream reqStream = request.GetRequestStream();
                        StreamWriter writer = new StreamWriter(reqStream);
                        writer.Write(postData);
                        writer.Close();
                        reqStream.Close();
                    }
                    catch (Exception e)
                    {
                        Utils.DebugLog(e);
                        return null;
                    }
                }
            }

            // FIXME 403 とかのときの例外をキャッチ
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return response;
            }
            catch (Exception e)
            {
                Utils.DebugLog(e);
                return null;
            }
        }

        public static HttpWebRequest GetRequest(Uri uri)
        {
            /*
            if (!instance.isLoggedIn)
            {
                if (!instance.Login())
                {
                    return null;
                }
            }
            */

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            //request.Headers.Add("Cookie", instance.cookie);
            request.Timeout = Settings.HttpTimeout;
            IWebProxy proxy = Settings.GetProxy();
            if (proxy != null)
            {
                request.Proxy = proxy;
            }
            return request;
        }

        /// <summary>
        /// APIを使用して指定したポストのlike/unlikeを行う
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="reblogKey"></param>
        /// <param name="isLike"></param>
        /// <returns>成功した場合true，失敗の場合false</returns>
        public static bool Like(string postId, string reblogKey, bool isLike)
        {
            PostDataBuilder pdb = new PostDataBuilder();
            pdb.Add("email", Settings.Email);
            pdb.Add("password", Settings.Password);
            pdb.Add("post-id", postId);
            pdb.Add("reblog-key", reblogKey);

            Uri uri = null;
            if(isLike)
            {
                uri = new Uri("http://www.tumblr.com/api/like");
            }
            else
            {
                uri = new Uri("http://www.tumblr.com/api/unlike");
            }

            HttpWebResponse r = Request(uri, "POST", pdb.ToString());
            if(r == null) { return false; }
            if (r.StatusCode != HttpStatusCode.OK)
            {
                Utils.DebugLog("like: code = " + r.StatusCode.ToString());
                return false;
            }

            Stream stream = r.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            Utils.DebugLog(sr.ReadToEnd());
            sr.Close();
            stream.Close();
            r.Close();

            return true;
        }

        /// <summary>
        /// APIを使用して指定したポストのreblogを行う
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="reblogKey"></param>
        /// <param name="comment"></param>
        /// <returns>成功した場合true，失敗の場合false</returns>
        public static bool Reblog(string postId, string reblogKey, string comment)
        {
            PostDataBuilder pdb = new PostDataBuilder();
            pdb.Add("email", Settings.Email);
            pdb.Add("password", Settings.Password);
            pdb.Add("post-id", postId);
            pdb.Add("reblog-key", reblogKey);

            if (comment != null)
            {
                pdb.Add("comment", comment);
            }

            HttpWebResponse r = Request(new Uri("http://www.tumblr.com/api/reblog"), "POST", pdb.ToString());
            if (r == null) { return false; }
            if (r.StatusCode != HttpStatusCode.Created)
            {
                Utils.DebugLog("reblog: code = " + r.StatusCode.ToString());
                return false;
            }

            Stream stream = r.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            Utils.DebugLog(sr.ReadToEnd());
            sr.Close();
            stream.Close();
            r.Close();

            return true;
        }
    }

    #region ジョブ管理

    public class JobResult
    {
        public object Object { get; set; }
        public bool IsError { get; set; }
        public int Status { get; set; }
    }

    public delegate JobResult JobCallback(WaitCallback progressChanged, object o);

    public class JobItem
    {
        public JobCallback Job { get; set; }
        public WaitCallback JobProgressChanged { get; set; }
        public WaitCallback JobCompleted { get; set; }
        public object StateObject { get; set; }
    }

    public sealed class JobQueue
    {
        public enum Priority { Urgent = 0, High, Normal, Low, Count };
        private Queue<JobItem>[] queue = new Queue<JobItem>[(int)(Priority.Count)];

        private static readonly JobQueue instance = new JobQueue();

        public static event EventHandler IsEmpty;
        public static event EventHandler IsNotEmpty;

        private JobQueue()
        {
            Utils.DebugLog("construct");
            for (var i = 0; i < (int)Priority.Count; i++)
            {
                this.queue[i] = new Queue<JobItem>();
            }

            Thread t = new Thread(new ThreadStart(Run));
            t.IsBackground = true;
            t.Start();
        }

        public static JobQueue Instance
        {
            get
            {
                return instance;
            }
        }

        public static void Enqueue(Priority pri, JobCallback job, WaitCallback progressChanged, WaitCallback completed, object o)
        {
            Utils.DebugLog("Add job to queue " + pri.ToString());
            instance.queue[(int)pri].Enqueue(new JobItem { Job = job, JobProgressChanged = progressChanged, JobCompleted = completed, StateObject = o });
        }

        private static void Run()
        {
            bool isEmpty = true;
            JobItem item;
            
            while (true)
            {
                item = null;

                foreach (var q in instance.queue)
                {
                    if (q.Count > 0)
                    {
                        item = q.Dequeue();
                        break;
                    }
                }

                if(item != null)
                {
                    if (isEmpty)
                    {
                        EventHandler evh = JobQueue.IsNotEmpty;
                        if (evh != null)
                        {
                            evh(instance, null);
                        }
                        isEmpty = false;
                    }

                    object result = null;

                    // do job
                    JobCallback job = item.Job;
                    if (job != null)
                    {
                        //Utils.DebugLog("job running");
                        result = job(item.JobProgressChanged, item.StateObject);

                        // for debug
                        //Thread.Sleep(3000);
                        //Utils.DebugLog("job completed");
                    }

                    WaitCallback comp = item.JobCompleted;
                    if (comp != null)
                    {
                        comp(result);
                    }
                }
                else
                {
                    // empty

                    if (!isEmpty)
                    {
                        EventHandler evh = JobQueue.IsEmpty;
                        if (evh != null)
                        {
                            evh(instance, null);
                        }
                        isEmpty = true;
                    }

                    Thread.Sleep(100);
                }
            }
        }
  
    }

    #endregion


    public class TumblrResult
    {
        public string Text { get; set; }
    }

    /// <summary>
    /// 投稿リスト
    /// </summary>
    public class TumblrPosts : List<TumblrPost>
    {
        public Uri NextPageUri;

        private object lockObject = new object();

        private bool isRunning = false;
        private bool requestRunning = false;
        private bool cancelFlag = false;

        private HttpWebRequest request;
        private HttpWebResponse response;
        private Stream networkStream;
        private byte[] buffer = new byte[2048];

        private long totalReadLength;
        private long totalLength;

        private IAsyncResult asyncResult = null;

        private List<byte> strByte = new List<byte>();

        private Action<int> progressChangedDelg = null;
        private Action<TumblrResult> completedDelg = null;

        /// <summary>
        /// ダッシュボードを受信
        /// </summary>
        /// <param name="url"></param>
        /// <param name="progressChanged"></param>
        /// <param name="completed"></param>
        public void FetchDashboard(int start, Action<int> progressChanged, Action<TumblrResult> completed)
        {
            lock (lockObject)
            {
                if (isRunning)
                {
                    Utils.DebugLog("TumblrClient: FetchDashboard is running");
                    return;
                }
                isRunning = true;
                requestRunning = true;

                this.progressChangedDelg = progressChanged;
                this.completedDelg = completed;

                ThreadPool.QueueUserWorkItem(FetchDashboardAsync, start);
            }
        }

        private void FetchDashboardAsync(object o)
        {
            // FIXME start, num, type を指定可能にする

            int start = (int)o;

            request = TumblrClient.GetRequest(new Uri("http://www.tumblr.com/api/dashboard"));
            if (request == null)
            {
                // FIXME 例外をリスローすべきか
                return;
            }

            PostDataBuilder pdb = new PostDataBuilder();
            pdb.Add("email", Settings.Email);
            pdb.Add("password", Settings.Password);
            pdb.Add("start", start.ToString());
            pdb.Add("num", "20");
            pdb.Add("likes", "1");
            //pdb.Add("type", "link");

            string postData = pdb.ToString();

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded"; // required
            request.ContentLength = postData.Length; // 指定しないと GetRequestStream で InvalidOperationException

            try
            {
                if (postData != null)
                {
                    Stream reqStream = request.GetRequestStream();
                    StreamWriter writer = new StreamWriter(reqStream);
                    writer.Write(postData);
                    writer.Close();
                    reqStream.Close();
                }

                asyncResult = request.BeginGetResponse(new AsyncCallback(ResponseCallback), null);
            }
            catch (Exception e)
            {
                //MessageBox.Show("受信中の例外: " + e.ToString());
                CleanupVariables();

                TumblrResult r = new TumblrResult();

                if (e is WebException)
                {
                    Utils.DebugLog((((HttpWebResponse)((WebException)e).Response).StatusCode));
                    r.Text = (((HttpWebResponse)((WebException)e).Response).StatusCode).ToString();
                }

                this.completedDelg(r);
            }
        }

        private void ResponseCallback(IAsyncResult ar)
        {
            lock (lockObject)
            {
                try
                {
                    response = (HttpWebResponse)request.EndGetResponse(ar);
                    requestRunning = false;

                    if (cancelFlag)
                    {
                        CleanupVariables();
                        return;
                    }

                    totalLength = response.ContentLength;

                    networkStream = response.GetResponseStream();

                    totalReadLength = 0;

                    asyncResult = networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallback), null);
                }
                catch (WebException e)
                {
                    if (cancelFlag)
                    {
                        // cancel
                    }
                    else
                    {
                        // error
                        throw e;
                    }

                    CleanupVariables();
                }
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            lock (lockObject)
            {
                try
                {
                    int readLength = networkStream.EndRead(ar);
                    totalReadLength += readLength;

                    if (cancelFlag)
                    {
                        // cancel
                        CleanupVariables();
                        return;
                    }

                    if (readLength > 0)
                    {
                        // UTF-8 への変換はあとで行うべき
                        // (buffer 境界に UTF-8 マルチバイト文字が挟まるとまずい)
                        byte[] tmp = new byte[readLength];
                        Buffer.BlockCopy(buffer, 0, tmp, 0, readLength * sizeof(byte));
                        strByte.AddRange(tmp);

                        if (totalLength > 0)
                        {
                            int per = (int)(totalReadLength * 100 / totalLength);
                            progressChangedDelg(per);
                        }
                        else
                        {
                            //
                        }

                        // next data
                        asyncResult = networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallback), null);
                    }
                    else
                    {
                        // read done

                        string tmp = Encoding.UTF8.GetString(strByte.ToArray(), 0, strByte.Count);
                        progressChangedDelg(100);

                        // TODO スクレイピングかAPI使用か切り替えられるといい
                        //Scrape(tmp);
                        ParseXML(tmp);

                        this.completedDelg(null);
                        CleanupVariables();
                    }
                }
                catch (Exception e)
                {
                    // error
                    CleanupVariables();

                    Utils.DebugLog(e.ToString());

                    TumblrResult r = new TumblrResult();

                    if (e is WebException)
                    {
                        Utils.DebugLog((((HttpWebResponse)((WebException)e).Response).StatusCode));
                        r.Text = (((HttpWebResponse)((WebException)e).Response).StatusCode).ToString();
                    }

                    this.completedDelg(r);
                }
            }
        }

        private void CleanupVariables()
        {
            if (networkStream != null)
            {
                networkStream.Close();
                networkStream = null;
            }

            asyncResult = null;
            request = null;
            response = null;
            requestRunning = false;
            cancelFlag = false;
            isRunning = false;

            strByte.Clear();
        }

        /// <summary>
        /// dashboard API取得結果の取り込み
        /// </summary>
        /// <param name="xml"></param>
        private void ParseXML(string xml)
        {
            StringReader sr = new StringReader(xml);
            XmlTextReader reader = new XmlTextReader(sr);

            TumblrPost post = null;

            //Stack<string> pathStack = new Stack<string>();
            string currentElement = null;
            bool isThumbnailImageNode = false;
            bool isLargeImageNode = false;

            // 参考:
            // XmlTextReaderを使用してXMLを読み込む (C#)
            // <http://capsulecorp.studio-web.net/tora/cs/XmlTextReader.html>
            // FIXME Rate limit exceededの場合，ここでWebExceptionが発生(503)
            while (reader.Read())
            {
                //string[] pathArray = pathStack.ToArray();
                //Array.Reverse(pathArray);
                //string path = String.Join("/", pathArray);
                //Utils.DebugLog(path);

                //Utils.DebugLog(reader.NodeType.ToString());
                switch(reader.NodeType)
                {
                    case XmlNodeType.Element:
                        currentElement = reader.Name;
                        Utils.DebugLog("<" + reader.Name + ">");
                        switch (reader.Name)
                        {
                            case "post":
                                post = new TumblrPost();
                                ParsePostAttributes(reader, post);
                                break;
                            case "tumblelog":
                                ParseTumblelogAttributes(reader, post);
                                break;
                            case "photo-url":
                                if (reader.MoveToFirstAttribute())
                                {
                                    do
                                    {
                                        Utils.DebugLog("> " + reader.Name + "=" + reader.Value);
                                        // FIXME 全サイズのURLを持っておくべきかも
                                        if (reader.Name == "max-width" && reader.Value == ((int)Settings.ThumbnailImageSize).ToString())
                                        {
                                            isThumbnailImageNode = true;
                                        }
                                        if (reader.Name == "max-width" && reader.Value == "500")
                                        {
                                            isLargeImageNode = true;
                                        }
                                    }
                                    while (reader.MoveToNextAttribute());
                                }
                                break;
                            case "photo-caption":
                                break;
                            default:
                                break;
                        }
                        //pathStack.Push(reader.Name);
                        break;
                    case XmlNodeType.Text:
                        Utils.DebugLog("text:" + currentElement + "> " + reader.Value);
                        switch (currentElement)
                        {
                            case "link-url":
                                post.LinkUri = new Uri(reader.Value);
                                break;
                            case "link-text":
                                post.LinkText = reader.Value;
                                break;
                            case "regular-body":
                            case "quote-text":
                            case "photo-caption":
                            case "link-description":
                            case "conversation-text":
                            case "video-caption":
                            case "audio-caption":
                                post.ContentBody = reader.Value;
                                break;
                            case "quote-source":
                                post.ContentFooter = "<p>― <b>" + reader.Value + "</b></p>";
                                break;
                            case "photo-url":
                                if (isThumbnailImageNode)
                                {
                                    post.ImageUri = new Uri(reader.Value);
                                    isThumbnailImageNode = false;
                                }
                                if (isLargeImageNode)
                                {
                                    post.LargeImageUri = new Uri(reader.Value);
                                    isLargeImageNode = false;
                                }
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        //pathStack.Pop();
                        Utils.DebugLog("</" + reader.Name + ">");
                        if (reader.Name == "post")
                        {
                            if (post != null)
                            {
                                if (post.Type == TumblrPost.Types.Link)
                                {
                                    post.ContentHeader = "<div><b><a href=\"" + post.LinkUri.ToString() + "\">" + post.LinkText + "</a></b></div>";
                                }
                                post.Info = post.Tumblelog;
                                if (post.RebloggedFrom != null)
                                {
                                    post.Info += " reblogged " + post.RebloggedFrom;
                                }
                                post.Info += ":";
                                post.Html = "<html>" + post.ContentHeader + post.ContentBody + post.ContentFooter + "</html>";
                                // 既にpostsに存在するIDなら追加しない
                                if (!this.Exists(match => { return (match.Id == post.Id); }))
                                {
                                    this.Add(post);
                                }
                                post = null;
                            }
                        }
                        break;
                    default:
                        break;
                }

                if (post != null)
                {
                    //Utils.DebugLog("post.Id = " + post.Id);
                }
            }

            reader.Close();
            sr.Close();
        }

        private void ParsePostAttributes(XmlTextReader reader, TumblrPost post)
        {
            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    Utils.DebugLog("> " + reader.Name + "=" + reader.Value);
                    switch (reader.Name)
                    {
                        case "id":
                            post.Id = reader.Value;
                            break;
                        case "type":
                            switch (reader.Value)
                            {
                                case "text":
                                    post.Type = TumblrPost.Types.Text;
                                    break;
                                case "quote":
                                    post.Type = TumblrPost.Types.Quote;
                                    break;
                                case "photo":
                                    post.Type = TumblrPost.Types.Photo;
                                    break;
                                case "link":
                                    post.Type = TumblrPost.Types.Link;
                                    break;
                                case "chat":
                                    post.Type = TumblrPost.Types.Chat;
                                    break;
                                case "video":
                                    post.Type = TumblrPost.Types.Video;
                                    break;
                                case "audio":
                                    post.Type = TumblrPost.Types.Audio;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "format":
                            post.Format = reader.Value;
                            break;
                        case "reblog-key":
                            post.ReblogKey = reader.Value;
                            break;
                        case "tumblelog":
                            post.Tumblelog = reader.Value;
                            break;
                        case "reblogged-from-name":
                            post.RebloggedFrom = reader.Value;
                            break;
                        case "reblogged-from-url":
                            post.RebloggedFromUri = new Uri(reader.Value);
                            break;
                        case "reblogged-root-name":
                            post.RebloggedRoot = reader.Value;
                            break;
                        case "reblogged-root-url":
                            post.RebloggedRootUri = new Uri(reader.Value);
                            break;
                        case "liked":
                            post.IsLiked = (reader.Value == "true");
                            break;
                        default:
                            break;
                    }
                }
                while (reader.MoveToNextAttribute());
            }
        }

        private void ParseTumblelogAttributes(XmlTextReader reader, TumblrPost post)
        {
            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    Utils.DebugLog("> " + reader.Name + "=" + reader.Value);
                    switch (reader.Name)
                    {
                        case "avatar-url-64":
                            post.AvatarImageUri = new Uri(reader.Value);
                            break;
                        default:
                            break;
                    }
                }
                while (reader.MoveToNextAttribute());
            }
        }

        #region スクレイパー(旧仕様)
        #if false
        /// <summary>
        /// dashboard の HTML のスクレイピングを行う
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private void Scrape(string html)
        {
            /*
             * <ol id="posts" > ～ </ol>
             * <li id="post ～ </li>
             * <div class="post_content"> ～ <div class="clear"></div>
             * src="～"
             * 
             * photourl
             * 75, 100, 250, 400, 500
             * 
             * <a id="next_page_link" href="/dashboard/2/238122002">
             *   238122002 は 1 ページ目の最後の post
             */

            int pPostsStart = html.IndexOf("<ol id=\"posts\"");
            if (pPostsStart == -1)
            {
                // FIXME
            }
            int pPostsEnd = html.IndexOf("</ol>", pPostsStart);

            TumblrPost previousPost = null;

            int pos = pPostsStart;
            while (true)
            {
                int pPostStart = html.IndexOf("<li id=\"post", pos, pPostsEnd - pos);
                if (pPostStart == -1) { break; }
                int pPostEnd = html.IndexOf("</li>", pPostStart, pPostsEnd - pPostStart);
                if (pPostEnd == -1) { break; }
                pPostEnd += 5;

                // 次回ループのため
                pos = pPostEnd;

                XmlDocument xmlDoc = new XmlDocument();
                string xml = html.Substring(pPostStart, pPostEnd - pPostStart);
                xml = Utils.ReplaceCharacterEntityReferences(xml);
                xml = Utils.RemoveWhiteSpaces(xml);
                // album_artでのalt重複を修正
                xml = xml.Replace("alt=\"\" ", "");

                try
                {
                    xmlDoc.LoadXml(xml);
                }
                catch (Exception e)
                {
                    Utils.DebugLog(e);
                    continue;
                }

                /*
                 * <li id="postXXXXXXXXX">～</li>
                 */
                if (xmlDoc.DocumentElement.NodeType != XmlNodeType.Element ||
                    xmlDoc.DocumentElement.Name != "li")
                {
                    continue;
                }

                Dictionary<string, string> attrs = GetElementAttributes(xmlDoc.DocumentElement);

                if (attrs.ContainsKey("id"))
                {
                    if (attrs["id"].StartsWith("post"))
                    {
                        TumblrPost post = new TumblrPost();

                        post.Id = attrs["id"].Substring(4);
                        Utils.DebugLog("post id = " + post.Id);

                        string classes = attrs["class"];

                        // post type を決定
                        if (classes.IndexOf("regular") != -1)
                        {
                            post.Type = TumblrPost.Types.Text;
                        }
                        else if (classes.IndexOf("photo") != -1)
                        {
                            post.Type = TumblrPost.Types.Photo;
                        }
                        else if (classes.IndexOf("quote") != -1)
                        {
                            post.Type = TumblrPost.Types.Quote;
                        }
                        else if (classes.IndexOf("link") != -1)
                        {
                            post.Type = TumblrPost.Types.Link;
                        }
                        else if (classes.IndexOf("conversation") != -1)
                        {
                            post.Type = TumblrPost.Types.Chat;
                        }
                        else if (classes.IndexOf("audio") != -1)
                        {
                            post.Type = TumblrPost.Types.Audio;
                        }
                        else if (classes.IndexOf("video") != -1)
                        {
                            post.Type = TumblrPost.Types.Video;
                        }

                        if (classes.IndexOf("is_mine") != -1)
                        {
                            post.IsMine = true;
                        }

                        if (classes.IndexOf("same_user_as_last") != -1)
                        {
                            post.IsSameUserAsLast = true;
                        }

                        if (classes.IndexOf("is_reblog") != -1)
                        {
                            post.IsReblog = true;
                        }

                        /*
                         *   <div class="post_controls">～</div>
                         *   <div class="post_info">～</div>
                         *   <div class="post_content">～</div>
                         */
                        ScrapePost(xmlDoc.DocumentElement, ref post);

                        if (post.Info == null)
                        {
                            if (post.IsMine && !post.IsReblog)
                            {
                                post.Info = "You:";
                            }
                            else if (post.IsSameUserAsLast)
                            {
                                if (previousPost != null)
                                {
                                    post.Info = previousPost.Info;
                                }
                            }
                        }

                        this.Add(post);
                        previousPost = post;
                    }
                }
            }

            // FIXME Enable endless scrollingオン時に動作するかどうか確認
            //Utils.DebugLog("next_page_link");
            this.NextPageUri = null;
            int pLinkStart = html.IndexOf("<a id=\"next_page_link\" href=\"", pPostsEnd);
            if (pLinkStart != -1)
            {
                pLinkStart += 29;
                int pLinkEnd = html.IndexOf("\">", pLinkStart);
                if (pLinkEnd != -1)
                {
                    string uri = "http://www.tumblr.com" + html.Substring(pLinkStart, pLinkEnd - pLinkStart);
                    //Utils.DebugLog(uri);
                    this.NextPageUri = new Uri(uri);
                }
            }
        }

        private void ScrapePost(XmlNode postNode, ref TumblrPost post)
        {
            // <li id="postXXXXXXXXX"> 以下の <div> について処理
            foreach (XmlNode div in postNode.ChildNodes)
            {
                Dictionary<string, string> attrs = GetElementAttributes(div);

                if (!attrs.ContainsKey("class"))
                {
                    continue;
                }

                if (attrs["class"] == "post_controls")
                {
                    ScrapePostControls(div, ref post);
                }
                else if (attrs["class"] == "post_info")
                {
                    // 同じユーザによるreblogでないpostが連続する場合，最初の1つにしか出現しない
                    // 自分のpostの場合も出現しない
                    ScrapePostInfo(div, ref post);
                }
                else if (attrs["class"] == "post_content")
                {
                    ScrapePostContent(div, ref post);
                }
                else if (attrs["class"] == "so_ie_doesnt_treat_this_as_inline")
                {
                    ScrapePostUserInfo(div, ref post);
                }
            }

            //post.Type = TumblrPost.Types.Text;
            //post.Info = "dummy";
            //post.Html = "<p>dummy</p>";
        }

        private void ScrapePostControls(XmlNode node, ref TumblrPost post)
        {
            Utils.DebugLog("post_controls");

            foreach (XmlNode n in node.ChildNodes)
            {
                Dictionary<string, string> attrs = GetElementAttributes(n);

                if (n.Name == "a" && attrs.ContainsKey("href"))
                {
                    if (attrs["href"].StartsWith("/reblog/"))
                    {
                        post.ReblogActionUri = new Uri("http://www.tumblr.com" + attrs["href"]);
                    }
                }
                else if (n.Name == "form" && attrs.ContainsKey("action") && attrs.ContainsKey("style"))
                {
                    // unlike のフォームと like のフォームが存在
                    // form の style に display:none; が入ってるほうが現在の状態
                    if (attrs["action"].StartsWith("/unlike/"))
                    {
                        post.UnlikeActionUri = new Uri("http://www.tumblr.com" + attrs["action"]);
                        if (attrs["style"] == "display:none;")
                        {
                            post.IsLiked = false;
                        }
                    }
                    else if (attrs["action"].StartsWith("/like/"))
                    {
                        post.LikeActionUri = new Uri("http://www.tumblr.com" + attrs["action"]);
                        if (attrs["style"] == "display:none;")
                        {
                            post.IsLiked = true;
                        }
                    }

                    foreach (XmlNode n2 in n.ChildNodes)
                    {
                        if (n2.Name == "input")
                        {
                            Dictionary<string, string> a2 = GetElementAttributes(n2);
                            if (a2.ContainsKey("id"))
                            {
                                if (a2["id"] == "form_key")
                                {
                                    post.ReblogKey = a2["value"];
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ScrapePostInfo(XmlNode node, ref TumblrPost post)
        {
            string text = node.InnerText;
            text = text.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");
            post.Info = text;
            Utils.DebugLog("post_info = " + post.Info);
        }

        private void ScrapePostContent(XmlNode node, ref TumblrPost post)
        {
            Utils.DebugLog("post_content");

            post.Html = "<content>" + node.InnerXml + "</content>";

            if (post.Type == TumblrPost.Types.Photo)
            {
                Dictionary<string, string> attr;
                
                attr = GetElementAttributes(node.FirstChild.FirstChild);
                if (attr.ContainsKey("src"))
                {
                    post.ImageUri = new Uri(attr["src"]);
                }
                
                attr = GetElementAttributes(node.ChildNodes[1].FirstChild.FirstChild);
                if (attr.ContainsKey("src"))
                {
                    post.LargeImageUri = new Uri(attr["src"]);
                }

                attr = null;
            }
        }

        private void ScrapePostUserInfo(XmlNode node, ref TumblrPost post)
        {
            Utils.DebugLog("userinfo");

            foreach (XmlNode n in node.ChildNodes)
            {
                Dictionary<string, string> attrs = GetElementAttributes(n);

                if (n.Name == "a" && attrs.ContainsKey("class"))
                {
                    if (attrs["class"] == "avatar")
                    {
                        string style = attrs["style"];
                        int sp = style.IndexOf("url('");
                        if (sp != -1)
                        {
                            sp += 5;
                            int ep = style.IndexOf("')", sp);
                            if (ep != -1)
                            {
                                post.AvatarImageUri = new Uri(style.Substring(sp, ep - sp));
                                Utils.DebugLog(post.AvatarImageUri);
                            }
                        }
                    }
                }
            }
        }

        private Dictionary<string, string> GetElementAttributes(XmlNode elem)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            XmlAttributeCollection attrs = elem.Attributes;
            if (attrs != null)
            {
                foreach (XmlAttribute attr in attrs)
                {
                    //Utils.DebugLog(attr.Name + "=" + attr.Value);
                    dict.Add(attr.Name, attr.Value);
                }
            }

            return dict;
        }
        private void Scrape_old(string html)
        {
            /*
             * <ol id="posts" > ～ </ol>
             * <li id="post ～ </li>
             * <div class="post_content"> ～ <div class="clear"></div>
             * src="～"
             * 
             * photourl
             * 75, 100, 250, 400, 500
             * 
             * <a id="next_page_link" href="/dashboard/2/238122002">
             *   238122002 は 1 ページ目の最後の post
             */

            int pPostsStart = html.IndexOf("<ol id=\"posts\"");
            int pPostsEnd = html.IndexOf("</ol>", pPostsStart);

            int pos = pPostsStart;
            TumblrPost previousPost = null;

            while (true)
            {
                int pPostStart = html.IndexOf("<li id=\"post", pos, pPostsEnd - pos);
                if (pPostStart == -1) { break; }
                int pPostEnd = html.IndexOf("</li>", pPostStart, pPostsEnd - pPostStart);
                if (pPostEnd == -1) { break; }

                string tmp = html.Substring(pPostStart, pPostEnd - pPostStart);
                //Utils.DebugLog(tmp);

                // 次回ループのため
                pos = pPostEnd + 5;

                try
                {
                    var post = new TumblrPost();

                    // post id
                    int pPostIdStart = html.IndexOf("<li id=\"post", pPostStart, pPostEnd - pPostStart);
                    if (pPostIdStart == -1) { continue; }
                    pPostIdStart += 12;
                    int pPostIdEnd = html.IndexOf("\"", pPostIdStart, pPostEnd - pPostIdStart);
                    if (pPostIdEnd == -1) { continue; }
                    post.Id = html.Substring(pPostIdStart, pPostIdEnd - pPostIdStart);

                    // post type -------------------------------------------------------------
                    int pPostClassStart = html.IndexOf("class=\"", pPostIdEnd, pPostEnd - pPostIdEnd);
                    if (pPostClassStart == -1) { continue; }
                    int pPostClassEnd = html.IndexOf("\">", pPostClassStart, pPostEnd - pPostClassStart);

                    // TODO is_reblog
                    string classes = html.Substring(pPostClassStart, pPostClassEnd - pPostClassStart);
                    if (classes.IndexOf("regular") != -1)
                    {
                        post.Type = TumblrPost.Types.Text;
                    }
                    else if (classes.IndexOf("photo") != -1)
                    {
                        post.Type = TumblrPost.Types.Photo;
                    }
                    else if (classes.IndexOf("quote") != -1)
                    {
                        post.Type = TumblrPost.Types.Quote;
                    }
                    else if (classes.IndexOf("link") != -1)
                    {
                        post.Type = TumblrPost.Types.Link;
                    }
                    else if (classes.IndexOf("conversation") != -1)
                    {
                        post.Type = TumblrPost.Types.Chat;
                    }
                    else if (classes.IndexOf("audio") != -1)
                    {
                        post.Type = TumblrPost.Types.Audio;
                    }
                    else if (classes.IndexOf("video") != -1)
                    {
                        post.Type = TumblrPost.Types.Video;
                    }

                    bool isMine = false;
                    if (classes.IndexOf("is_mine") != -1)
                    {
                        isMine = true;
                    }

                    bool isSameUserAsLast = false;
                    if (classes.IndexOf("same_user_as_last") != -1)
                    {
                        isSameUserAsLast = true;
                    }

                    bool isReblog = false;
                    if (classes.IndexOf("is_reblog") != -1)
                    {
                        isReblog = true;
                    }

                    // post_controls
                    int pPostControlsStart = html.IndexOf("<div class=\"post_controls\">", pPostStart, pPostEnd - pPostStart);
                    if (pPostControlsStart == -1)
                    {
                        Utils.DebugLog("post_controls not found");
                        continue;
                    }
                    pPostControlsStart += 27;

                    // post_info
                    // 同じユーザによるreblogでないpostが連続する場合，最初の1つにしか出現しない
                    // 自分のpostの場合も出現しない
                    int pPostInfoStart = html.IndexOf("<div class=\"post_info\">", pPostStart, pPostEnd - pPostStart);
                    if (pPostInfoStart == -1)
                    {
                        if (isMine && !isReblog)
                        {
                            post.Info = "You:";
                        }
                        else if (isSameUserAsLast)
                        {
                            if (previousPost == null)
                            {
                                Utils.DebugLog("post_info not found");
                                continue;
                            }
                            else
                            {
                                post.Info = previousPost.Info;
                            }
                        }
                    }
                    else
                    {
                        pPostInfoStart += 23;

                        int pPostInfoEnd = html.IndexOf("</div>", pPostInfoStart, pPostEnd - pPostInfoStart);
                        if (pPostInfoEnd == -1) { continue; }

                        post.Info = html.Substring(pPostInfoStart, pPostInfoEnd - pPostInfoStart).TrimStart(' ', '\r', '\n').TrimEnd(' ', '\r', '\n');
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.PreserveWhitespace = false;
                        string xml = null;
                        try
                        {
                            xml = html.Substring(pPostInfoStart, pPostInfoEnd - pPostInfoStart);
                            xml = "<postinfo>" + xml + "</postinfo>";
                            xmlDoc.LoadXml(xml);
                            string text = xmlDoc.DocumentElement.InnerText;
                            text = text.Replace("\r\n", "");
                            text = text.Replace("\r", "");
                            text = text.Replace("\n", "");
                            text = text.Replace("\t", "");
                            // FIXME さすがに遅い
                            while (text.IndexOf("  ") != -1)
                            {
                                text = text.Replace("  ", " ");
                            }
                            text = text.Trim();
                            //Utils.DebugLog(text);

                            post.Info = text;
                        }
                        catch (Exception e)
                        {
                            Utils.DebugLog(e);
                            Utils.DebugLog(xml);
                        }
                    }

                    // post_content
                    // +26
                    int pPostContentStart = html.IndexOf("<div class=\"post_content\">", pPostStart, pPostEnd - pPostStart);
                    if (pPostContentStart == -1)
                    {
                        Utils.DebugLog("post_content not found");
                        continue;
                    }
                    pPostContentStart += 26;

                    int pPostContentEnd = html.IndexOf("<div class=\"clear\"></div>", pPostContentStart, pPostEnd - pPostContentStart);
                    if (pPostContentEnd == -1)
                    {
                        Utils.DebugLog("div clear not found");
                        continue;
                    }

                    post.Html = html.Substring(pPostContentStart - 26, pPostContentEnd - (pPostContentStart - 26));

                    if (post.Type == TumblrPost.Types.Photo)
                    {
                        // FIXME
                        // dashboardのPreferencesでShow full size photosをオンにした場合
                        // class="image"のみ出現
                        int pThumbImg = html.IndexOf("class=\"image_thumbnail\"", pPostContentStart, pPostContentEnd - pPostContentStart);

                        int pThumbImgUrlStart = html.IndexOf("src=\"", pThumbImg, pPostContentEnd - pThumbImg);
                        if (pThumbImgUrlStart == -1) { continue; }
                        pThumbImgUrlStart += 5;
                        int pThumbImgUrlEnd = html.IndexOf("\"", pThumbImgUrlStart, pPostContentEnd - pThumbImgUrlStart);
                        string thumbImgUrl = html.Substring(pThumbImgUrlStart, pThumbImgUrlEnd - pThumbImgUrlStart);

                        int pHighresImg = html.IndexOf("class=\"image\"", pThumbImgUrlEnd, pPostContentEnd - pThumbImgUrlEnd);

                        int pHighresImgUrlStart = html.IndexOf("src=\"", pHighresImg, pPostContentEnd - pHighresImg);
                        if (pHighresImgUrlStart == -1) { continue; }
                        pHighresImgUrlStart += 5;
                        int pHighresImgUrlEnd = html.IndexOf("\"", pHighresImgUrlStart, pPostContentEnd - pHighresImgUrlStart);
                        string highresImgUrl = html.Substring(pHighresImgUrlStart, pHighresImgUrlEnd - pHighresImgUrlStart);

                        post.ImageUri = new Uri(thumbImgUrl);
                        post.LargeImageUri = new Uri(highresImgUrl);
                    }

                    this.Add(post);
                    previousPost = post;
                }
                catch (Exception e)
                {
                }
            }

            // FIXME Enable endless scrollingオン時に動作するかどうか確認
            //Utils.DebugLog("next_page_link");
            this.NextPageUri = null;
            int pLinkStart = html.IndexOf("<a id=\"next_page_link\" href=\"", pPostsEnd);
            if (pLinkStart != -1)
            {
                pLinkStart += 29;
                int pLinkEnd = html.IndexOf("\">", pLinkStart);
                if (pLinkEnd != -1)
                {
                    string uri = "http://www.tumblr.com" + html.Substring(pLinkStart, pLinkEnd - pLinkStart);
                    //Utils.DebugLog(uri);
                    this.NextPageUri = new Uri(uri);
                }
            }
        }
        #endif
        #endregion
    }

    /// <summary>
    /// 投稿内容
    /// </summary>
    public class TumblrPost
    {
        public enum ImageType { Avatar, Normal, Large };
        public enum Priority { Normal, High };

        public string Id { get; set; }
        public string ReblogKey { get; set; }

        public Uri ImageUri { get; set; }
        public Uri LargeImageUri { get; set; }
        public Uri AvatarImageUri { get; set; }

        // scrape版用
        public Uri ReblogActionUri { get; set; }
        public Uri LikeActionUri { get; set; }
        public Uri UnlikeActionUri { get; set; }

        // ----
        // regular: regular-title, regular-body
        // quote: quote-text, quote-source
        // photo: photo-caption, photo-link-url, photo-url
        // link: link-text, link-url, link-description
        // conversation: conversation-title, conversation-text
        // audio: audio-caption, download-url
        // video: video-caption
        // question: question, answer
        public string ContentHeader { get; set; }
        public string ContentBody { get; set; }
        public string ContentFooter { get; set; }

        public string Format { get; set; }
        public string Tumblelog { get; set; }
        public string RebloggedFrom { get; set; }
        public Uri RebloggedFromUri { get; set; }
        public string RebloggedRoot { get; set; }
        public Uri RebloggedRootUri { get; set; }

        public string LinkText { get; set; }
        public Uri LinkUri { get; set; }
        // ----

        public string Html { get; set; }
        public string Text { get; set; }
        public Image Image { get; set; }
        public Image LargeImage { get; set; }
        public Image AvatarImage { get; set; }
        public string Info { get; set; }

        public bool IsMine { get; set; }
        public bool IsSameUserAsLast { get; set; }
        public bool IsReblog { get; set; }
        public bool IsLiked { get; set; }

        public enum Types { Text, Photo, Quote, Link, Chat, Audio, Video };
        public Types Type { get; set; }

        public void GetImage(ImageType size, Priority pri, WaitCallback progressChanged, WaitCallback completed)
        {
            JobQueue.Priority jqpri;
            if (pri == Priority.High)
            {
                jqpri = JobQueue.Priority.Normal;
            }
            else
            {
                jqpri = JobQueue.Priority.Low;
            }

            // キューに追加
            JobQueue.Enqueue(jqpri, new JobCallback(GetImageAsync), progressChanged, completed, size);
        }

        private JobResult GetImageAsync(WaitCallback jobProgressChangedDelegate, object o)
        {
            Utils.DebugLog("running GetImageAsync");

            ImageType size = (ImageType)o;

            Uri uri = null;
            switch (size)
            {
                case ImageType.Avatar:
                    uri = this.AvatarImageUri;
                    break;
                case ImageType.Normal:
                    uri = this.ImageUri;
                    break;
                case ImageType.Large:
                    uri = this.LargeImageUri;
                    break;
            }

            if (uri == null)
            {
                return new JobResult { Object = this, IsError = true, Status = 0 };
            }

            // まずキャッシュが存在するかチェック
            Stream stream;
            if (Cache.Exists(uri))
            {
                // ヒット
                stream = Cache.Get(uri);
            }
            else
            {
                // ミス

                HttpWebResponse r = TumblrClient.Fetch(uri);
                if (r == null)
                {
                    // FIXME
                    return new JobResult { Object = this, IsError = true, Status = 0 };
                }

                Stream respStream = r.GetResponseStream();
                stream = new MemoryStream();

                // 受信したデータをメモリストリームへ書き込む
                // respStreamの長さは不明なので，読めなくなるまで読み込む
                byte[] buf = new byte[2048];
                while (true)
                {
                    int readLen = respStream.Read(buf, 0, buf.Length);
                    if (readLen <= 0) { break; }

                    stream.Write(buf, 0, readLen);
                }
                Utils.DebugLog("total length = " + stream.Length);

                respStream.Close();
                r.Close();

                // キャッシュに追加する
                Cache.AddStream(uri, stream);
            }

            Image img = null;
            try
            {
                // ストリームから画像生成
                img = new Bitmap(stream);
            }
            catch (Exception e)
            {
                Utils.DebugLog(e);
            }
            finally
            {
                stream.Close();
            }

            switch (size)
            {
                case ImageType.Avatar:
                    this.AvatarImage = img;
                    break;
                case ImageType.Normal:
                    this.Image = img;
                    break;
                case ImageType.Large:
                    this.LargeImage = img;
                    break;
            }

            return new JobResult { Object = this, IsError = false, Status = 0 };
        }

        public void Reblog(WaitCallback progressChanged, WaitCallback completed)
        {
            JobQueue.Enqueue(JobQueue.Priority.High, new JobCallback(ReblogAsync), progressChanged, completed, null);
        }

        private JobResult ReblogAsync(WaitCallback jobProgressChangedDelegate, object o)
        {
            Utils.DebugLog("running ReblogAsync");

            if (!TumblrClient.Reblog(this.Id, this.ReblogKey, null))
            {
                return new JobResult { Object = this, IsError = true, Status = 0 };
            }

            return new JobResult { Object = this, IsError = false, Status = 0 };
            
            string html = null;

            try
            {
                HttpWebResponse r = TumblrClient.Fetch(ReblogActionUri);
                if (r == null)
                {
                    // FIXME
                    return new JobResult { Object = this, IsError = true, Status = 0 };
                }

                Stream stream = r.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                html = sr.ReadToEnd();
                sr.Close();
                stream.Close();
                r.Close();
            }
            catch (Exception e)
            {
                Utils.DebugLog(e);
                return new JobResult { Object = this, IsError = true, Status = 0 };
            }

            Dictionary<string, string> param;
            param = GrabFormValues(html);

            // build params
            StringBuilder sb = new StringBuilder();
            foreach (var item in param)
            {
                sb.Append(item.Key + "=");
                if(item.Value != null)
                {
                    sb.Append(Uri.EscapeDataString(item.Value));
                }
                sb.Append("&");
            }
            Utils.DebugLog("-----------------------------------------");
            Utils.DebugLog("POST: " + sb.ToString());

            try
            {
                // 本来はReblogActionUriのかわりにformのactionの値を用いるべき
                HttpWebResponse r = TumblrClient.Request(ReblogActionUri, "POST", sb.ToString());
                if (r == null)
                {
                    // FIXME
                    return new JobResult { Object = this, IsError = true, Status = 0 };
                }

                Stream stream = r.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                //Utils.DebugLog(sr.ReadToEnd());
                sr.ReadToEnd();
                sr.Close();
                stream.Close();
                r.Close();
            }
            catch (Exception e)
            {
                Utils.DebugLog(e);
                return new JobResult { Object = this, IsError = true, Status = 0 };
            }

            return new JobResult { Object = this, IsError = false, Status = 0 };
        }

        private static Dictionary<string, string> GrabFormValues(string html)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            // id="edit_post"なformを探す
            int pFormStart = 0, pFormTagEnd = 0, pFormEnd = -1;
            while (pFormEnd < 0)
            {
                pFormStart = html.IndexOf("<form ", pFormTagEnd);
                if (pFormStart == -1)
                {
                    return dict;
                }
                pFormTagEnd = html.IndexOf(">", pFormStart + 6);
                pFormTagEnd += 1;
                string formTag = html.Substring(pFormStart, pFormTagEnd - pFormStart);
                //Utils.DebugLog(formTag);
                if (formTag.IndexOf("id=\"edit_post\"") != -1)
                {
                    pFormEnd = html.IndexOf("/form>", pFormTagEnd);
                    pFormEnd += 6;
                }
            }

            //Utils.DebugLog(html.Substring(pFormStart, pFormEnd - pFormStart));

            string form = html.Substring(pFormStart, pFormEnd - pFormStart);

            /* XMLとして処理するのはちょっとしんどいかも(2010/02/22) */

            // inputのvalue，textarea，selectのselected="selected"なoptionの値を取得
            Regex r = new Regex(@"<(?'elem'input|textarea|select)\s(?'attrs'[^>]+)(?:>(?'inner'.*?)</\1>|\/\s*>)",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection mc = r.Matches(form);
            foreach (Match m in mc)
            {
                Utils.DebugLog(m.Groups["elem"].Value + ": " + m.Groups["attrs"].Value);
                Utils.DebugLog("inner: " + m.Groups["inner"].Value);

                string elem = m.Groups["elem"].Value.ToLower();
                Dictionary<string, string> attrs = GetElementAttributes(m.Groups["attrs"].Value);
                string inner = m.Groups["inner"].Value;

                if (elem == "select")
                {
                    // selectedなoptionの値を取得
                    Regex ropt = new Regex("<option\\s+value=\"([^\"]*)\"\\s+selected=\"selected\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    Match mopt = ropt.Match(inner);

                    dict.Add(attrs["name"], mopt.Groups[1].Value);
                }
                else if(elem == "textarea")
                {
                    // 文字実体参照を解決しておく
                    string tmp = Utils.ReplaceCharacterEntityReferences(inner);
                    tmp = Regex.Replace(tmp, @"&#(\d+);", new MatchEvaluator(ReplaceCharRef));
                    Utils.DebugLog("----------------------------------------");
                    Utils.DebugLog("RESOLVED TEXT: " + tmp);
                    dict.Add(attrs["name"], tmp);
                }
                else if (elem == "input")
                {
                    if (attrs["type"].ToLower() != "submit")
                    {
                        if (attrs.ContainsKey("value"))
                        {
                            dict.Add(attrs["name"], attrs["value"]);
                        }
                        else
                        {
                            dict.Add(attrs["name"], null);
                        }
                    }
                }
            }

            foreach (var i in dict)
            {
                Utils.DebugLog(i.Key + "=" + i.Value);
            }

            return dict;
        }

        private static string ReplaceCharRef(Match m)
        {
            string ret = null;
            try
            {
                ret = Char.ToString((char)int.Parse(m.Groups[1].Value));
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString() + ": " + m.Groups[1].Value);
            }
            return ret;
        }

        private static Dictionary<string, string> GetElementAttributes(string element)
        {
            Dictionary<string, string> attr = new Dictionary<string,string>();

            Regex r = new Regex("\\s*([^=]+)=\"([^\"]*)\"\\s*", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection mc = r.Matches(element);
            foreach (Match m in mc)
            {
                attr.Add(m.Groups[1].Value, m.Groups[2].Value);
            }
            
            return attr;
        }

        private static string GetStringBetween(string str, string begin, string end, int pos)
        {
            int pBegin = str.IndexOf(begin, pos);
            if (pBegin == -1) { return null; }

            int pEnd = str.IndexOf(end, pBegin + begin.Length);
            if (pEnd == -1) { return null; }
            pEnd += end.Length;

            return str.Substring(pBegin, pEnd - pBegin);
        }

        public void Like(bool likeOrNot, WaitCallback progressChanged, WaitCallback completed)
        {
            JobQueue.Enqueue(JobQueue.Priority.High, new JobCallback(LikeAsync), progressChanged, completed, !this.IsLiked);
        }

        private JobResult LikeAsync(WaitCallback jobProgressChangedDelegate, object o)
        {
            Utils.DebugLog("running LikeAsync");

            bool like = (bool)o;

            if (!TumblrClient.Like(this.Id, this.ReblogKey, like))
            {
                return new JobResult { Object = this, IsError = true, Status = 0 };
            }

            return new JobResult { Object = this, IsError = false, Status = 0 };

            // --------

            Uri actionUri = null;
            if (like)
            {
                actionUri = this.LikeActionUri;
            }
            else
            {
                actionUri = this.UnlikeActionUri;
            }

            // id, redirect_to, form_key
            StringBuilder sb = new StringBuilder();
            sb.Append("id=");
            sb.Append(Uri.EscapeDataString(this.Id));
            //sb.Append("&redirect_to=/dashboard");
            sb.Append("&form_key=");
            sb.Append(Uri.EscapeDataString(this.ReblogKey));

            HttpWebResponse r = TumblrClient.Request(actionUri, "POST", sb.ToString());
            if (r == null)
            {
                // FIXME
                return new JobResult { Object = this, IsError = true, Status = 0 };
            }

            Stream stream = r.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            Utils.DebugLog(sr.ReadToEnd());
            sr.Close();
            stream.Close();
            r.Close();

            return new JobResult { Object = this, IsError = false, Status = 0 };
        }
    }

    public class CacheItem
    {
        public byte[] Data { get; set; }
        public int RefCount { get; set; }
        public int Type { get; set; }
        public string Guid { get; set; }

        public CacheItem()
        {
            this.RefCount = 0;
        }
    }

    /*
     * キャッシュ
     * 主にオンメモリ
     * URLとデータの関連付け
     */
    public sealed class Cache : Dictionary<Uri,CacheItem>
    {
        private static readonly Cache instance = new Cache();

        public static Cache Instance
        {
            get { return instance; }
        }

        public static int EntryCount
        {
            get { return instance.Count; }
        }

        public static int Size
        {
            get
            {
                int size = 0;
                foreach (var item in instance.Values)
                {
                    size += item.Data.Length;
                }
                return size;
            }
        }

        private Cache()
        {
        }

        public static bool Exists(Uri uri)
        {
            if (uri == null)
            {
                return false;
            }

            if (instance.ContainsKey(uri))
            {
                Utils.DebugLog("Cache Hit: " + uri.ToString());
                return true;
            }

            Utils.DebugLog("Cache Miss: " + uri.ToString());
            return false;
        }

        public static void AddStream(Uri uri, Stream stream)
        {
            // streamはseekableでLengthプロパティがサポートされていること
            // いっそMemoryStream限定にしてしまおうか

            CacheItem item = new CacheItem();
            item.Data = new byte[stream.Length];
            //Utils.DebugLog("stream length = " + stream.Length.ToString());

            stream.Seek(0, SeekOrigin.Begin);
            int l = stream.Read(item.Data, 0, (int)stream.Length);
            //Utils.DebugLog(l.ToString() + "bytes read");
            instance.Add(uri, item);
        }

        public static Stream Get(Uri uri)
        {
            if (instance.ContainsKey(uri))
            {
                instance[uri].RefCount++;
                return new MemoryStream(instance[uri].Data, false);
            }

            return null;
        }

        public static void Truncate()
        {
        }
    }

    public class FormBuilder
    {
        private Dictionary<string, string> items;
        private string boundary;

        public FormBuilder()
        {
            items = new Dictionary<string,string>();
            boundary = "---------------------------" + DateTime.Now.Ticks.ToString();
        }

        public string Boundary
        {
            get { return boundary; }
        }

        public void Add(string name, string value)
        {
            items[name] = value;
        }

        public string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in items)
            {
                sb.Append("--" + boundary + "\r\n");
                sb.Append("Content-Disposition: form-data; name=\"" + item.Key + "\"\r\n\r\n");
                sb.Append(item.Value + "\r\n");
            }
            sb.Append("--" + boundary + "--\r\n");

            return sb.ToString();
        }
    }

    public class PostDataBuilder
    {
        private Dictionary<string,string> data;

        public PostDataBuilder()
        {
            data = new Dictionary<string,string>();
        }

        public void Add(string key, string value)
        {
            this.data.Add(key, value);
        }

        public void Clear()
        {
            this.data.Clear();
        }

        public string ToString()
        {
            List<string> dataList = new List<string>();
            foreach(var d in data)
            {
                dataList.Add(Utils.UrlEncode(d.Key, Encoding.UTF8) + "=" + Utils.UrlEncode(d.Value, Encoding.UTF8));
            }
            return String.Join("&", dataList.ToArray());
        }
    }
}
