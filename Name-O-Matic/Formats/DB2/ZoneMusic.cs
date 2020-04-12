using System.Collections.Generic;
using System.Text.RegularExpressions;
using NameOMatic.Constants;
using NameOMatic.Database;

namespace NameOMatic.Formats.DB2
{
    class ZoneMusic : IDB2
    {
        public IDictionary<int, string> FileNames { get; }

        public bool IsValid => DBContext.Instance["ZoneMusic"] != null && DBContext.Instance["SoundKitEntry"] != null;

        private readonly SoundGlobalIndexer GlobalIndex;
        private readonly Regex PrefixRegex;

        public ZoneMusic()
        {
            FileNames = new Dictionary<int, string>();
            GlobalIndex = SoundGlobalIndexer.Instance;
            PrefixRegex = new Regex(@"^([A-Z]+)_\d+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public void Enumerate()
        {
            var zoneMusic = DBContext.Instance["ZoneMusic"];
            var listfile = ListFile.Instance;

            var expansion = (ExpansionFull)FileContext.Instance.Expansion;

            foreach (var rec in zoneMusic)
            {
                string setName = PrefixRegex.Replace(rec.Value.Field<string>("SetName"), "");
                int[] sounds = rec.Value.FieldAs<int[]>("Sounds");

                for (int i = 0; i < sounds.Length; i++)
                {
                    if (GlobalIndex.TryGetValue(sounds[i], out var fileIds))
                    {
                        for (int j = 0; j < fileIds.Count; j++)
                        {
                            if (listfile.ContainsKey(fileIds[j]))
                                continue;

                            FileNames[fileIds[j]] = $"sound/music/{expansion}/mus_{setName.TrimStart('_')}_{j + 1:D2}.mp3";
                        }
                    }
                }
            }
        }
    }
}
