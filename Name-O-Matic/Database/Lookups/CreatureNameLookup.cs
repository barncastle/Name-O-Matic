using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace NameOMatic.Database.Lookups
{
    class CreatureNameLookup : BaseLookup<int, string>
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
