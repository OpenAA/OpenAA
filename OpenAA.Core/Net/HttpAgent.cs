namespace OpenAA.Net
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class HttpAgent : System.Net.Http.HttpClient
    {

        public HttpAgent()
            : this (new HttpClientHandler(), true)
        {
        }

        public HttpAgent(HttpMessageHandler handler)
            : this (handler, true)
        {
        }

        public HttpAgent(HttpMessageHandler handler, bool disposeHandler)
            : base (handler, disposeHandler)
        {
            // cookie用
            this.HttpClientHandler = handler as HttpClientHandler;

            // User-Agent
            this.DefaultRequestHeaders.Add("User-Agent", @"Opera/9.80 (Windows NT 6.1) Presto/2.12.388 Version/12.16");
            // Accpet
            this.DefaultRequestHeaders.Accept.Clear();
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html", 0.9));
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/png" , 0.1));
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/webp", 0.1));
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/jpeg", 0.1));
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/gif" , 0.1));
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/x-xbitmap", 0.1));
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.1));
        }

        public HttpClientHandler HttpClientHandler { get; private set; }

        public Task<string> GetStringWithAutoDetectEncodingAsync (string requestUri)
        {
            return GetStringWithAutoDetectEncodingAsync(new Uri(requestUri));
        }

        public async Task<string> GetStringWithAutoDetectEncodingAsync (Uri requestUri)
        {
            var buffer = await this.GetByteArrayAsync(requestUri);

            var detector = new OpenAA.Text.EncodingDetector();
            var encoding = detector.GetCode(buffer);
            var result = encoding.GetString(buffer);
            return result;
        }
    }
}

