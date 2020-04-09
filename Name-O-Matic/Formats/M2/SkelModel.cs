using System.Collections.Generic;
using System.IO;
using NameOMatic.Database;

namespace NameOMatic.Formats.M2
{
    class SkelModel : IModel
    {
        public IDictionary<int, string> FileNames { get; }

        public string FileName { get; set; }

        public readonly int FileDataId;
        public int[] BoneFileIds { get; set; }
        public AnimInfo[] AnimFiles { get; set; }

        public SkelModel(int fileDataId)
        {
            FileNames = new Dictionary<int, string>();

            FileDataId = fileDataId;
        }


        public void GenerateFileNames()
        {
            var listfile = ListFile.Instance;

            string name = Path.ChangeExtension(FileName, null);

            for (int i = 0; i < AnimFiles.Length; i++)
                if (!listfile.ContainsKey(AnimFiles[i].AnimFileId))
                    FileNames[AnimFiles[i].AnimFileId] = name + AnimFiles[i];

            for (int i = 0; i < BoneFileIds.Length; i++)
                if (!listfile.ContainsKey(BoneFileIds[i]))
                    FileNames[BoneFileIds[i]] = name + "_" + BoneFileIds[i].ToString("00") + ".bone";
        }
    }
}
