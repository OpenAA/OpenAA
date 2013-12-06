namespace OpenAA.Monazilla.Models
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// 板
    /// </summary>
    public class MonaBoard 
    {
        /// <summary>
        /// カテゴリ名
        /// bbsmenu.htmlに含まれる日本語表記
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// サーバーのURL
        /// </summary>
        public string Server { get; set; }

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

        /// <summary>
        /// スレ一覧
        /// </summary>
        /// <value>The threads.</value>
        public IList<MonaThread> Threads { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MonaBoard()
        {
            this.Threads = new List<MonaThread>();
        }

    }
}

