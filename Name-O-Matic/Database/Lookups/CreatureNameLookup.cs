using System;
using System.IO;

namespace NameOMatic.Database.Lookups
{
    internal class CreatureNameLookup : BaseLookup<int, string>
    {
        public CreatureNameLookup() : base(Path.Combine("Lookups", "CreatureNameLookup.csv"))
        {
            Populate();
        }

        protected override void ParseLine(string line)
        {
            string[] parts = line.Split(',', 3);

            if (int.TryParse(parts[0], out int id))
                this[id] = parts[2];
        }
    }
}
