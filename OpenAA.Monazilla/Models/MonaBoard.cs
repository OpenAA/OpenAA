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
        public MonaCategory Category;

        /// <summary>
        /// サーバーのURL
        /// </summary>
        public Uri Server;

        /// <summary>
        /// 板ID
        /// URLに含まれるアルファベット表記
        /// </summary>
        public string Id;

        /// <summary>
        /// 板名
        /// bbsmenu.htmlに登場する日本語表記
        /// </summary>
        public string Name;

        public override string ToString()
        {
            return
                "Name: "   + Name + ", " +
                "Id: "     + Id + ", " +
                "Server: " + Server;
        }
    }
}

