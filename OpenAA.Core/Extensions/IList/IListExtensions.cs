namespace OpenAA.Extensions.IList
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// System.Collections.Generic.IListの拡張クラス
    /// </summary>
    public static class IListExtensions
    {
        /// <summary>
        /// リストの末尾から要素を一つ取り出す。
        /// PHPのarray_popのようなもの。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T Pop<T>(this IList<T> list)
        {
            var count = list.Count - 1;
            if (count <= 0)
            {
                return default(T);
            }
            var local = list[count];
            list.RemoveAt(count);
            return local;
        }

        /// <summary>
        /// リストの先頭から要素を一つ取り出す。
        /// PHPのarray_shiftのようなもの。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T Shift<T>(this IList<T> list)
        {
            var count = 0;
            if (list.Count <= 0)
            {
                return default(T);
            }
            var local = list[count];
            list.RemoveAt(count);
            return local;
        }

        /// <summary>
        /// リストの末尾に要素を追加する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        public static void Push<T>(this IList<T> list, T item)
        {
            list.Add(item);
        }

        /// <summary>
        /// リストの先頭に要素を追加する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        public static void Unshift<T>(this IList<T> list, T item)
        {
            list.Insert(0, item);
        }
    }
}
