using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace NameOMatic.Database.Lookups
{
    class CreatureNameLookup : IDictionary<int, string>
    {
        private readonly string Name = Path.Combine("Lookups", "CreatureNameLookup.csv");
        private readonly Dictionary<int, string> Records;

        public CreatureNameLookup()
        {
            Records = new Dictionary<int, string>();
            Populate();
        }

        public void Populate()
        {
            if (!File.Exists(Name))
                return;

            using (var sr = new StreamReader(Name))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(',', 3);
                    if (int.TryParse(parts[0], out int id))
                        Records[id] = parts[2];
                }
            }
        }

        public string this[int key]
        {
            get => Records[key];
            set => Records[key] = value;
        }

        public IEnumerable<int> Keys => ((IReadOnlyDictionary<int, string>)Records).Keys;

        public IEnumerable<string> Values => ((IReadOnlyDictionary<int, string>)Records).Values;

        public int Count => Records.Count;

        ICollection<int> IDictionary<int, string>.Keys => ((IDictionary<int, string>)Records).Keys;

        ICollection<string> IDictionary<int, string>.Values => ((IDictionary<int, string>)Records).Values;

        public bool IsReadOnly => ((IDictionary<int, string>)Records).IsReadOnly;

        string IDictionary<int, string>.this[int key] { get => Records[key]; set => Records[key] = value; }

        public bool ContainsKey(int key) => Records.ContainsKey(key);

        public IEnumerator<KeyValuePair<int, string>> GetEnumerator() => Records.GetEnumerator();

        public bool TryGetValue(int key, [MaybeNullWhen(false)] out string value) => Records.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => Records.GetEnumerator();

        public void Add(int key, string value) => Records.Add(key, value);

        public bool Remove(int key) => Records.Remove(key);

        public void Clear() => Records.Clear();

        public void Add(KeyValuePair<int, string> item) => throw new NotImplementedException();

        public bool Contains(KeyValuePair<int, string> item) => throw new NotImplementedException();

        public void CopyTo(KeyValuePair<int, string>[] array, int arrayIndex) => throw new NotImplementedException();

        public bool Remove(KeyValuePair<int, string> item) => throw new NotImplementedException();
    }
}
