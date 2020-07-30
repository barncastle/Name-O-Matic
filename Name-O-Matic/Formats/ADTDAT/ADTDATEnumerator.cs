using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NameOMatic.Extensions;
using NameOMatic.Helpers.Collections;
using NameOMatic.Helpers.WoWTools;

namespace NameOMatic.Formats.ADTDAT
{
    internal class ADTDATEnumerator : IFileNamer
    {
        public string Format { get; } = "ADTDAT";
        public bool Enabled { get; } = true;
        public FileNameLookup FileNames { get; }
        public UniqueLookup<string, int> Tokens { get; }

        private readonly DiffEnumerator DiffEnumerator;
        private readonly FileEnumerator Enumerator;
        private readonly ADTDATReader Reader;

        public ADTDATEnumerator(DiffEnumerator diff)
        {
            FileNames = new FileNameLookup();
            Tokens = new UniqueLookup<string, int>();

            DiffEnumerator = diff;
            Enumerator = new FileEnumerator("unnamed", "type:adtdat");
            Reader = new ADTDATReader();
        }

        public FileNameLookup Enumerate()
        {
            Stopwatch sw = Stopwatch.StartNew();
            var (CursorLeft, CursorTop) = (Console.CursorLeft, Console.CursorTop);
            Console.Write("Started ADTDAT enumeration... ");

            DiffEnumerator.FileType = "adtdat";

            Parallel.ForEach(Enumerator.Union(DiffEnumerator), fileId =>
            {
                if (Reader.TryRead(fileId, out var model))
                {
                    model.GenerateFileNames();
                    FileNames.AddRange(model.FileNames);
                    Tokens.Merge(Reader.Tokens);
                }
            });

            sw.StopAndLog("ADTDAT", CursorLeft, CursorTop);
            return FileNames;
        }
    }
}
