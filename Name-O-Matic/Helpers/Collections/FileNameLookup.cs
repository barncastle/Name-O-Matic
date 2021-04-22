using System;
using System.IO;
using System.Linq;

namespace NameOMatic.Helpers.Collections
{
    internal class FileNameLookup : UniqueLookup<int, string>
    {
        private const string OutputDirectory = "Output";

        public FileNameLookup() : base(null, StringComparer.OrdinalIgnoreCase)
        {
        }

        public void Export(string filename)
        {
            Export(OutputDirectory, filename, x => x.ToLowerInvariant());
        }

        public void Dump(string filename)
        {
            if (Data.Count == 0)
                return;

            Directory.CreateDirectory(OutputDirectory);

            using var fs = File.CreateText(Path.Combine(OutputDirectory, filename));            

            foreach (var entry in Data.OrderBy(x => x.Key))
            {
                foreach(var item in entry.Value)
                    fs.WriteLine(entry.Key + ";" + item);
            }
        }
    }
}
