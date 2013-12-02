namespace OpenAA.Security.Cryptography
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Security.Cryptography;

    public static class HashUtility
    {
        /// <summary>
        /// 指定されたハッシュアルゴリズムでハッシュ化された16進数文字列を取得します。
        /// </summary>
        /// <param name="hasher"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetHash(HashAlgorithm hasher, object source)
        {
            if (source == null)
            {
                source = "";
            }

            byte[] byteValue = Encoding.UTF8.GetBytes(source.ToString());
            return GetHash(hasher, byteValue);
        }

        public static string GetHash(HashAlgorithm hasher, byte[] byteValue)
        {
            if (hasher == null)
            {
                throw new ArgumentNullException("hasher");
            }

            // ハッシュ算出
            byte[] hashValue = hasher.ComputeHash(byteValue);

            // バイト配列を文字列に変換。
            string result = BitConverter.ToString(hashValue).ToLower();
            result = result.Replace("-", "");

            return result;
        }

        public static string GetHashHmac(HMAC hasher, object source, object mackey)
        {
            if (source == null)
            {
                source = "";
            }
            if (mackey == null)
            {
                mackey = "";
            }

            byte[] sourceValue = Encoding.UTF8.GetBytes(source.ToString());
            byte[] mackeyValue = Encoding.UTF8.GetBytes(mackey.ToString());
            return GetHashHmac(hasher, sourceValue, mackeyValue);
        }

        public static string GetHashHmac(HMAC hasher, byte[] byteValue, byte[] keyValue)
        {
            if (hasher == null)
            {
                throw new ArgumentNullException("hasher");
            }

            // キー設定
            hasher.Key = keyValue;
            // ハッシュ算出
            byte[] hashValue = hasher.ComputeHash(byteValue);

            // バイト配列を文字列に変換。
            string result = BitConverter.ToString(hashValue).ToLower();
            result = result.Replace("-", "");

            return result;
        }
    }
}
