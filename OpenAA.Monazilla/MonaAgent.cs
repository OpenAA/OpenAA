namespace OpenAA.Monazilla
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Linq;
    using System.IO;
    using NLog;

    using OpenAA.Net;
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

        private string GetBbsMenuCachePath()
        {
            var path = PathUtility.GetDataPath() + Path.DirectorySeparatorChar + "bbsmenu.html";
            return path;
        }

        private IList<MonaCategory> ParseBbsMenu(string bbsMenuHtml)
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

        private IList<MonaThread> ParseSubject(MonaBoard board, string subject)
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
    }
}

