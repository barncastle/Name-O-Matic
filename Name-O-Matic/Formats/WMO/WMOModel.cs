using System;
using System.Collections.Generic;
using System.IO;
using NameOMatic.Database;

namespace NameOMatic.Formats.WMO
{
    class WMOModel : IModel
    {
        public IDictionary<int, string> FileNames { get; }

        public readonly int FileDataId;
        public readonly int WMOId;

        public int NGroups { get; set; }
        public int NLod { get; set; }
        public int[] GroupFileDataId;
        public MOMT[] Materials;

        public WMOModel(int fileDataId, int wmoID)
        {
            FileNames = new Dictionary<int, string>();
            FileDataId = fileDataId;
            WMOId = wmoID;
        }

        public void GenerateFileNames()
        {
            var listfile = ListFile.Instance;
            string name = ToString();

            if (!listfile.ContainsKey(FileDataId))
                FileNames[FileDataId] = name + ".wmo";

            for (int i = 0; i < GroupFileDataId.Length; i++)
            {
                int group = i % NGroups;
                int lod = i / NGroups;

                if (listfile.ContainsKey(GroupFileDataId[i]))
                    continue;

                if (lod == 0)
                    FileNames[GroupFileDataId[i]] = $"{name}_{group:D3}.wmo";
                else
                    FileNames[GroupFileDataId[i]] = $"{name}_{group:D3}_lod{lod}.wmo";
            }
        }

        public void GenerateTextures()
        {
            string name = ToString();

            // TODO can we tell if a texture is a building or a texture? is the blp guesser useful here?
            foreach (var material in Materials)
            {
                if (!ListFile.Instance.ContainsKey(material.texture1))
                    FileNames[material.texture1] = $"{name}_{material.texture1}.blp"; // blpGuesstimator.Guess(material.texture1, name);
                if (!ListFile.Instance.ContainsKey(material.texture2))
                    FileNames[material.texture2] = $"{name}_{material.texture2}.blp"; // blpGuesstimator.Guess(material.texture2, name);
                if (!ListFile.Instance.ContainsKey(material.texture3))
                    FileNames[material.texture3] = $"{name}_{material.texture3}.blp"; // blpGuesstimator.Guess(material.texture3, name);
            }
        }

        public override string ToString()
        {
            if (ListFile.Instance.TryGetValue(FileDataId, out string filename))
                return Path.ChangeExtension(filename, null);

            var guesstimator = WMOGuesstimator.Instance;

            // try to use a previous filename for the same WMOID
            if (!guesstimator.GetPreviousName(WMOId, out filename))
            {
                // lookup up the AreaTable then the WMOAreaTable
                if (!guesstimator.GetAreaName(WMOId, out filename))
                    filename = "unknown"; // then ultimately, fallback to 'unknown'

                return $"world/wmo/autogen-names/{filename}/{WMOId}";
            }
            else
            {
                return filename.Replace("world/minimaps/wmo/", "world/wmo/");
            }
        }
    }
}
