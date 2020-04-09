using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NameOMatic.Constants;
using NameOMatic.Database;

namespace NameOMatic.Helpers.WoWTools
{
    class DiffEnumerator : IEnumerable<int>
    {
        public string FileType { get; set; }

        private readonly string PrevRootEKey;
        private readonly Dictionary<string, IEnumerable<JToken>> DataByType;

        public DiffEnumerator(string previousRootEKey)
        {
            PrevRootEKey = previousRootEKey;
            DataByType = Request().GroupBy(x => x.Value<string>("type")).ToDictionary(x => x.Key, x => x.AsEnumerable());
        }

        public IEnumerator<int> GetEnumerator()
        {
            if (DataByType.TryGetValue(FileType, out var records))
                foreach (var record in records)
                    if (record.Value<string>("action") != "removed")
                        yield return record.Value<int>("id");
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private JArray Request()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(PrevRootEKey))
                {
                    Console.WriteLine("Requesting Build Diff data...");

                    using (var client = new WebClientEx())
                    {
                        var url = string.Format(Endpoints.DiffAPI, PrevRootEKey, FileContext.Instance.RootEKey, DateTime.Now.Ticks).ToLowerInvariant();
                        var response = client.DownloadString(url);
                        var data = JToken.Parse(response)["data"];

                        return data.Value<JArray>();
                    }
                }
            }
            catch { }

            return new JArray();
        }

    }
}
