﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NameOMatic.Extensions;
using NameOMatic.Helpers.Collections;
using NameOMatic.Helpers.WoWTools;

namespace NameOMatic.Formats.WMO
{
    class WMOEnumerator : IFileNamer
    {
        public FileNameLookup FileNames { get; private set; }

        private readonly DiffEnumerator DiffEnumerator;
        private readonly FileEnumerator FileEnumerator;
        private readonly WMOReader Reader;
        private readonly BLP.BLPGuesstimator BLPGuesstimator;

        public WMOEnumerator(DiffEnumerator diff)
        {
            DiffEnumerator = diff;
            FileEnumerator = new FileEnumerator("unnamed", "type:wmo");
            Reader = new WMOReader();
            BLPGuesstimator = new BLP.BLPGuesstimator();
            FileNames = new FileNameLookup();
        }

        public FileNameLookup Enumerate()
        {
            Stopwatch sw = Stopwatch.StartNew();
            var (CursorLeft, CursorTop) = (Console.CursorLeft, Console.CursorTop);
            Console.Write("Started WMO enumeration... ");

            DiffEnumerator.FileType = "wmo";

            Parallel.ForEach(FileEnumerator.Union(DiffEnumerator), fileId =>
            {
                if (Reader.TryRead(fileId, out var model))
                {
                    model.GenerateFileNames();
                    model.GenerateTextures(BLPGuesstimator);
                    FileNames.AddRange(model.FileNames);
                }
            });

            sw.StopAndLog("WMO", CursorLeft, CursorTop);
            return FileNames;
        }
    }
}
