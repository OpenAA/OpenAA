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
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static readonly Regex RegexBbsMenuCategory = new Regex(
            @"<BR><BR><B>(?<cate>.+?)<\/B><BR>",
            RegexOptions.IgnoreCase | 
            RegexOptions.Singleline | 
            RegexOptions.Compiled);

        private static readonly Regex RegexBbsMenuBoard = new Regex(
            @"<A HREF=(?<serv>http:\/\/.*(\.2ch\.net|\.bbspink\.com|\.machi\.to))\/(?<code>[^\s]*).*>(?<name>.+)<\/A>",
            RegexOptions.IgnoreCase | 
            RegexOptions.Singleline | 
            RegexOptions.Compiled);

        private static readonly Regex RegexSubject = new Regex(
            @"(?<id>[0-9]*).dat\<\>(?<title>.*?)\((?<nums>[0-9]*)\)", 
            RegexOptions.IgnoreCase | 
            RegexOptions.Singleline | 
            RegexOptions.Compiled);


        private static readonly List<string> IgnoreCategories = new List<string>{ "特別企画", "チャット", "ツール類" };

        private static readonly List<string> IgnoreBoards     = new List<string>{ "2chプロジェクト", "いろいろランク" };

        public async Task<IList<MonaCategory>> GetCategories()
        {
            var html = "";
            html = await GetBbsMenuFromCache();
            if (string.IsNullOrEmpty(html))
            {
                html = await GetBbsMenuFromOnline();
            }
            var categories = ParseBbsMenu(html);
            return categories;
        }

        public async Task<string> GetBbsMenuFromOnline(string bbsMenuUri = "http://menu.2ch.net/bbsmenu.html", bool doNotSaveToCache = false)
        {
            var agent = new HttpAgent();
            var html = await agent.GetStringWithAutoDetectEncodingAsync(bbsMenuUri);

            if (!doNotSaveToCache)
            {
                var path = GetBbsMenuCachePath();
                Console.WriteLine(path);
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                using (var sw = new StreamWriter(path))
                {
                    await sw.WriteAsync(html);
                }
            }

            return html;
        }

        public async Task<string> GetBbsMenuFromCache()
        {
            var html = "";
            var path = GetBbsMenuCachePath();
            if (File.Exists(path))
            {
                using (var sr = new StreamReader(path))
                {
                    html = await sr.ReadToEndAsync();
                }
            }
            return html;
        }

        public string GetBbsMenuCachePath()
        {
            var path = PathUtility.GetDataPath() + Path.DirectorySeparatorChar + "bbsmenu.html";
            return path;
        }

        public IList<MonaCategory> ParseBbsMenu(string bbsMenuHtml)
        {
            var categories = new List<MonaCategory>();

            var category = new MonaCategory();
            string serv = "";
            string code = "";
            string name = "";

            foreach (var line in bbsMenuHtml.SplitToLines())
            {
                // カテゴリー行か？
                var match = RegexBbsMenuCategory.Match(line);
                if (match.Success)
                {
                    var cate = match.Groups["cate"].Value;
                    if (IgnoreCategories.Contains(cate))
                    {
                        category = null;
                    }
                    else
                    {
                        category = new MonaCategory
                        {
                            Name = cate
                        };
                        categories.Add(category);
                    }
                    _logger.Trace("cate=" + cate);
                    continue;
                }
                // カテゴリーが選択されているか？
                if (category == null)
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
                        Category = category,
                        Server = new Uri(serv),
                        Id = code,
                        Name = name
                    };
                    _logger.Trace(board);

                    category.Boards.Add(board);
                }
            }

            return categories;
        }

        // ------------------------------------------------------------

        public async Task<IList<MonaThread>> GetSubject(MonaBoard board)
        {
            var url = new Uri(board.Server + "/" + board.Id + "/subject.txt");
            _logger.Trace(url);

            var subject = "";
            using (var agent = new HttpAgent())
            {
                subject = await agent.GetStringWithAutoDetectEncodingAsync(url);
            }

            var threads = ParseSubject(board, subject);
            return threads;
        }

        public IList<MonaThread> ParseSubject(MonaBoard board, string subject)
        {
            var threads = new List<MonaThread>();
            var now = DateTime.Now;
            int cnt = 1;

            foreach (var line in subject.SplitToLines())
            {
                _logger.Trace(line);

                var match = RegexSubject.Match(line);
                if (!match.Success)
                {
                    _logger.Debug("unmatch: " + line);
                    continue;
                }

                var thread = new MonaThread
                {
                    Board = board,
                    No    = cnt++,
                    Id    = match.Groups["id"].Value,
                    Title = match.Groups["title"].Value,
                    Nums  = Int32Utility.ParseOrDefault(match.Groups["nums"].Value),
                    UpdateTime = now,
                };
                threads.Add(thread);
            }

            return threads;
        }

        // ------------------------------------------------------------

        private MonaAgentSession Session { get; set; }

        public async Task CreateResponse(MonaThread thread, string name, string mail, string message)
        {
            for (int i = 0; i < 2; i++)
            {
                await CreateResponseCore(thread, name, mail, message);
                Console.WriteLine(this.Session);

                var wait = (int)(this.Session.Wait - DateTime.Now).TotalSeconds;
                if (0 < wait)
                {
                    Console.WriteLine("wait=" + wait);
                    Console.Out.Flush();
                    await Task.Delay(wait * 1000);
                }
            }
        }

        public async Task CreateResponseCore(MonaThread thread, string name, string mail, string message)
        {
            var domain = thread.Board.Server.Host;
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
            if (this.Session == null)
            {
                this.Session = new MonaAgentSession();
            }
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
            AnalyzeCreateResponseResult(msg);
        }

        public void AnalyzeCreateResponseResult(string message)
        {
            var now = DateTime.Now;
            this.Session.Wait = now;

            if (message.Contains("書きこみました"))
            {
                //this.ResultType = ResultTypes.SUCCEED;
                return;
            }
            if (message.Contains("書きこみ＆クッキー確認"))
            {
                //this.ResultType = ResultTypes.CONTINUE;
                this.Session.Wait = now.AddSeconds(2);
                return;
            }
            if (message.Contains("たたないと書けません"))
            {
//                Session.SambaCount++;
//                if (3 <= Session.SambaCount)
//                {
//                    //this.ResultType = ResultTypes.FAILED;
//                    return;
//                }
//                else
//                {
                    // this.ResultType = ResultTypes.CONTINUE;
                    this.Session.Wait.AddSeconds(120 + 10);
                    return;
//                }
            }

            if (message.Contains("公開ＰＲＯＸＹからの投稿は受け付けていません"))
            {
                //this.ResultType = ResultTypes.FAILED;
                return;
            }
            if (message.Contains("バーボン"))
            {
                //this.ResultType = ResultTypes.FAILED;
                return;
            }

            if (message.Contains("このスレッドには書き込めません"))
            {
                //this.ResultType = ResultTypes.STOPED;
                return;
            }
            if (message.Contains("このスレッドは512kを超えているので書けません"))
            {
                //this.ResultType = ResultTypes.STOPED;
                return;
            }

            if (message.Contains("貴方の忍法帖を作成します"))
            {
                //this.ResultType = ResultTypes.CONTINUE;
                //this.Wait = 120;
                this.Session.Wait.AddSeconds(120);
                return;
            }
            if (message.Contains("修行が足りません"))
            {
//                Session.NinjaCount++;
//                if (3 <= Session.NinjaCount)
//                {
//                    this.ResultType = ResultTypes.FAILED;
//                    return;
//                }
//                else
//                {
//                    //this.ResultType = ResultTypes.CONTINUE;
                    this.Session.Wait.AddSeconds(120);
                    return;
//                }
            }
            if (message.Contains("忍法帖発行から"))
            {
                //this.ResultType = ResultTypes.CONTINUE;
                this.Session.Wait.AddSeconds(62);
                return;
            }
            if (message.Contains("やられたでござる"))
            {
                //this.ResultType = ResultTypes.KILLED;
                this.Session.Wait.AddSeconds(30);
                return;
            }

            // unknown error
            //this.ResultType = ResultTypes.FAILED;
        }
    }
}

