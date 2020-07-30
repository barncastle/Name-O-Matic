using System.Collections.Generic;
using System.Linq;
using NameOMatic.Constants;
using NameOMatic.Database;
using NameOMatic.Extensions;

namespace NameOMatic.Formats.DB2
{
    internal class BakedTexture : IDB2
    {
        public IDictionary<int, string> FileNames { get; }

        public bool IsValid => DBContext.Instance["CreatureDisplayInfoExtra"] != null && DBContext.Instance["TextureFileData"] != null;

        public BakedTexture() => FileNames = new Dictionary<int, string>();

        public void Enumerate()
        {
            var creatureDisplayInfoExtra = DBContext.Instance["CreatureDisplayInfoExtra"];
            var textureFileData = DBContext.Instance["TextureFileData"];
            var listfile = ListFile.Instance;

            var textureMap = textureFileData.Select(x => new
            {
                ID = x.Key,
                Res = x.Value.FieldAs<int>("MaterialResourcesID")
            })
            .ToDictionarySafe(x => x.Res, x => x.ID);

            foreach (var rec in creatureDisplayInfoExtra)
            {
                var bakeMatRes = rec.Value.FieldAs<int>("BakeMaterialResourcesID");
                var hdMatRes = rec.Value.FieldAs<int>("HDBakeMaterialResourcesID");

                if (textureMap.TryGetValue(bakeMatRes, out int fileDataID) && !listfile.ContainsKey(fileDataID))
                {
                    var suffix = TextureSuffix.Get(textureFileData[fileDataID].FieldAs<int>("UsageType"));
                    FileNames[fileDataID] = $"textures/bakednpctextures/creaturedisplayextra-{rec.Key:D5}{suffix}.blp";
                }

                if (textureMap.TryGetValue(hdMatRes, out fileDataID) && !listfile.ContainsKey(fileDataID))
                {
                    var suffix = TextureSuffix.Get(textureFileData[fileDataID].FieldAs<int>("UsageType"));
                    FileNames[fileDataID] = $"textures/bakednpctextures/creaturedisplayextra-{rec.Key:D5}_hd{suffix}.blp";
                }
            }
        }
    }
}
