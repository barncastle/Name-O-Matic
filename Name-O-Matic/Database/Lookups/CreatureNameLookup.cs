using System;
using System.IO;
using System.Text.RegularExpressions;

namespace NameOMatic.Database.Lookups
{
    internal class CreatureNameLookup : BaseLookup<int, string>
    {
        private readonly Regex Replacements = new Regex(@"[,'\\\/]+");

        public CreatureNameLookup() : base(Path.Combine("Lookups", "CreatureNameLookup.csv"))
        {
            Populate();
        }

        protected override void ParseLine(string line)
        {
            string[] parts = line.Split(',', 3);

            if (int.TryParse(parts[0], out int id))
                this[id] = Replacements.Replace(parts[2], "");
        }
    }
}
