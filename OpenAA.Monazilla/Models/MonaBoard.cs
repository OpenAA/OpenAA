namespace OpenAA.Monazilla.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 板
    /// </summary>
    public class MonaBoard 
    {
        /// <summary>
        /// カテゴリ名
        /// bbsmenu.htmlに含まれる日本語表記
        /// </summary>
        public MonaCategory Category { get; set; }

        /// <summary>
        /// サーバーのURL
        /// </summary>
        public Uri Server { get; set; }

        /// <summary>
        /// 板ID
        /// URLに含まれるアルファベット表記
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 板名
        /// bbsmenu.htmlに登場する日本語表記
        /// </summary>
        public string Name { get; set; }

        public IList<MonaThread> Threads { get; set; }

        public MonaBoard()
        {
            this.Threads = new List<MonaThread>();
        }

        public override string ToString()
        {
            return string.Format("[MonaBoard: Category={0}, Server={1}, Id={2}, Name={3}, Threads={4}]", Category, Server, Id, Name, Threads);
        }
    }
}

