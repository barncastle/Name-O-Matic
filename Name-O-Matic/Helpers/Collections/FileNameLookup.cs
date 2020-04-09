using System;
using System.IO;
using System.Linq;

namespace NameOMatic.Helpers.Collections
{
    class FileNameLookup : UniqueLookup<int, string>
    {
        public FileNameLookup() : base(null, StringComparer.OrdinalIgnoreCase) { }

        public void Export(string filename)
        {
            if (!this.Any())
                return;

            Directory.CreateDirectory("Output");

            using (var fs = File.CreateText(Path.Combine("Output", filename)))
                foreach (var entry in this.OrderBy(x => x.Key))
                    fs.WriteLine(entry.Key + ";" + entry.First().ToLowerInvariant());
        }
    }
}
