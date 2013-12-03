namespace OpenAA.Extensions.String
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Globalization;

    using OpenAA.Extensions.Char;

    /// <summary>
    /// System.Stringの拡張クラス
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 文字列から前後のキーワードを指定して間の文字列を取得します。
		/// キーワードが空文字列の場合は、先頭または後尾を検索位置とします。
		/// キーワードが見つからなかった場合は、 null を取得します。
        /// </summary>
        /// 
        /// <param name="source">検索対象の文字列</param>
        /// <param name="prevKeyword">前方キーワード</param>
        /// <param name="backKeyword">後方キーワード</param>
        /// 
        /// <returns>取得した文字列</returns>
        public static string InnerText(this string source, string prevKeyword, string backKeyword)
        {
            if (source == null)
            {
                return null;
            }

            // 前方
            int prevIndex;

            if (string.IsNullOrEmpty(prevKeyword))
            {
                prevIndex = 0;
            }
            else
            {
                prevIndex = source.IndexOf(prevKeyword);
                if (prevIndex < 0)
                {
                    return null;
                }
                prevIndex += prevKeyword.Length;
            }

            // 後方
            int backIndex;
            if (string.IsNullOrEmpty(backKeyword))
            {
                backIndex = source.Length;
            }
            else
            {
                backIndex = source.IndexOf(backKeyword, prevIndex);
                if (backIndex < 0)
                {
                    return null;
                }
            }

            // prevIndexからbackIndexの範囲内も文字列を抽出
            int length = backIndex - prevIndex;
            return source.Substring(prevIndex, length);
        }

        /// <summary>
        /// 文字列を反転する。
        /// </summary>
        /// <param name="source">文字列</param>
        /// <returns>反転した文字列</returns>
        public static string Reverse(this string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }
            if (source == "")
            {
                return "";
            }

            int len = source.Length;
            var buf = new char[len];
            int p0, p1;

            for (p0 = 0, p1 = len - 1; p0 <= p1; p0++, p1--)
            {
                buf[p0] = source[p1];
                buf[p1] = source[p0];
            }

            return new string(buf);
        }

        /// <summary>
        /// 文字列中からkeywordに一致する文字列を除去する。
        /// </summary>
        /// <param name="source">文字列</param>
        /// <param name="keyword">削除する文字列</param>
        /// <returns></returns>
        public static string Remove(this string source, string keyword)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }
            if (keyword == null)
            {
                throw new ArgumentNullException();
            }
            return source.Replace(keyword, "");
        }

        /// <summary>
        /// valueで指定された文字列のうち、delimiterで指定された文字の前後の空白文字を削除します。
        /// たとえばCSVであれば各カンマの前後のスペースを除去することになります。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string TrimArrayElements(this string value, string delimiter)
        {
            if (value == null || value == "")
            {
                return value;
            }

            string[] items = null;
            string ret;
            int len;

            try
            {
                items = value.Split(new string[]{value}, StringSplitOptions.None);
                len = items.Length;
                if (0 < len)
                {
                    for (int i = 0; i < len; i++)
                    {
                        items[i] = items[i].Trim();
                    }
                    ret = string.Join(delimiter, items);
                }
                else
                {
                    ret = value.Clone() as string;
                }
            }
            finally
            {
                items = null;
            }

            return ret;
        }

        /// <summary>
        /// 文字数を取得する。
        /// UTF-16サロゲートペア対応
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int LengthInTextElements(this string text)
        {
            int[] indexes = StringInfo.ParseCombiningCharacters(text);
            return indexes.Length;
        }

        /// <summary>
        /// 文字列のバイト数を取得する。
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static int ByteCount(this string text, Encoding encoding)
        {
            if (encoding == null)
            {
                encoding = Encoding.Unicode;
            }
            return encoding.GetByteCount(text);
        }

        /// <summary>
        /// 文字列がnullまたは空文字かを調べる。
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        /// <summary>
        /// 文字列がnullまたは空白文字だけで構成されているかを調べる。
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNullOrWhitespace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        /// <summary>
        /// 文字列sourceに文字列valueが含まれるかどうかを示す値を取得します。
        /// String.IndexOfメソッドの戻り値だと条件式が見づらいという人向け。
        /// 大した意味はない。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static bool IsContains(this string source, string keyword)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }
            if (keyword == null)
            {
                throw new ArgumentNullException();
            }

            if (0 <= source.IndexOf(keyword))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 改行コードが含まれているかを調べる
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsMultiLine(this string text)
        {
            if (text == null)
            {
                return false;
            }

            if (IsContains(text, "\n") ||
                IsContains(text, "\r") ||
                IsContains(text, "\r\n"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Ises the ASCII alphabet and numeric.
        /// </summary>
        /// <returns><c>true</c>, if ASCII alphabet and numeric was ised, <c>false</c> otherwise.</returns>
        /// <param name="text">Text.</param>
        public static bool IsAsciiAlphabetAndNumeric(this string text)
        {
            return text.All(c => c.IsAlphabetCharacter() || c.IsNumberCharacter());
        }

        /// <summary>
        /// 指定した回数繰り返した文字列を取得します。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Repeat(this string source, uint count)
        {
            var rep = new StringBuilder();
            int index;

            for (index = 0; index < count; index++)
            {
                rep.Append(source);
            }
            return rep.ToString();
        }

        /// <summary>
        /// テキストをインデントして取得します。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="indent"></param>
        /// <param name="isIndentHead"></param>
        /// <returns></returns>
        public static string Indent(this string source, uint indent = 4, bool isIndentHead = true)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }
            if (indent == 0)
            {
                return source;
            }

            string ret = source.Clone() as string;
            string space = " ".Repeat(indent);

            if (0 <= source.IndexOf("\r\n"))
            {
                ret = source.Replace("\r\n", "\r\n" + space);
            }
            else if (0 <= source.IndexOf("\r"))
            {
                ret = source.Replace("\r", "\r" + space);
            }
            else if (0 <= source.IndexOf("\n"))
            {
                ret = source.Replace("\n", "\n" + space);
            }

            if (isIndentHead)
            {
                ret = space + ret;
            }

            return ret;
        }

        /// <summary>
        /// テキストを行単位に分割する。
        /// </summary>
        /// <returns>The to lines.</returns>
        /// <param name="source">Source.</param>
        /// <param name="removeEmptyLine">If set to <c>true</c> remove empty line.</param>
        public static string[] SplitToLines(this string source, bool removeEmptyLine = false)
        {
            var opt = removeEmptyLine 
                ? StringSplitOptions.RemoveEmptyEntries
                : StringSplitOptions.None;

            return source
                    .Replace("\r\n", "\n")
                    .Replace("\r"  , "\n")
                    .Split(new[]{'\n'}, opt);
        }
    }
}
