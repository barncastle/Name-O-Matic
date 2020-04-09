using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NameOMatic.Database;
using NameOMatic.Extensions;
using NameOMatic.Helpers.Collections;
using NameOMatic.Helpers.WoWTools;

namespace NameOMatic.Formats.M2
{
    class M2Enumerator : IFileNamer
    {
        public FileNameLookup FileNames { get; private set; }

        private readonly DiffEnumerator DiffEnumerator;
        private readonly FileEnumerator Enumerator;
        private readonly M2Reader M2Reader;
        private readonly SkelReader SkelReader;
        private readonly BLP.BLPGuesstimator BLPGuesstimator;

        public M2Enumerator(DiffEnumerator diff)
        {
            DiffEnumerator = diff;
            Enumerator = new FileEnumerator("unnamed", "type:m2");
            M2Reader = new M2Reader();
            SkelReader = new SkelReader();
            BLPGuesstimator = new BLP.BLPGuesstimator();
            FileNames = new FileNameLookup();
        }

        public FileNameLookup Enumerate()
        {
            Stopwatch sw = Stopwatch.StartNew();
            var (CursorLeft, CursorTop) = (Console.CursorLeft, Console.CursorTop);
            Console.WriteLine("Started M2 enumeration... ");

            // preload the databases to prevent concurrency errors
            DBContext.Instance.TryLoad("ComponentModelFileData", out _);
            DBContext.Instance.TryLoad("ChrRaces", out _);

            // M2
            DiffEnumerator.FileType = "m2";
            Parallel.ForEach(Enumerator.Union(DiffEnumerator), fileId =>
            {
                if (M2Reader.TryRead(fileId, out var model) && model.IsValid())
                {
                    model.GenerateFileNames();
                    model.GenerateTextures(BLPGuesstimator);
                    FileNames.AddRange(model.FileNames);
                    FileNames.AddRange(ReadSkel(model));
                }
            });

            // SKEL
            DiffEnumerator.FileType = "skel";
            Parallel.ForEach(DiffEnumerator, fileId =>
            {
                if (ListFile.Instance.TryGetValue(fileId, out string filename))
                {
                    if (SkelReader.TryRead(fileId, out var skel))
                    {
                        skel.FileName = filename;
                        skel.GenerateFileNames();
                        FileNames.AddRange(skel.FileNames);
                    }
                }
            });

            sw.StopAndLog("M2", CursorLeft, CursorTop);
            return FileNames;
        }

        private IDictionary<int, string> ReadSkel(M2Model model)
        {
            if (model.FileNames.TryGetValue(model.SkelFileId, out string filename))
            {
                if (SkelReader.TryRead(model.SkelFileId, out var skel))
                {
                    skel.FileName = filename;
                    skel.GenerateFileNames();
                    return skel.FileNames;
                }
            }

            return null;
        }
    }
}
