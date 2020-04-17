using System.Collections.Generic;
using System.Linq;
using NameOMatic.Database;
using NameOMatic.Extensions;

namespace NameOMatic.Formats.DB2
{
    class CharSection : IDB2
    {
        public IDictionary<int, string> FileNames { get; }

        public bool IsValid
        {
            get
            {
                return DBContext.Instance["CharSections"] != null &&
                       DBContext.Instance["ChrRaces"] != null &&
                       DBContext.Instance["TextureFileData"] != null;
            }
        }

        private const string FilenameTemplate = "character/{0}/{1}/{0}{1}{2}.blp";

        public CharSection() => FileNames = new Dictionary<int, string>();

        public void Enumerate()
        {
            var charsections = DBContext.Instance["CharSections"];
            var chrraces = DBContext.Instance["ChrRaces"];
            var textureFileData = DBContext.Instance["TextureFileData"];
            var listfile = ListFile.Instance;

            var raceMap = chrraces.ToDictionary(x => x.Key, x => x.Value["ClientFileString"].ToString());

            var textureMap = textureFileData.Select(x => new
            {
                ID = x.Key,
                Res = x.Value.FieldAs<int>("MaterialResourcesID")
            })
            .ToDictionarySafe(x => x.Res, x => x.ID);

            foreach (var rec in charsections)
            {
                var race = raceMap[rec.Value.FieldAs<int>("RaceID")];
                var gender = rec.Value.FieldAs<bool>("SexID") ? "female" : "male";
                var section = (Constants.CharSection)rec.Value.FieldAs<int>("BaseSection");
                var variation = rec.Value.FieldAs<int>("VariationIndex");
                var color = rec.Value.FieldAs<int>("ColorIndex");
                var matResources = rec.Value.FieldAs<int[]>("MaterialResourcesID");

                for (int i = 0; i < matResources.Length; i++)
                {
                    if (!textureMap.TryGetValue(matResources[i], out int fileDataID))
                        continue;
                    if (listfile.ContainsKey(fileDataID))
                        continue;

                    string variant = string.Format(GetSectionVariationType(section, i), $"{variation:D2}_{color:D2}");
                    if (string.IsNullOrEmpty(variant))
                        continue;

                    // at some point ~Legion they switched to just _hd'ing all skin/face textures
                    string filename = string.Format(FilenameTemplate, race, gender, variant);
                    if (listfile.RecordsReverse.ContainsKey(filename) && FileContext.Instance.Expansion >= Constants.Expansion.BfA)
                        filename = filename.Insert(filename.Length - 4, "_hd");

                    if (!string.IsNullOrEmpty(variant))
                        FileNames[fileDataID] = filename;
                }
            }
        }

        private string GetSectionVariationType(Constants.CharSection section, int material)
        {
            string suffix = section.ToString().StartsWith("HD") ? "_hd" : "";

            switch (section)
            {
                case Constants.CharSection.Skin:
                case Constants.CharSection.HDSkin:
                    return material switch
                    {
                        0 => "skin{0}" + suffix,// character/tauren/male/taurenmaleskin00_22.blp
                        1 => "skin{0}_extra" + suffix,// character/tauren/male/taurenmaleskin00_06_extra.blp
                        _ => "",
                    };
                case Constants.CharSection.Face:
                case Constants.CharSection.HDFace:
                    return material switch
                    {
                        0 => "facelower{0}" + suffix,// character/human/female/humanfemalefacelower00_00.blp
                        1 => "faceupper{0}" + suffix,// character/human/female/humanfemalefaceupper00_00.blp
                        _ => "",
                    };
                case Constants.CharSection.FacialHair:
                case Constants.CharSection.HDFacialHair:
                    return material switch
                    {
                        0 => "faciallowerhair{0}" + suffix,// character/human/faciallowerhair00_00.blp
                        1 => "facialupperhair{0}" + suffix,// character/human/facialupperhair00_00.blp
                        _ => "",
                    };
                case Constants.CharSection.Hair:
                case Constants.CharSection.HDHair:
                    return material switch
                    {
                        0 => "hair{0}" + suffix,// character/human/hair00_13.blp
                        1 => "scalplowerhair{0}" + suffix,// character/human/scalplowerhair02_00.blp
                        2 => "scalpupperhair{0}" + suffix,// character/human/male/scalpupperhair02_00_hd.blp
                        _ => "",
                    };
                case Constants.CharSection.Underwear:
                case Constants.CharSection.HDUnderwear:
                    return material switch
                    {
                        0 => "nakedpelvisskin{0}" + suffix,// character/human/female/humanfemalenakedpelvisskin00_00.blp
                        1 => "nakedtorsoskin{0}" + suffix,// character/human/female/humanfemalenakedtorsoskin00_00.blp
                        _ => "",
                    };
                case Constants.CharSection.Custom1:
                case Constants.CharSection.HDCustom1:
                    return material switch
                    {
                        0 => "tattoo{0}" + suffix,// character/lightforgeddraenei/female/lightforgeddraeneifemaletattoo00.blp
                        _ => "",
                    };
                case Constants.CharSection.Custom2:
                case Constants.CharSection.HDCustom2:
                case Constants.CharSection.Custom3:
                case Constants.CharSection.HDCustom3:
                    return "custom{0}" + suffix;
            }

            return "";
        }
    }
}
