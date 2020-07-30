using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NameOMatic.Constants;
using NameOMatic.Database.Lookups;
using TACT.Net;
using TACT.Net.Configs;
using TACT.Net.Cryptography;

namespace NameOMatic.Database.Storage
{
    using CHashLookup = Dictionary<MD5Hash, IEnumerable<int>>;

    internal class TACTNetWrapper : IStorage
    {
        public string Build { get; }
        public string FormattedBuild { get; }
        public Expansion Expansion { get; }
        public string RootEKey { get; }

        private TACTRepo Repo;
        private CHashLookup CHashLookup;

        public TACTNetWrapper(string path, string buildconfig, string cdnconfig)
        {
            if (string.IsNullOrWhiteSpace(buildconfig))
                throw new ArgumentNullException(nameof(buildconfig), "Invalid buildconfig hash");
            if (string.IsNullOrWhiteSpace(cdnconfig))
                throw new ArgumentNullException(nameof(cdnconfig), "Invalid cdnconfig hash");

            Repo = new TACTRepo(path)
            {
                ConfigContainer = new ConfigContainer()
            };
            Repo.ConfigContainer.OpenLocal(Repo.BaseDirectory, buildconfig, cdnconfig);

            Console.WriteLine("Loading indices..");
            Repo.IndexContainer = new TACT.Net.Indices.IndexContainer();
            Repo.IndexContainer.Open(Repo.BaseDirectory, Repo.ConfigContainer);

            Console.WriteLine("Loading encoding..");
            Repo.EncodingFile = new TACT.Net.Encoding.EncodingFile(Repo.BaseDirectory, Repo.ConfigContainer.EncodingEKey, true);

            Console.WriteLine("Looking up root..");
            Repo.EncodingFile.TryGetCKeyEntry(Repo.ConfigContainer.RootCKey, out var rootCEntry);
            RootEKey = rootCEntry.EKey.ToString();

            Console.WriteLine("Loading root..");
            Repo.RootFile = new TACT.Net.Root.RootFile(Repo.BaseDirectory, rootCEntry.EKey);

            // store various build info
            var buildName = Repo.ConfigContainer.BuildConfig.GetValue("Build-Name");
            var regex = Regex.Match(buildName, @"(\d+)patch(\d\.\d+\.\d)");
            var build = regex.Groups[2].Value.Split('.');

            Build = regex.Groups[2].Value + "." + regex.Groups[1].Value;
            FormattedBuild = string.Join("", build, 0, build[2] == "0" ? 2 : 3);
            Expansion = (Expansion)byte.Parse(build[0]);

            GenerateCHashLookup();
        }

        public bool FileExists(int fileDataId) => Repo.RootFile.ContainsFileId((uint)fileDataId);

        public Stream OpenFile(int fileDataId) => Repo.RootFile.OpenFile((uint)fileDataId, Repo) ?? new MemoryStream(0);

        public IEnumerable<int> GetMatchingFiles(int fileDataId)
        {
            var fileentry = Repo.RootFile.Get((uint)fileDataId).FirstOrDefault();

            if (fileentry != null && CHashLookup.TryGetValue(fileentry.CKey, out var fids))
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
                    KeyService.TryAddKey(tactId, key);
                }
            }

            var lookup = new TactKeyLookup();
            foreach (var kvp in lookup)
                KeyService.TryAddKey(kvp.Key, kvp.Value);
        }

        private void GenerateCHashLookup()
        {
            var root = Repo.RootFile;
            var comparer = new MD5HashComparer();

            CHashLookup = root.GetBlocks(root.LocaleFlags)
                              .SelectMany(x => x.Records)
                              .GroupBy(x => x.Value.CKey, comparer)
                              .ToDictionary(x => x.Key, x => x.Select(e => (int)e.Key), comparer);
        }
    }
}
