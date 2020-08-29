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
            ScrapeWoWTools();
        }

        protected override void ParseLine(string line)
        {
            string[] parts = line.Split(';');

            if (parts.Length == 2 && ulong.TryParse(parts[0], NumberStyles.HexNumber, null, out var key) && !parts[1].Contains('?'))
                this[key] = parts[1].ToByteArray();
        }

        private void ScrapeWoWTools()
        {
            var wc = new WebClient();
            var source = wc.DownloadString("https://wow.tools/dbc/tactkey.php");
            var regex = new Regex("\"keyname\":\"([A-F0-9]{16})\",\"keybytes\":\"([A-F0-9]{32})\"");

            foreach (Match match in regex.Matches(source))
            {
                if (match.Success && ulong.TryParse(match.Groups[1].Value, NumberStyles.HexNumber, null, out var key) && !match.Groups[2].Value.Contains('?'))
                    this[key] = match.Groups[2].Value.ToByteArray();
            }
        }
    }
}
