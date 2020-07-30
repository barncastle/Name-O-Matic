using System;
using System.Collections;
using System.Collections.Generic;
using NameOMatic.Constants;
using Newtonsoft.Json.Linq;

namespace NameOMatic.Helpers.WoWTools
{
    internal class FileEnumerator : IEnumerable<int>, IDisposable
    {
        private readonly string Filters;

        private int Offset;
        private int? Count;
        private JArray Data;

        private readonly WebClientEx _client;

        public FileEnumerator(params string[] parameters)
        {
            _client = new WebClientEx();
            Filters = string.Join(",", parameters).ToLowerInvariant();
        }

        public IEnumerator<int> GetEnumerator()
        {
            do
            {
                Request();

                foreach (var record in Data)
                    if (!IsEncrypted(record))
                        yield return record[0].Value<int>();
            }
            while (Offset < Count);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Reset() => Offset = 0;

        private void Request()
        {
            string url = string.Format(Endpoints.FilesAPI, Offset, Filters, DateTime.Now.Ticks).ToLowerInvariant();
            string data = _client.DownloadString(url);

            var token = JToken.Parse(data);
            if (!Count.HasValue)
                Count = token["recordsFiltered"].Value<int>();

            Data = token["data"].Value<JArray>();
            Offset += Data.Count;
        }

        private bool IsEncrypted(JToken record)
        {
            if (!record[3].HasValues || !record[3][0].HasValues)
                return false;

            return record[3][0]["enc"].Value<byte?>() == 1;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
