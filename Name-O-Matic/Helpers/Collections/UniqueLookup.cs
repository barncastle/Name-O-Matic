using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NameOMatic.Helpers.Collections
{
    class UniqueLookup<TKey, TValue> : ILookup<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, IEnumerable<TValue>> _dictionary;
        private readonly IEqualityComparer<TValue> _valueComparer;

        public UniqueLookup(IEqualityComparer<TKey> keycomparer = null, IEqualityComparer<TValue> valuecomparer = null)
        {
            _dictionary = new ConcurrentDictionary<TKey, IEnumerable<TValue>>(keycomparer);
            _valueComparer = valuecomparer;
        }


        public int Count => _dictionary.Count(x => x.Value.Count() == 1);

        public void Add(TKey key, TValue value)
        {
            _dictionary.AddOrUpdate(key, (k) => AddMethod(value), (k, s) => UpdateMethod(s, value));
        }

        public void Replace(TKey key, TValue value)
        {
            _dictionary[key] = AddMethod(value);
        }

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            if (items != null)
                foreach (var item in items)
                    Add(item.Key, item.Value);
        }

        public void ReplaceRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            if (items != null)
                foreach (var item in items)
                    Replace(item.Key, item.Value);
        }

        public void Merge(UniqueLookup<TKey, TValue> other)
        {
            var items = other?.SelectMany(x => x.Select(v => new KeyValuePair<TKey, TValue>(x.Key, v)));
            AddRange(items);
        }


        public bool Contains(TKey key) => _dictionary.ContainsKey(key);

        public IEnumerable<TValue> this[TKey key] => _dictionary[key];


        public void TrimExcess()
        {
            var conflicts = _dictionary.Where(x => x.Value.Count() != 1).Select(x => x.Key).ToArray();

            foreach (var key in conflicts)
                _dictionary.TryRemove(key, out _);
        }

        public IDictionary<TKey, TValue> ToDictionary() => this.ToDictionary(x => x.Key, x => x.First());

        public void Export(string directory, string filename, Func<TValue, string> valueFormatter = null)
        {
            if (!this.Any())
                return;

            Directory.CreateDirectory(directory);

            using var fs = File.CreateText(Path.Combine("Output", filename));
            foreach (var entry in this.OrderBy(x => x.Key))
            {
                var value = valueFormatter != null ? valueFormatter.Invoke(entry.First()) : entry.First().ToString();
                fs.WriteLine(entry.Key + ";" + value);
            }                
        }


        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        IEnumerator<IGrouping<TKey, TValue>> IEnumerable<IGrouping<TKey, TValue>>.GetEnumerator() => GetEnumeratorInternal();

        IEnumerator<IGrouping<TKey, TValue>> GetEnumeratorInternal()
        {
            return _dictionary.Where(x => x.Value.Count() == 1).Select(x => new Grouping(x)).GetEnumerator();
        }


        private IEnumerable<TValue> AddMethod(TValue value)
        {
            return new HashSet<TValue>(_valueComparer) { value };
        }

        public IEnumerable<TValue> UpdateMethod(IEnumerable<TValue> set, TValue value)
        {
            ((ISet<TValue>)set).Add(value);
            return set;
        }


        private class Grouping : IGrouping<TKey, TValue>
        {
            private readonly TKey _key;
            private readonly IEnumerable<TValue> _values;

            public Grouping(KeyValuePair<TKey, IEnumerable<TValue>> kvp) => (_key, _values) = (kvp.Key, kvp.Value);

            TKey IGrouping<TKey, TValue>.Key => _key;

            IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => _values.GetEnumerator();
        }
    }
}
