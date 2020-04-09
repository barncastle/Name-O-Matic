using System.Collections.Generic;
using NameOMatic.Database;

namespace NameOMatic.Formats.DB2
{
    class Map : IDB2
    {
        public IDictionary<int, string> FileNames { get; }

        public bool IsValid => DBContext.Instance["Map"] != null;

        public Map() => FileNames = new Dictionary<int, string>();

        public void Enumerate()
        {
            var map = DBContext.Instance["Map"];
            var listfile = ListFile.Instance;

            string directory; int zmpID, wdtID;
            foreach (var rec in map)
            {
                zmpID = rec.Value.FieldAs<int>("ZmpFileDataID");
                wdtID = rec.Value.FieldAs<int>("WdtFileDataID");
                directory = rec.Value.Field<string>("Directory");

                if (!listfile.ContainsKey(zmpID))
                    FileNames[zmpID] = $"interface/worldmap/{directory}.zmp";
                if (!listfile.ContainsKey(wdtID))
                    FileNames[wdtID] = $"world/maps/{directory}/{directory}.wdt";
            }
        }
    }
}
