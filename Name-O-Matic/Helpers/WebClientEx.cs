using System;
using System.Net;

namespace NameOMatic.Helpers
{
    class WebClientEx : WebClient
    {
        public DecompressionMethods AutomaticDecompression { get; set; }
        public string Method { get; set; }


        public WebClientEx() { }

        public WebClientEx(DecompressionMethods decompression) => AutomaticDecompression = decompression;


        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            if (!string.IsNullOrEmpty(Method))
                request.Method = Method;
            request.AutomaticDecompression = AutomaticDecompression;
            return request;
        }
    }
}
