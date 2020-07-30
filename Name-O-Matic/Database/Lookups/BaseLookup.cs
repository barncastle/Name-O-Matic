using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace NameOMatic.Database.Lookups
{
    internal abstract class BaseLookup<TKey, TValue> : IDictionary<TKey, TValue>
    {
        protected readonly string Name;

        private readonly Dictionary<TKey, TValue> Records;

        public BaseLookup(string filepath)
        {
            Name = filepath;
            Records = new Dictionary<TKey, TValue>();
        }

        public TValue this[TKey key] { get => Records[key]; set => Records[key] = value; }

        public ICollection<TKey> Keys => Records.Keys;

        public ICollection<TValue> Values => Records.Values;

        public int Count => Records.Count;

        public bool IsReadOnly { get; }

        public void Add(TKey key, TValue value) => Records.Add(key, value);

        public void Add(KeyValuePair<TKey, TValue> item) => Records.Add(item.Key, item.Value);

        public void Clear() => Records.Clear();

        public bool Contains(KeyValuePair<TKey, TValue> item) => Records.ContainsKey(item.Key);

        public bool ContainsKey(TKey key) => Records.ContainsKey(key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => throw new NotImplementedException();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Records.GetEnumerator();

        public void Populate()
        {
            if (!File.Exists(Name))
                throw new FileNotFoundException($"{Name} is missing");

            using var sr = new StreamReader(Name);
            string line;
            while ((line = sr.ReadLine()) != null)
                ParseLine(line);
        }

        protected abstract void ParseLine(string line);

        public bool Remove(TKey key) => Records.Remove(key);

        public bool Remove(KeyValuePair<TKey, TValue> item) => Records.Remove(item.Key);

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => Records.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
