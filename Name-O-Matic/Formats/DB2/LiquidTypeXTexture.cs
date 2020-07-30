using System.Collections.Generic;
using NameOMatic.Database;

namespace NameOMatic.Formats.DB2
{
    internal class LiquidTypeXTexture : IDB2
    {
        public IDictionary<int, string> FileNames { get; }

        public bool IsValid => DBContext.Instance["LiquidTypeXTexture"] != null && DBContext.Instance["LiquidType"] != null;

        public LiquidTypeXTexture() => FileNames = new Dictionary<int, string>();

        public void Enumerate()
        {
            var liquidtypextexture = DBContext.Instance["LiquidTypeXTexture"];
            var liquidtype = DBContext.Instance["LiquidType"];
            var listfile = ListFile.Instance;

            int liquidTypeID, fileDataID, orderIndex;
            foreach (var rec in liquidtypextexture)
            {
                liquidTypeID = rec.Value.FieldAs<int>("LiquidTypeID");
                fileDataID = rec.Value.FieldAs<int>("FileDataID");
                orderIndex = rec.Value.FieldAs<int>("OrderIndex") + 1;

                if (fileDataID == 0 || listfile.ContainsKey(fileDataID))
                    continue;

                if (liquidtype.TryGetValue(liquidTypeID, out var row))
                {
                    // grab the texture and check it is formattable
                    var texture = row.Field<string[]>("Texture")[0];
                    if (texture.Contains("%d"))
                        FileNames[fileDataID] = texture.Replace("%d", orderIndex.ToString()).Replace("\\", "/");
                }
            }
        }
    }
}
