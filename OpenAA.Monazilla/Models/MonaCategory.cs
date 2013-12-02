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
        public string Name;

        /// <summary>
        /// カテゴリに含まれる板一覧
        /// </summary>
        public IList<MonaBoard> Boards = new List<MonaBoard>();

        public override string ToString()
        {
            var ret = "Name: " + Name + " {" + Environment.NewLine;
            foreach (var board in Boards)
            {
                ret += "  " + board.ToString() + Environment.NewLine;
            }
            ret += "}";
            return ret;
        }
    }
}

