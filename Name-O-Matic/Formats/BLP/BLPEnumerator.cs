using System;
using System.Diagnostics;
using NameOMatic.Database;
using NameOMatic.Extensions;
using NameOMatic.Helpers.Collections;

namespace NameOMatic.Formats.BLP
{
    class BLPEnumerator : IFileNamer
    {
        public FileNameLookup FileNames { get; }

        private readonly BLPGuesstimator BLPGuesstimator;

        public BLPEnumerator()
        {
            FileNames = new FileNameLookup();
            BLPGuesstimator = new BLPGuesstimator();
        }

        public FileNameLookup Enumerate()
        {
            Stopwatch sw = Stopwatch.StartNew();
            var (CursorLeft, CursorTop) = (Console.CursorLeft, Console.CursorTop);
            Console.Write("Started BLP enumeration... ");

            foreach (var file in ListFile.Instance)
            {
                var match = BLPGuesstimator.FileIdSuffix.Match(file.Value);
                if (match.Success)
                {
                    var filename = BLPGuesstimator.Guess(file.Key, file.Value.Substring(0, file.Value.Length - 11));
                    if (filename != file.Value)
                        FileNames.Add(file.Key, filename);
                }
            }

            sw.StopAndLog("BLP", CursorLeft, CursorTop);
            return FileNames;
        }
    }
}
