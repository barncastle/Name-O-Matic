using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using CASCLib;

namespace NameOMatic.Database.Lookups
{
    class TactKeyLookup : IReadOnlyDictionary<ulong, byte[]>
    {
        private readonly string Name = Path.Combine("Lookups", "TactKeyLookup.txt");
        private readonly Dictionary<ulong, byte[]> Records;

        public TactKeyLookup()
        {
            Records = new Dictionary<ulong, byte[]>();
            Populate();
        }

        public void Populate()
        {
            if (!File.Exists(Name))
                throw new FileNotFoundException($"{Name} is missing");

            using var sr = new StreamReader(Name);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] parts = line.Split(';', 2);

                if (ulong.TryParse(parts[0], NumberStyles.HexNumber, null, out var key))
                    Records[key] = parts[1].ToByteArray();
            }
        }


        public byte[] this[ulong key] => Records[key];

        public IEnumerable<ulong> Keys => Records.Keys;

        public IEnumerable<byte[]> Values => Records.Values;

        public int Count => Records.Count;

        public bool ContainsKey(ulong key) => Records.ContainsKey(key);

        public IEnumerator<KeyValuePair<ulong, byte[]>> GetEnumerator() => Records.GetEnumerator();

        public bool TryGetValue(ulong key, [MaybeNullWhen(false)] out byte[] value) => Records.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => Records.GetEnumerator();
    }
}
