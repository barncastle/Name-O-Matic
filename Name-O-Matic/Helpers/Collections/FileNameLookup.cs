using System;
using System.IO;
using System.Linq;

namespace NameOMatic.Helpers.Collections
{
    class FileNameLookup : UniqueLookup<int, string>
    {
        public FileNameLookup() : base(null, StringComparer.OrdinalIgnoreCase) { }

        public void Export(string filename) => Export("Output", filename, x => x.ToLowerInvariant());
    }
}
