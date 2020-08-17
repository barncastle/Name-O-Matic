using System.Collections.Generic;
using System.IO;
using System.Linq;
using NameOMatic.Constants;
using NameOMatic.Database;
using NameOMatic.Extensions;

namespace NameOMatic.Formats.DB2
{
    internal class ComponentTextureFileData : IDB2
    {
        public IDictionary<int, string> FileNames { get; }

        public bool IsValid
        {
            get
            {
                return DBContext.Instance["ComponentTextureFileData"] != null &&
                       DBContext.Instance["TextureFileData"] != null &&
                       DBContext.Instance["ItemDisplayInfoMaterialRes"] != null &&
                       DBContext.Instance["ItemAppearance"] != null;
            }
        }

        public ComponentTextureFileData() => FileNames = new Dictionary<int, string>();

        public void Enumerate()
        {
            var componentTextureFileData = DBContext.Instance["ComponentTextureFileData"];
            var textureFileData = DBContext.Instance["TextureFileData"];
            var itemDisplayInfoMaterialRes = DBContext.Instance["ItemDisplayInfoMaterialRes"];
            var itemAppearance = DBContext.Instance["ItemAppearance"];
            var listfile = ListFile.Instance;

            /*
             *  componentTextureFileData.FileDataId -> textureFileData.FileDataId
             *  textureFileData.MaterialResourcesID -> itemDisplayInfoMaterialRes.MaterialResourcesID
             *  itemDisplayInfoMaterialRes.ItemDisplayInfoID -> itemAppearance.ItemDisplayInfoID
             *  where !listfile.contains(componentTextureFileData.FileDataId) :: already named
             *     && listfile.contains(itemAppearance.DefaultIconFileDataID) :: icon is named
             *     && itemAppearance.ItemDisplayInfoID != 0
             */

            var temp = from ctf in componentTextureFileData
                       join tfd in textureFileData on ctf.Key equals tfd.Key
                       join idir in itemDisplayInfoMaterialRes on tfd.Value.FieldAs<int>("MaterialResourcesID") equals idir.Value.FieldAs<int>("MaterialResourcesID")
                       join ia in itemAppearance on idir.Value.FieldAs<int>("ItemDisplayInfoID") equals ia.Value.FieldAs<int>("ItemDisplayInfoID")
                       where !listfile.ContainsKey(ctf.Key) &&
                              listfile.ContainsKey(ia.Value.FieldAs<int>("DefaultIconFileDataID")) &&
                              ia.Value.FieldAs<int>("ItemDisplayInfoID") > 0

                       select new
                       {
                           FileDataId = ctf.Key,
                           TextureSuffix = TextureSuffix.Get(tfd.Value.FieldAs<int>("UsageType")),
                           Gender = (ComponentGender)ctf.Value.FieldAs<int>("GenderIndex"),
                           Section = (ComponentSection)idir.Value.FieldAs<int>("ComponentSection"),
                           IconName = listfile[ia.Value.FieldAs<int>("DefaultIconFileDataID")]
                       };

            string slot, collection;
            foreach (var rec in temp.Unique(x => x.FileDataId))
            {
                // use the icon to guesstimate the slot and collection name
                var parts = Path.GetFileNameWithoutExtension(rec.IconName).Split('_', 3);
                if (parts.Length < 3)
                    continue;

                slot = parts[1];
                collection = parts[2];

                if (string.IsNullOrEmpty(slot) || string.IsNullOrEmpty(collection))
                    continue;
                if (!SectionPaths.TryGetValue(rec.Section, out string path))
                    continue;

                FileNames[rec.FileDataId] = $"{path}{collection}_{slot}_{rec.Section}_{rec.Gender}{rec.TextureSuffix}.blp";
            }
        }

        private static readonly Dictionary<ComponentSection, string> SectionPaths = new Dictionary<ComponentSection, string>
        {
            [ComponentSection.AL] = "item/texturecomponents/armlowertexture/",
            [ComponentSection.AU] = "item/texturecomponents/armuppertexture/",
            [ComponentSection.FO] = "item/texturecomponents/foottexture/",
            [ComponentSection.HA] = "item/texturecomponents/handtexture/",
            [ComponentSection.LL] = "item/texturecomponents/leglowertexture/",
            [ComponentSection.LU] = "item/texturecomponents/leguppertexture/",
            [ComponentSection.PR] = "item/texturecomponents/accessorytexture/",
            [ComponentSection.TL] = "item/texturecomponents/torsolowertexture/",
            [ComponentSection.TU] = "item/texturecomponents/torsouppertexture/",
        };
    }
}
