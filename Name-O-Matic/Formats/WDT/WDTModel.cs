using System;
using System.Collections.Generic;
using NameOMatic.Database;

namespace NameOMatic.Formats.WDT
{
    class WDTModel : IModel
    {
        public IDictionary<int, string> FileNames { get; }

        public readonly int FileDataId;

        public string Directory { get; set; }
        public MPHD Header { get; set; }
        public MAID[] MapIDs { get; set; }


        public WDTModel(int fileDataId)
        {
            FileNames = new Dictionary<int, string>();
            FileDataId = fileDataId;
        }

        public void GenerateFileNames()
        {
            if (string.IsNullOrWhiteSpace(Directory))
                return;

            string name = $"world/maps/{Directory}/{Directory}";
            string map = Directory;
            var listfile = ListFile.Instance;

            if (!listfile.ContainsKey(Header.LgtFile))
                FileNames[Header.LgtFile] = name + "_lgt.wdt";
            if (!listfile.ContainsKey(Header.OccFile))
                FileNames[Header.OccFile] = name + "_occ.wdt";
            if (!listfile.ContainsKey(Header.FogsFile))
                FileNames[Header.FogsFile] = name + "_fogs.wdt";
            if (!listfile.ContainsKey(Header.MpvFile))
                FileNames[Header.MpvFile] = name + "_mpv.wdt";
            if (!listfile.ContainsKey(Header.TexFile))
                FileNames[Header.TexFile] = name + ".tex";
            if (!listfile.ContainsKey(Header.WdlFile))
                FileNames[Header.WdlFile] = name + ".wdl";
            if (!listfile.ContainsKey(Header.PD4File))
                FileNames[Header.PD4File] = name + ".pd4";

            if (FileDataId == 3182457)
                Console.WriteLine();

            int i, j;
            for (int m = 0; m < MapIDs.Length; m++)
            {
                MAID maid = MapIDs[m];
                i = m % 64; j = m / 64;

                if (!listfile.ContainsKey(maid.RootADT))
                    FileNames[maid.RootADT] = name + $"_{i}_{j}.adt";
                if (!listfile.ContainsKey(maid.Obj0ADT))
                    FileNames[maid.Obj0ADT] = name + $"_{i}_{j}_obj0.adt";
                if (!listfile.ContainsKey(maid.Obj1ADT))
                    FileNames[maid.Obj1ADT] = name + $"_{i}_{j}_obj1.adt";
                if (!listfile.ContainsKey(maid.Tex0ADT))
                    FileNames[maid.Tex0ADT] = name + $"_{i}_{j}_tex0.adt";
                if (!listfile.ContainsKey(maid.LodADT))
                    FileNames[maid.LodADT] = name + $"_{i}_{j}_lod.adt";
                if (!listfile.ContainsKey(maid.MapTexture))
                    FileNames[maid.MapTexture] = $"world/maptextures/{map}/{map}_{i:D2}_{j:D2}.blp";
                if (!listfile.ContainsKey(maid.MapTextureN))
                    FileNames[maid.MapTextureN] = $"world/maptextures/{map}/{map}_{i:D2}_{j:D2}_n.blp";
                if (!listfile.ContainsKey(maid.MinimapTexture))
                    FileNames[maid.MinimapTexture] = $"world/minimaps/{map}/map{i:D2}_{j:D2}.blp";
            }
        }
    }
}
