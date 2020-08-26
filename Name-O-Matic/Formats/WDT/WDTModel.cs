using System.Collections.Generic;
using NameOMatic.Database;

namespace NameOMatic.Formats.WDT
{
    internal class WDTModel : IModel
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

            bool IsValidReplacement(int fid) => !listfile.ContainsKey(fid) || listfile[fid].Contains("unknown");

            if (IsValidReplacement(FileDataId))
                FileNames[FileDataId] = name + ".wdt";
            if (IsValidReplacement(Header.LgtFile))
                FileNames[Header.LgtFile] = name + "_lgt.wdt";
            if (IsValidReplacement(Header.OccFile))
                FileNames[Header.OccFile] = name + "_occ.wdt";
            if (IsValidReplacement(Header.FogsFile))
                FileNames[Header.FogsFile] = name + "_fogs.wdt";
            if (IsValidReplacement(Header.MpvFile))
                FileNames[Header.MpvFile] = name + "_mpv.wdt";
            if (IsValidReplacement(Header.TexFile))
                FileNames[Header.TexFile] = name + ".tex";
            if (IsValidReplacement(Header.WdlFile))
                FileNames[Header.WdlFile] = name + ".wdl";
            if (IsValidReplacement(Header.PD4File))
                FileNames[Header.PD4File] = name + ".pd4";

            int i, j;
            for (int m = 0; m < MapIDs.Length; m++)
            {
                MAID maid = MapIDs[m];
                i = m % 64; j = m / 64;

                if (IsValidReplacement(maid.RootADT))
                    FileNames[maid.RootADT] = name + $"_{i}_{j}.adt";
                if (IsValidReplacement(maid.Obj0ADT))
                    FileNames[maid.Obj0ADT] = name + $"_{i}_{j}_obj0.adt";
                if (IsValidReplacement(maid.Obj1ADT))
                    FileNames[maid.Obj1ADT] = name + $"_{i}_{j}_obj1.adt";
                if (IsValidReplacement(maid.Tex0ADT))
                    FileNames[maid.Tex0ADT] = name + $"_{i}_{j}_tex0.adt";
                if (IsValidReplacement(maid.LodADT))
                    FileNames[maid.LodADT] = name + $"_{i}_{j}_lod.adt";
                if (IsValidReplacement(maid.MapTexture))
                    FileNames[maid.MapTexture] = $"world/maptextures/{map}/{map}_{i:D2}_{j:D2}.blp";
                if (IsValidReplacement(maid.MapTextureN))
                    FileNames[maid.MapTextureN] = $"world/maptextures/{map}/{map}_{i:D2}_{j:D2}_n.blp";
                if (IsValidReplacement(maid.MinimapTexture))
                    FileNames[maid.MinimapTexture] = $"world/minimaps/{map}/map{i:D2}_{j:D2}.blp";
            }
        }
    }
}
