namespace OpenAA.Monazilla.Models
{
    using System;
    using System.Collections.Generic;
    using OpenAA.Extensions.String;

    /// <summary>
    /// 板カテゴリ
    /// </summary>
    public class MonaCategory
    {
        /// <summary>
        /// カテゴリ名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// カテゴリに含まれる板一覧
        /// </summary>
        public IList<MonaBoard> Boards { get; set; }

        public MonaCategory()
        {
            this.Boards = new List<MonaBoard>();
        }

        public override string ToString()
        {
            return string.Format("[MonaCategory: Name={0}, Boards={1}]", Name, Boards);
        }
    }
}

