using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using NameOMatic.Constants;
using NameOMatic.Helpers;

namespace NameOMatic.Database
{
    class ListFile : Singleton<ListFile>, IDictionary<int, string>
    {
        public const string Filename = "ListFile.csv";

        public Dictionary<string, int> RecordsReverse { get; }

        private readonly Dictionary<int, string> Records;

        public ListFile()
        {
            // block out 0
            Records = new Dictionary<int, string> { [0] = "" };
            RecordsReverse = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        public void Download()
        {
            Stopwatch sw = Stopwatch.StartNew();
            Console.Write("Downloading ListFile... ");

            using (var client = new WebClientEx(DecompressionMethods.Deflate | DecompressionMethods.GZip))
                client.DownloadFile(Endpoints.ListFile, Filename);

            sw.Stop();
            Console.WriteLine("completed in " + sw.Elapsed);
        }

        public void Load()
        {
            Download();

            Stopwatch sw = Stopwatch.StartNew();

            using (var sr = new StreamReader(Filename))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(';', 2);

                    if (int.TryParse(parts[0], out int fid))
                    {
                        Records[fid] = parts[1];
                        RecordsReverse[parts[1]] = fid;
                    }
                }
            }

            sw.Stop();
            Console.WriteLine("ListFile loaded in " + sw.Elapsed);
        }

        public void Dispose() => File.Delete(Filename);


        public ICollection<int> Keys => Records.Keys;

        public ICollection<string> Values => Records.Values;

        public int Count => Records.Count;

        public bool IsReadOnly => false;

        public string this[int key] { get => Records[key]; set => Records[key] = value; }


        public void Add(int key, string value) => Records.Add(key, value);

        public bool ContainsKey(int key) => Records.ContainsKey(key);

        public bool Remove(int key) => Records.Remove(key);

        public bool TryGetValue(int key, out string value) => Records.TryGetValue(key, out value);

        public void Add(KeyValuePair<int, string> item) => Records.Add(item.Key, item.Value);

        public void Clear() => Records.Clear();

        public bool Contains(KeyValuePair<int, string> item) => throw new NotImplementedException();

        public void CopyTo(KeyValuePair<int, string>[] array, int arrayIndex) => throw new NotImplementedException();

        public bool Remove(KeyValuePair<int, string> item) => Records.Remove(item.Key);

        public IEnumerator<KeyValuePair<int, string>> GetEnumerator() => Records.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Records.GetEnumerator();
    }
}
