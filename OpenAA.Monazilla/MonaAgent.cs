namespace OpenAA.Monazilla
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Linq;
    using System.IO;
    using System.Web;
    using System.Net;
    using System.Net.Http;
    using NLog;

    using OpenAA.Net;
    using OpenAA.Net.Http;
    using OpenAA.Extensions.String;
    using OpenAA.IO;

    using OpenAA.Monazilla.Models;

    public class MonaAgent
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        // ----------------------------------------------------------------
        // 板一覧

        /// <summary>
        /// bbsmenu.htmlのカテゴリ行
        /// </summary>
        private static readonly Regex RegexBbsMenuCategory = new Regex(
            @"<BR><BR><B>(?<cate>.+?)<\/B><BR>",
            RegexOptions.IgnoreCase | 
            RegexOptions.Singleline | 
            RegexOptions.Compiled);

        /// <summary>
        /// bbsmenu.htmlの板行
        /// </summary>
        private static readonly Regex RegexBbsMenuBoard = new Regex(
            @"<A HREF=(?<serv>http:\/\/.*(\.2ch\.net|\.bbspink\.com|\.machi\.to))\/(?<code>[^\s]*).*>(?<name>.+)<\/A>",
            RegexOptions.IgnoreCase | 
            RegexOptions.Singleline | 
            RegexOptions.Compiled);

        /// <summary>
        /// subject.txtの1行
        /// </summary>
        private static readonly Regex RegexSubject = new Regex(
            @"(?<id>[0-9]*).dat\<\>(?<title>.*?)\((?<nums>[0-9]*)\)", 
            RegexOptions.IgnoreCase | 
            RegexOptions.Singleline | 
            RegexOptions.Compiled);

        /// <summary>
        /// bbsmenu.htmlから除外するカテゴリ
        /// </summary>
        private static readonly List<string> IgnoreCategories = new List<string>{ "特別企画", "チャット", "ツール類" };

        /// <summary>
        /// bbsmenu.htmlから除外する板
        /// </summary>
        private static readonly List<string> IgnoreBoards     = new List<string>{ "2chプロジェクト", "いろいろランク" };

        /// <summary>
        /// Gets the bbsmenu.html.
        /// </summary>
        /// <returns>The bbs menu html.</returns>
        public async Task<string> GetBbsMenuHtml()
        {
            var html = GetBbsMenuFromCache();
            if (string.IsNullOrEmpty(html))
            {
                html = await GetBbsMenuFromOnline();
            }
            return html;
        }

        public async Task<string> GetBbsMenuFromOnline(string url = "http://menu.2ch.net/bbsmenu.html", bool doNotSaveToCache = false)
        {
            var encoding = Encoding.GetEncoding("shift_jis");

            // 受信
            var html = "";
            using (var agent = new HttpAgent())
            {
                html = await agent.GetStringAsync(url, encoding);
            }

            // キャッシュ保存
            if (!doNotSaveToCache)
            {
                var path = this.GetBbsMenuCachePath();
                FileUtility.Write(path, html, append:false, encoding:encoding);
            }

            return html;
        }

        public string GetBbsMenuFromCache()
        {
            var encoding = Encoding.GetEncoding("shift_jis");
            var path = this.GetBbsMenuCachePath();
            var html = FileUtility.ReadToEnd(path, encoding);
            return html;
        }

        public IList<MonaBoard> ParseBbsMenu(string bbsMenuHtml)
        {
            var boards = new List<MonaBoard>();

            string cate = "";
            string serv = "";
            string code = "";
            string name = "";

            foreach (var line in bbsMenuHtml.SplitToLines())
            {
                // カテゴリ行か？
                var match = RegexBbsMenuCategory.Match(line);
                if (match.Success)
                {
                    cate = match.Groups["cate"].Value;
                    if (IgnoreCategories.Contains(cate))
                    {
                        cate = null;
                    }
                    _logger.Trace("cate=" + cate);
                    continue;
                }
                // カテゴリーが選択されているか？
                if (string.IsNullOrEmpty(cate))
                {
                    continue;
                }

                // ボード行か？
                var mBoard = RegexBbsMenuBoard.Match(line);
                if (mBoard.Success)
                {
                    serv = mBoard.Groups["serv"].Value;
                    code = mBoard.Groups["code"].Value.Trim('/');
                    name = mBoard.Groups["name"].Value;

                    if (string.IsNullOrEmpty(serv) || string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name))
                    {
                        continue;
                    }

                    if (IgnoreBoards.Contains(name))
                    {
                        continue;
                    }
                    var board = new MonaBoard
                    {
                        Category = cate,
                        Server = serv,
                        Id = code,
                        Name = name
                    };
                    _logger.Trace(board);

                    boards.Add(board);
                }
            }

            return boards;
        }

        /// <summary>
        /// 板一覧を取得する。
        /// </summary>
        /// <returns>The boards.</returns>
        public async Task<IList<MonaBoard>> GetBoards()
        {
            var html = await GetBbsMenuHtml();
            var boards = ParseBbsMenu(html);
            return boards;
        }

        /// <summary>
        /// 板を取得する
        /// </summary>
        /// <returns>The board.</returns>
        /// <param name="url">URL.</param>
        public async Task<MonaBoard> GetBoard(string url)
        {
            var server = "";
            var boardId = "";
            var m1 = Regex.Match(url, @"^(?<server>http:\/\/.*(\.2ch\.net|\.bbspink\.com|\.machi\.to))\/test\/read.cgi\/(?<board>[^\s|\/]*)");
            if (m1.Success)
            {
                server = m1.Groups["server"].Value;
                boardId = m1.Groups["board"].Value;
            }
            else
            {
                var m2 = Regex.Match(url, @"^(?<server>http:\/\/.*(\.2ch\.net|\.bbspink\.com|\.machi\.to))\/(?<board>[0-9a-zA-Z\-_]+)");
                if (m2.Success)
                {
                    server  = m2.Groups["server"].Value;
                    boardId = m2.Groups["board"].Value;
                }
                else
                {
                    return null;
                }
            }
            return await GetBoard(server, boardId);
        }

        /// <summary>
        /// 板を取得する
        /// </summary>
        /// <returns>The board.</returns>
        /// <param name="server">Server.</param>
        /// <param name="boardId">Board identifier.</param>
        public async Task<MonaBoard> GetBoard(string server, string boardId)
        {
            var host = new Uri(server).Host;
            var boards = await this.GetBoards();
            return boards.FirstOrDefault(x => new Uri(x.Server).Host == host && x.Id == boardId);
        }

        // ----------------------------------------------------------------

        public int SubjectCacheExpire = 120;

        public async Task<string> GetSubject(MonaBoard board)
        {
            var text = GetSubjectFromCache(board);
            if (string.IsNullOrEmpty(text))
            {
                text = await GetSubjectFromOnline(board);
            }
            return text;
        }

        public string GetSubjectFromCache(MonaBoard board)
        {
            var path = this.GetSubjectCachePath(board);
            if (!File.Exists(path))
            {
                return string.Empty;
            }

            var last = File.GetLastWriteTime(path);
            var expr = last.AddSeconds(this.SubjectCacheExpire) - DateTime.Now;
            if (expr.TotalSeconds < 0)
            {
                return string.Empty;
            }

            var encoding = Encoding.GetEncoding("shift_jis");
            var text = FileUtility.ReadToEnd(path, encoding);
            return text;
        }

        public async Task<string> GetSubjectFromOnline(MonaBoard board, bool doNotSaveToCache = false)
        {
            var encoding = Encoding.GetEncoding("shift_jis");
            var url = new Uri(board.Server + "/" + board.Id + "/subject.txt");
            var text = "";
            using (var agent = new HttpAgent())
            {
                text = await agent.GetStringAsync(url, encoding);
            }

            if (!doNotSaveToCache)
            {
                var path = this.GetSubjectCachePath(board);
                FileUtility.Write(path, text, append:false, encoding:encoding);
            }

            return text;
        }

        public IList<MonaThread> ParseSubject(MonaBoard board, string subject)
        {
            var threads = new List<MonaThread>();
            var now = DateTime.Now;
            int cnt = 1;

            foreach (var line in subject.SplitToLines())
            {
                var match = RegexSubject.Match(line);
                if (!match.Success)
                {
                    _logger.Debug("unmatch: " + line);
                    continue;
                }

                var thread = new MonaThread
                {
                    No    = cnt++,
                    Board = board,
                    Id    = match.Groups["id"].Value,
                    Title = match.Groups["title"].Value,
                    Nums  = Int32Utility.ParseOrDefault(match.Groups["nums"].Value),
                    UpdateTime = now,
                };
                threads.Add(thread);
            }

            return threads;
        }

        public async Task<IList<MonaThread>> GetThreads(MonaBoard board)
        {
            var subject = await this.GetSubject(board);
            var threads = this.ParseSubject(board, subject);
            return threads;
        }

        public async Task<IList<MonaThread>> GetThreads(string server, string boardId)
        {
            var board = await this.GetBoard(server, boardId);
            if (board == null)
            {
                throw new ArgumentOutOfRangeException();
            }
            return await this.GetThreads(board);
        }

        // ----------------------------------------------------------------
        // レス投稿

        public async Task CreateResponse(MonaThread thread, string name, string mail, string message)
        {
            var host = new Uri(thread.Board.Server).Host;

            for (;;)
            {
                var result = await CreateResponseCore(thread, name, mail, message);

                if (result.ResultType == MonaAgentPostResult.ResultTypes.SUCCEED)
                {// 書き込み成功
                    break;
                }
                if (result.ResultType == MonaAgentPostResult.ResultTypes.FAILED)
                {// 書き込み失敗
                    break;
                }
                if (result.ResultType == MonaAgentPostResult.ResultTypes.STOPED)
                {// 書けないスレ
                    break;
                }

                if (result.ResultType == MonaAgentPostResult.ResultTypes.KILLED || //やられた
                    result.ResultType == MonaAgentPostResult.ResultTypes.CONTINUE)
                {
                    if (this.Session.Wait.ContainsKey(host))
                    {
                        var span = (int)(this.Session.Wait[host] - DateTime.Now).TotalSeconds;
                        if (0 < span)
                        {
                            await Task.Delay(span * 1000);
                        }
                    }
                }
            }
        }

        public async Task<MonaAgentPostResult> CreateResponseCore(MonaThread thread, string name, string mail, string message)
        {
            var domain = new Uri(thread.Board.Server).Host;
            var bbsCgi = thread.Board.Server + "/test/bbs.cgi?guid=ON";

            // 投稿データ
            var encoding = Encoding.GetEncoding("shift_jis");
            var content = new DictionaryContent( new Dictionary<string,string>()
            {
                    { "submit" , "書き込む" },
                    { "FROM"   , name },
                    { "bbs"    , thread.Board.Id },
                    { "key"    , thread.Id },
                    { "time"   , "1104688508" },
                    { "mail"   , mail },
                    { "MESSAGE", message },
                    { "yuki"   , "akari" },
            }, encoding);

            // エージェント
            var agent = new HttpAgent();

            // cookie書き込み
            var cookie = new CookieContainer();
            if (this.Session.HAP != string.Empty)
            {
                cookie.Add(new Cookie("HAP", this.Session.HAP, "/", domain));
            }
            if (this.Session.PON != string.Empty)
            {
                cookie.Add(new Cookie("PON", this.Session.PON, "/", domain));
            }

            agent.HttpClientHandler.CookieContainer = cookie;

            // referer
            agent.DefaultRequestHeaders.Referrer = new Uri(thread.Board.Server + "/test/read.cgi/" + thread.Board.Id + "/" + thread.Id + "/");

            // 実行
            var res = await agent.PostAsync(bbsCgi, content);

            // 結果受信
            var msg = await res.Content.ReadAsStringAsync();

            // cookie読み取り
            var cookies = cookie.GetCookies(new Uri(bbsCgi));

            if (cookies["HAP"] != null)
            {
                this.Session.HAP = cookies["HAP"].Value;
            }
            if (cookies["PON"] != null)
            {
                this.Session.PON = cookies["PON"].Value;
            }

            // 結果解析
            var result = new MonaAgentPostResult(thread.Board, this.Session, msg);

            // セッション保存
            this.Session.Save();

            return result;
        }

        // ----------------------------------------------------------------

        public async Task<string> GetDat(MonaThread thread, bool forceUpdateCache = false)
        {
            var encoding = Encoding.GetEncoding("shift_jis");
            var full = "";
            var size = 0L;
            var path = this.GetDatCachePath(thread);

            // --------
            // キャッシュ取得
            if (!forceUpdateCache)
            {
                if (File.Exists(path))
                {
                    size = new FileInfo(path).Length;
                    full = FileUtility.ReadToEnd(path, encoding);
                }
            }

            // --------
            // 差分取得
            var url = new Uri(thread.Board.Server + "/" + thread.Board.Id + "/dat/" + thread.Id + ".dat");
            var dif0 = "";
            var dif1 = "";
            using (var agent = new HttpAgent())
            {
                if (1 <= size)
                { 
                    var range = size - 1;
                    agent.DefaultRequestHeaders.Range = new System.Net.Http.Headers.RangeHeaderValue(range, null);
                }

                bool aborn = false;
                try
                {
                    dif0 = await agent.GetStringAsync(url, encoding);
                    if (1 <= size)
                    {
                        if (dif0[0] != '\n')
                        {// あぼーん検出
                            aborn = true;
                        }
                    }
                }
                catch (HttpRequestException e)
                {
                    var we = e.InnerException as WebException;
                    if (we != null && (int)we.Status == 416)
                    {// あぼーん検出2
                        aborn = true;
                    }
                    else
                    {
                        throw;
                    }
                }

                if (aborn)
                {
                    if (forceUpdateCache)
                    {
                        throw new Exception();
                    }
                    return await this.GetDat(thread, forceUpdateCache: true);
                }

                if (1 <= size)
                {
                    dif1 = dif0.Substring(1);
                }
                else
                {
                    dif1 = dif0;
                }

                if (dif1 != "")
                {
                    full = full + dif1;
                    FileUtility.Write(path, dif1, append: true, encoding: encoding);
                }
            }

            return full;
        }

        public void GetReponses(string server, string boardId, string threadId)
        {
            throw new NotImplementedException();
        }

        // ----------------------------------------------------------------
        // セッション

        private MonaAgentSession _session;

        public MonaAgentSession Session
        {
            get 
            {
                if (_session == null)
                {
                    _session = MonaAgentSession.Create(this); 
                }
                return _session; 
            }
            set 
            { 
                _session = value; 
            }
        }

        public void SaveSession()
        {
            throw new NotImplementedException();
        }

        public void LoadSession()
        {
            throw new NotImplementedException();
        }

        // ----------------------------------------------------------------
        // ディレクトリ

        private string _dataDirectory;

        public string DataDirectory 
        { 
            get 
            {
                if (string.IsNullOrEmpty(_dataDirectory))
                {
                    _dataDirectory = PathUtility.GetDataPath();
                }
                return _dataDirectory;
            }
            set { _dataDirectory = value; }
        }

        public string GetSessionDirectory()
        { 
            var dir = this.DataDirectory + Path.DirectorySeparatorChar + "session"; 
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }

        public string GetCacheDirectory()
        { 
            var dir = this.DataDirectory + Path.DirectorySeparatorChar + "cache"; 
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }

        public string GetBbsMenuCachePath()
        {
            return this.GetCacheDirectory()
                + Path.DirectorySeparatorChar + "bbsmenu.html";
        }

        public string GetSubjectCachePath(MonaBoard board)
        {
            return this.GetCacheDirectory()
                + Path.DirectorySeparatorChar + board.Id
                + Path.DirectorySeparatorChar + "subject.txt";
        }

        public string GetDatCachePath(MonaThread thread)
        {
            return this.GetCacheDirectory()
                + Path.DirectorySeparatorChar + thread.Board.Id
                + Path.DirectorySeparatorChar + thread.CreateTime.Year .ToString("D4")
                + Path.DirectorySeparatorChar + thread.CreateTime.Month.ToString("D2")
                + Path.DirectorySeparatorChar + thread.CreateTime.Day  .ToString("D2")
                + Path.DirectorySeparatorChar + thread.Id + ".dat"
                ;
        }
    }
}

