namespace OpenAA.Extensions.Uri
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;

    /// <summary>
    /// System.Uriの拡張クラス
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// QueryStringからパラメーター部分を分割して取得する。
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Dictionary<string,string> GetQueryStringParameters(this Uri uri)
        {
            // 戻り値格納用
            var ret = new Dictionary<string,string>();

            // nullチェック
            if (uri == null)
            {
                return ret;
            }

            // クエリー抽出
            string query = uri.Query;

            // アンパサンドで分割
            string[] kvp = query.Split('&');
            foreach (string pair in kvp)
            {
                // イコールで分割
                var vars = pair.Split(new char[]{'='}, 2);

                string key = "";
                string val = "";

                if (vars.Count() == 1)
                {
                    key = HttpUtility.UrlDecode(vars[0]);
                    val = "";
                }
                else if (vars.Count() == 2)
                {
                    key = HttpUtility.UrlDecode(vars[0]);
                    val = HttpUtility.UrlDecode(vars[1]);
                }

                if (!ret.ContainsKey(key))
                {
                    ret.Add(key, val);
                }
            }
            return ret;
        }
    }
}

