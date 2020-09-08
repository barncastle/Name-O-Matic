using System;
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
            EnumerateCollections();
            EnumerateCapes();
        }

        private void EnumerateCollections()
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

                FileNames[rec.FileDataId] = $"{path}{collection}_{slot}_{rec.Section}_{rec.Gender}{rec.TextureSuffix}_{rec.FileDataId}.blp".Replace("__", "_");
            }
        }

        private void EnumerateCapes()
        {
            var componentTextureFileData = DBContext.Instance["ComponentTextureFileData"];
            var textureFileData = DBContext.Instance["TextureFileData"];
            var itemDisplayInfo = DBContext.Instance["ItemDisplayInfo"];
            var itemAppearance = DBContext.Instance["ItemAppearance"];
            var listfile = ListFile.Instance;

            if (itemDisplayInfo == null)
                return;

            /*
             * textureFileData.MaterialResourcesID -> itemDisplayInfo.ModelMaterialResourcesID[0]
             * itemDisplayInfo.ID -> itemAppearance.ItemDisplayInfoID
             *  where !listfile.contains(textureFileData.FileDataId) :: already named
             *     && listfile.contains(itemAppearance.DefaultIconFileDataID) :: icon is named
             *     && itemDisplayInfo.ModelResourcesID == 0 :: no model to parse
             */
            var temp = from tfd in textureFileData
                       join idi in itemDisplayInfo on tfd.Value.FieldAs<int>("MaterialResourcesID") equals idi.Value.FieldAs<int[]>("ModelMaterialResourcesID")[0]
                       join ia in itemAppearance on idi.Key equals ia.Value.FieldAs<int>("ItemDisplayInfoID")
                       where !listfile.ContainsKey(tfd.Key) &&
                             listfile.ContainsKey(ia.Value.FieldAs<int>("DefaultIconFileDataID")) &&
                             idi.Value.FieldAs<int[]>("ModelResourcesID").All(x => x == 0)

                       select new
                       {
                           FileDataId = tfd.Key,
                           IconName = listfile[ia.Value.FieldAs<int>("DefaultIconFileDataID")]
                       };

            string icon;
            foreach (var rec in temp.Unique(x => x.FileDataId))
            {
                if (!rec.IconName.Contains("cape"))
                    continue;

                // use the icon to guesstimate the name
                icon = Path.GetFileNameWithoutExtension(rec.IconName);
                if (icon.StartsWith("inv"))
                    icon = icon[3..].TrimStart('_');
                if (icon.EndsWith("cape"))
                    icon = icon[0..^4];
                if (!icon.StartsWith("cape"))
                    icon = "cape_" + icon;

                FileNames[rec.FileDataId] = $"item/objectcomponents/cape/{icon}_{rec.FileDataId}.blp".Replace("__", "_");
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
