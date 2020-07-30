using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NameOMatic.Database;
using NameOMatic.Extensions;
using NameOMatic.Helpers.Collections;
using NameOMatic.Helpers.WoWTools;

namespace NameOMatic.Formats.WMO
{
    internal class WMOEnumerator : IFileNamer
    {
        public string Format { get; } = "WMO";
        public bool Enabled { get; } = true;
        public FileNameLookup FileNames { get; private set; }
        public UniqueLookup<string, int> Tokens { get; }

        private readonly DiffEnumerator DiffEnumerator;
        private readonly FileEnumerator FileEnumerator;
        private readonly WMOReader Reader;
#pragma warning disable IDE0052 // Remove unread private members
        private readonly BLP.BLPGuesstimator BLPGuesstimator;
#pragma warning restore IDE0052 // Remove unread private members

        public WMOEnumerator(DiffEnumerator diff)
        {
            DiffEnumerator = diff;
            FileEnumerator = new FileEnumerator("unnamed", "type:wmo");
            Reader = new WMOReader();
            BLPGuesstimator = new BLP.BLPGuesstimator();
            FileNames = new FileNameLookup();
            Tokens = new UniqueLookup<string, int>();
        }

        public FileNameLookup Enumerate()
        {
            Stopwatch sw = Stopwatch.StartNew();
            var (CursorLeft, CursorTop) = (Console.CursorLeft, Console.CursorTop);
            Console.Write("Started WMO enumeration... ");

            // preload database
            DBContext.Instance.TryLoad("AreaTable", out _);

            DiffEnumerator.FileType = "wmo";

            Parallel.ForEach(FileEnumerator.Union(DiffEnumerator), fileId =>
            {
                if (Reader.TryRead(fileId, out var model))
                {
                    model.GenerateFileNames();
                    model.GenerateTextures();
                    FileNames.AddRange(model.FileNames);
                    Tokens.Merge(Reader.Tokens);
                }
            });

            sw.StopAndLog("WMO", CursorLeft, CursorTop);
            return FileNames;
        }
    }
}
