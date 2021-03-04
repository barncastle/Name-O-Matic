using System;
using System.Net;

namespace NameOMatic.Helpers
{
    internal class WebClientEx : WebClient
    {
        public DecompressionMethods AutomaticDecompression { get; set; }
        public string Method { get; set; }

        public WebClientEx()
        {
        }

        public WebClientEx(DecompressionMethods decompression) => AutomaticDecompression = decompression;

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            if (!string.IsNullOrEmpty(Method))
                request.Method = Method;

            request.UserAgent = "Name-O-Matic/1.0 LOVE_DEATH_AND_MARTIN_BENJAMINS";

            request.AutomaticDecompression = AutomaticDecompression;
            return request;
        }
    }
}
