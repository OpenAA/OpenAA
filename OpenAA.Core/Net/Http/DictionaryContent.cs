namespace OpenAA.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Web;

    public class DictionaryContent : ByteArrayContent
    {
        public DictionaryContent(IEnumerable<KeyValuePair<string,string>> nameValueCollection)
            : this(nameValueCollection, Encoding.UTF8)
        {
        }

        public DictionaryContent(IEnumerable<KeyValuePair<string,string>> nameValueCollection, Encoding encoding)
            : base(EncodeContent(nameValueCollection, encoding))
        {
            this.Headers.ContentType = new MediaTypeHeaderValue ("application/x-www-form-urlencoded");
        }

        private static byte[] EncodeContent(IEnumerable<KeyValuePair<string,string>> nameValueCollection, Encoding encoding)
        {
            if (nameValueCollection == null)
            {
                throw new ArgumentNullException("nameValueCollection");
            }

            var sb = new StringBuilder();
            foreach (var item in nameValueCollection)
            {
                if (0 < sb.Length)
                {
                    sb.Append('&');
                }

                if (!string.IsNullOrEmpty(item.Key))
                {
                    var key = HttpUtility.UrlEncode(item.Key, encoding);
                    sb.Append(key);
                    sb.Append('=');
                }

                if (!string.IsNullOrEmpty(item.Value))
                {
                    var val = HttpUtility.UrlEncode(item.Value, encoding);
                    sb.Append(val);
                }
            }

            return Encoding.ASCII.GetBytes(sb.ToString());
        }
    }
}

