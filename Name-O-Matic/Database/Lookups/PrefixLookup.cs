using System;
using System.IO;

namespace NameOMatic.Database.Lookups
{
    internal class PrefixLookup : BaseLookup<string, string>
    {
        public int MaxTokenCount { get; private set; }

        public PrefixLookup() : base(Path.Combine("Lookups", "PrefixLookup.txt"))
        { }

        protected override void ParseLine(string line)
        {
            string[] parts = line.Split(';', 2);

            if (!parts[1].EndsWith('/'))
                parts[1] += '/';

            this[parts[0]] = parts[1];

            MaxTokenCount = Math.Max(parts[0].Split('_').Length + 1, MaxTokenCount);
        }
    }
}
