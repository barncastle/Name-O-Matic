using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;
using NameOMatic.Constants;
using NameOMatic.Database;
using NameOMatic.Extensions;
using NameOMatic.Helpers.Collections;

namespace NameOMatic.Formats.BLP
{
    class BLPEnumerator : IFileNamer
    {
        public string Format { get; } = "BLP";
        public bool Enabled { get; } = true;
        public FileNameLookup FileNames { get; }
        public UniqueLookup<string, int> Tokens { get; }

        private readonly BLPGuesstimator BLPGuesstimator;

        public BLPEnumerator()
        {
            FileNames = new FileNameLookup();
            Tokens = new UniqueLookup<string, int>();
            BLPGuesstimator = new BLPGuesstimator();
        }

        public FileNameLookup Enumerate()
        {
            Stopwatch sw = Stopwatch.StartNew();
            var (CursorLeft, CursorTop) = (Console.CursorLeft, Console.CursorTop);
            Console.Write("Started BLP enumeration... ");

            Parallel.ForEach(ListFile.Instance, file =>
            {
                if (!IsValid(file.Value))
                    return;

                var match = BLPGuesstimator.FileIdSuffix.Match(file.Value);
                if (match.Success)
                {
                    var filename = BLPGuesstimator.Guess(file.Key, file.Value[0..^11]);
                    if (!string.IsNullOrEmpty(filename) && filename != file.Value)
                        FileNames.Add(file.Key, filename);
                }
            });

            sw.StopAndLog("BLP", CursorLeft, CursorTop);
            return FileNames;
        }

        private bool IsValid(string filename)
        {
            if (filename.Contains("bakednpctextures", StringComparison.OrdinalIgnoreCase))
                return false;
            if (filename.StartsWith("world/wmo/", StringComparison.OrdinalIgnoreCase))
                return false;
            return true;
        }
    }
}
