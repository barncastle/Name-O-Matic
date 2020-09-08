using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using CASCLib;

namespace NameOMatic.Database.Lookups
{
    internal class TactKeyLookup : BaseLookup<ulong, byte[]>
    {
        public TactKeyLookup() : base(Path.Combine("Lookups", "TactKeyLookup.txt"))
        {
            Populate();
            LoadFromGit();
        }

        protected override void ParseLine(string line)
        {
            string[] parts = line.Split(';');

            if (parts.Length == 2 && ulong.TryParse(parts[0], NumberStyles.HexNumber, null, out var key) && !parts[1].Contains('?'))
                this[key] = parts[1].ToByteArray();
        }

        private void LoadFromGit()
        {
            var wc = new WebClient();
            var source = wc.OpenRead("https://raw.githubusercontent.com/wowdev/TACTKeys/master/WoW.txt");
            using var sr = new StreamReader(source);

            while (!sr.EndOfStream)
                ParseLine(sr.ReadLine().Replace(" ", ";"));
        }
    }
}
