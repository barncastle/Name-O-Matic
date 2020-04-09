using CASCLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NameOMatic.Constants;
using NameOMatic.Helpers;
using NameOMatic.Database.Lookups;

namespace NameOMatic.Database.Storage
{
    using CHashLookup = Dictionary<MD5Hash, IEnumerable<int>>;

    class CASCLibWrapper : IStorage
    {
        public string Build => Handler.Config.BuildName;
        public string FormattedBuild { get; }
        public Expansion Expansion { get; }
        public string RootEKey { get; }

        private CASCHandler Handler;
        private CHashLookup CHashLookup;

        public CASCLibWrapper(string path, string product = null)
        {
            CASCConfig.ThrowOnFileNotFound = false;
            CASCConfig.ThrowOnMissingDecryptionKey = false;

            // open the container
            Handler = CASCHandler.OpenLocalStorage(path, product);
            Handler.Root.SetFlags(LocaleFlags.enUS);
            Console.WriteLine();

            // store Root EKey
            Handler.Encoding.GetEntry(Handler.Config.RootMD5, out var root);
            RootEKey = root.Key.ToHexString();

            // store various build info
            var build = Build.Split('.');
            FormattedBuild = string.Join("", build, 0, build[2] == "0" ? 2 : 3);
            Expansion = (Expansion)byte.Parse(build[0]);

            GenerateCHashLookup();
        }

        public bool FileExists(int fileDataId) => Handler.FileExists(fileDataId);

        public Stream OpenFile(int fileDataId) => Handler.OpenFile(fileDataId) ?? new MemoryStream(0);

        public IEnumerable<int> GetMatchingFiles(int fileDataId)
        {
            var root = (WowRootHandler)Handler.Root;
            var fileentry = root.GetEntriesByFileDataId(fileDataId).FirstOrDefault();

            if (!fileentry.Equals(default(RootEntry)) && CHashLookup.TryGetValue(fileentry.MD5, out var fids))
                return fids;

            return new int[0];
        }


        public void UpdateKeyService()
        {
            DBContext.Instance.TryLoad("TactKey", out var tactKey);
            DBContext.Instance.TryLoad("TactKeyLookup", out var tactKeyLookup);

            if (tactKey != null && tactKeyLookup != null)
            {
                var commonIDs = tactKey.Keys.Intersect(tactKeyLookup.Keys);
                foreach (var id in commonIDs)
                {
                    var tactId = BitConverter.ToUInt64(tactKeyLookup[id].Field<byte[]>("TACTID"), 0);
                    var key = tactKey[id].Field<byte[]>("Key");
                    KeyService.SetKey(tactId, key);
                }
            }

            var lookup = new TactKeyLookup();
            foreach (var kvp in lookup)
                KeyService.SetKey(kvp.Key, kvp.Value);
        }


        private void GenerateCHashLookup()
        {
            var root = (WowRootHandler)Handler.Root;
            var comparer = new CASCLibMD5HashComparer();

            CHashLookup = root.GetAllEntries()
                              .GroupBy(x => x.Value.MD5, comparer)
                              .ToDictionary(x => x.Key, x => x.Select(e => root.GetFileDataIdByHash(e.Key)), comparer);
        }
    }
}
