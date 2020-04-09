using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NameOMatic.Extensions;
using NameOMatic.Helpers.Collections;
using NameOMatic.Helpers.WoWTools;

namespace NameOMatic.Formats.ADTDAT
{
    class ADTDATEnumerator : IFileNamer
    {
        public FileNameLookup FileNames { get; }

        private readonly DiffEnumerator DiffEnumerator;
        private readonly FileEnumerator Enumerator;
        private readonly ADTDATReader Reader;

        public ADTDATEnumerator(DiffEnumerator diff)
        {
            FileNames = new FileNameLookup();

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
                }
            });

            sw.StopAndLog("ADTDAT", CursorLeft, CursorTop);
            return FileNames;
        }
    }
}
