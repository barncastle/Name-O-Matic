using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NameOMatic.Database.Lookups
{
    class PrefixLookup : IReadOnlyDictionary<string, string>
    {
        private readonly string Name = Path.Combine("Lookups", "PrefixLookup.txt");
        private readonly Dictionary<string, string> Records;

        public int SplitSize { get; private set; }

        public PrefixLookup() => Records = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public void Populate()
        {
            if (!File.Exists(Name))
                throw new FileNotFoundException($"{Name} is missing");

            using (var sr = new StreamReader(Name))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(';', 2);

                    if (!parts[1].EndsWith('/'))
                        parts[1] += '/';

                    Records[parts[0]] = parts[1];
                }
            }

            SplitSize = Records.Keys.Max(x => x.Split('_').Length) + 1;
        }

        public string this[string key] => Records[key];

        public IEnumerable<string> Keys => Records.Keys;

        public IEnumerable<string> Values => Records.Values;

        public int Count => Records.Count;

        public bool ContainsKey(string key) => Records.ContainsKey(key);

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => Records.GetEnumerator();

        public bool TryGetValue(string key, out string value) => Records.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => Records.GetEnumerator();
    }
}
