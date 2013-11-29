namespace OpenAA.Extensions.Char
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// System.Charの拡張クラス。
    /// </summary>
    public static class CharExtensions
    {
        public const string WHITESPACE = " \t\r\n\f";

        /// <summary>
        /// 文字が数字文字かを調べる
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static bool IsNumberCharacter(this char character)
        {
            if ( 48 <= character && character <= 57 )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 文字が英字かを調べる
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static bool IsAlphabetCharacter(this char character)
        {
            if (65 <= character && character <= 90)
            {
                return true;
            }
            if (97 <= character && character <= 122)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 文字がアスキー文字かを調べる。
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static bool IsAsciiCharacter(this char character)
        {
            if (0 <= character && character <= 127)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 文字が空白文字かを調べる。
        /// </summary>
        /// <param name="character"></param>
        /// <param name="extCharList"></param>
        /// <returns></returns>
        public static bool IsWhitespace(this char character, string extCharList = WHITESPACE)
        {
            string charList;
            if (extCharList != null && extCharList != "")
            {
                charList = extCharList;
            }
            else 
            {
                charList = WHITESPACE;
            }

            int len = charList.Length;
            for (int i = 0; i < len; i++)
            {
                if (charList[i] == character)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// スペース文字かを調べる。
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static bool IsSpace(this char character)
        {
            if (character == ' ')
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

