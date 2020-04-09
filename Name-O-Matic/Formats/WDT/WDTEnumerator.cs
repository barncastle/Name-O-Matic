using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NameOMatic.Database;
using NameOMatic.Extensions;
using NameOMatic.Helpers.Collections;
using NameOMatic.Helpers.WoWTools;

namespace NameOMatic.Formats.WDT
{
    class WDTEnumerator : IFileNamer
    {
        public FileNameLookup FileNames { get; private set; }

        private readonly DiffEnumerator DiffEnumerator;
        private readonly WDTReader Reader;

        public WDTEnumerator(DiffEnumerator diff)
        {
            DiffEnumerator = diff;
            Reader = new WDTReader();
            FileNames = new FileNameLookup();
        }

        public FileNameLookup Enumerate()
        {
            Stopwatch sw = Stopwatch.StartNew();
            var (CursorLeft, CursorTop) = (Console.CursorLeft, Console.CursorTop);
            Console.Write("Started WDT enumeration... ");

            Parallel.ForEach(DBContext.Instance["Map"], entry =>
            {
                int wdtFileDataID = entry.Value.FieldAs<int>("WdtFileDataID");
                string directory = entry.Value.Field<string>("Directory");

                if (Reader.TryRead(wdtFileDataID, out var model))
                {
                    model.Directory = directory;
                    model.GenerateFileNames();
                    FileNames.AddRange(model.FileNames);
                }
            });

            DiffEnumerator.FileType = "wdt";
            Parallel.ForEach(DiffEnumerator, fileID =>
            {
                if (Reader.TryRead(fileID, out var model))
                {
                    model.GenerateFileNames();
                    FileNames.AddRange(model.FileNames);
                }
            });

            sw.StopAndLog("WDT", CursorLeft, CursorTop);
            return FileNames;
        }
    }
}
