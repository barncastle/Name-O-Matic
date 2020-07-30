using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using CASCLib;

namespace NameOMatic.Database.Lookups
{
    class TactKeyLookup : BaseLookup<ulong, byte[]>
    {
        public TactKeyLookup() : base(Path.Combine("Lookups", "TactKeyLookup.txt"))
        {
            Populate();
        }

        protected override void ParseLine(string line)
        {
            string[] parts = line.Split(';', 2);

            if (ulong.TryParse(parts[0], NumberStyles.HexNumber, null, out var key))
                this[key] = parts[1].ToByteArray();
        }
    }
}
