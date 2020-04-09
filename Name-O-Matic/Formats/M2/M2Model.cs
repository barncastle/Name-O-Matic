using System;
using System.Collections.Generic;
using System.IO;
using NameOMatic.Constants;
using NameOMatic.Database;

namespace NameOMatic.Formats.M2
{
    class M2Model : IModel
    {
        public IDictionary<int, string> FileNames { get; }

        public int FileDataId { get; }
        public string InternalName { get; private set; }

        public string Directory { get; set; } = "";
        public string Suffix { get; set; } = "";

        public int NViews { get; set; }
        public int NLod { get; set; }
        public int[] SkinFileIds { get; set; }
        public int[] LodSkinFileIds { get; set; }
        public int[] TextureFileIds { get; set; }
        public int[] BoneFileIds { get; set; }
        public AnimInfo[] AnimFiles { get; set; }
        public int PhysFileId { get; set; }
        public int SkelFileId { get; set; }

        private bool HasRaceSuffix;

        public M2Model(int fileDataId, string internalName)
        {
            FileNames = new Dictionary<int, string>();
            InternalName = internalName;

            FileDataId = fileDataId;
            TryGetSuffix();
        }


        public void GenerateFileNames()
        {
            if (!IsValid())
                return;

            var listfile = ListFile.Instance;
            string name = ToString();

            if (!listfile.ContainsKey(FileDataId))
                FileNames.Add(FileDataId, name + ".m2");

            for (int i = 0; i < SkinFileIds.Length; i++)
                if (!listfile.ContainsKey(SkinFileIds[i]))
                    FileNames[SkinFileIds[i]] = name + i.ToString("00") + ".skin";

            for (int i = 0; i < LodSkinFileIds.Length; i++)
                if (!listfile.ContainsKey(LodSkinFileIds[i]))
                    FileNames[LodSkinFileIds[i]] = name + "_lod" + i.ToString("00") + ".skin";

            for (int i = 0; i < AnimFiles.Length; i++)
                if (!listfile.ContainsKey(AnimFiles[i].AnimFileId))
                    FileNames[AnimFiles[i].AnimFileId] = name + AnimFiles[i];

            for (int i = 0; i < BoneFileIds.Length; i++)
                if (!listfile.ContainsKey(BoneFileIds[i]))
                    FileNames[BoneFileIds[i]] = name + "_" + BoneFileIds[i].ToString("00") + ".bone";

            if (!listfile.ContainsKey(PhysFileId))
                FileNames[PhysFileId] = name + ".phys";

            if (!listfile.ContainsKey(SkelFileId))
                FileNames[SkelFileId] = name + ".skel";
        }

        public void GenerateTextures(BLP.BLPGuesstimator blpGuesstimator)
        {
            string name = ToString();

            // strip the _{Race}_[M|F] suffix
            if (HasRaceSuffix)
                name = name.Substring(0, name.Length - Suffix.Length - 1);

            foreach (var fileId in TextureFileIds)
                if (!ListFile.Instance.ContainsKey(fileId))
                    FileNames[fileId] = blpGuesstimator.Guess(fileId, name);
        }

        public bool IsValid()
        {
            return FileDataId > 0 && (ListFile.Instance.ContainsKey(FileDataId) || !string.IsNullOrEmpty(Directory));
        }

        public override string ToString()
        {
            if (ListFile.Instance.TryGetValue(FileDataId, out string filename))
                return Path.ChangeExtension(filename, null);

            return Directory + InternalName + Suffix;
        }

        private void TryGetSuffix()
        {
            if (DBContext.Instance["ComponentModelFileData"] == null || DBContext.Instance["ChrRaces"] == null)
                return;

            if (DBContext.Instance["ComponentModelFileData"].TryGetValue(FileDataId, out var modelRow))
            {
                int raceID = modelRow.FieldAs<int>("RaceID");
                if (DBContext.Instance["ChrRaces"].TryGetValue(raceID, out var raceRow))
                {
                    Suffix = raceRow["ClientPrefix"] + "_" + (ComponentGender)modelRow.FieldAs<int>("GenderIndex");
                    HasRaceSuffix = true;

                    if (InternalName.EndsWith(Suffix, StringComparison.OrdinalIgnoreCase))
                        InternalName = InternalName.Substring(0, InternalName.Length - Suffix.Length);
                }
            }
        }
    }
}
