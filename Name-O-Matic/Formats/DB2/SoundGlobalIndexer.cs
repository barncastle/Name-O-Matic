using System.Collections;
using System.Collections.Generic;
using NameOMatic.Database;
using NameOMatic.Helpers;

namespace NameOMatic.Formats.DB2
{
    internal class SoundGlobalIndexer : Singleton<SoundGlobalIndexer>, IReadOnlyDictionary<int, List<int>>
    {
        private readonly Dictionary<int, List<int>> GlobalIndex;

        public SoundGlobalIndexer()
        {
            GlobalIndex = new Dictionary<int, List<int>>(200000);

            var listfile = ListFile.Instance;
            var soundKitEntries = DBContext.Instance["SoundKitEntry"];

            if (soundKitEntries == null)
                return; ;

            foreach (var rec in soundKitEntries)
            {
                var soundKitID = rec.Value.FieldAs<int>("SoundKitID");
                var fileDataID = rec.Value.FieldAs<int>("FileDataID");

                // index lookup
                if (!GlobalIndex.TryGetValue(soundKitID, out var fileIds))
                {
                    fileIds = new List<int>(0x80);
                    GlobalIndex.Add(soundKitID, fileIds);
                }
                fileIds.Add(fileDataID);
            }
        }

        public List<int> this[int key] => GlobalIndex[key];

        public IEnumerable<int> Keys => GlobalIndex.Keys;

        public IEnumerable<List<int>> Values => GlobalIndex.Values;

        public int Count => GlobalIndex.Count;

        public bool ContainsKey(int key) => GlobalIndex.ContainsKey(key);

        public IEnumerator<KeyValuePair<int, List<int>>> GetEnumerator() => GlobalIndex.GetEnumerator();

        public bool TryGetValue(int key, out List<int> value) => GlobalIndex.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => GlobalIndex.GetEnumerator();
    }
}
