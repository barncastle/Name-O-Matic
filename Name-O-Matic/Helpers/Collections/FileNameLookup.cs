using System;

namespace NameOMatic.Helpers.Collections
{
    internal class FileNameLookup : UniqueLookup<int, string>
    {
        public FileNameLookup() : base(null, StringComparer.OrdinalIgnoreCase)
        {
        }

        public void Export(string filename) => Export("Output", filename, x => x.ToLowerInvariant());
    }
}
