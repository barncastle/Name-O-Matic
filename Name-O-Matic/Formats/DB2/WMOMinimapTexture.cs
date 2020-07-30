using System.Collections.Generic;
using NameOMatic.Database;

namespace NameOMatic.Formats.DB2
{
    internal class WMOMinimapTexture : IDB2
    {
        public IDictionary<int, string> FileNames { get; }

        public bool IsValid => DBContext.Instance["WMOMinimapTexture"] != null;

        private readonly HashSet<int> UnnamedRecordIDs;
        private readonly Dictionary<int, string> WMONameMap;

        public WMOMinimapTexture()
        {
            FileNames = new Dictionary<int, string>();

            UnnamedRecordIDs = new HashSet<int>(0x500);
            WMONameMap = new Dictionary<int, string>(0x1000);
        }

        public void Enumerate()
        {
            var minimapTexture = DBContext.Instance["WMOMinimapTexture"];
            var listfile = ListFile.Instance;
            var guesstimator = WMO.WMOGuesstimator.Instance;

            int groupNum, blockX, blockY, fileDataID, wmoID;
            foreach (var rec in minimapTexture)
            {
                groupNum = rec.Value.FieldAs<int>("GroupNum");
                blockX = rec.Value.FieldAs<int>("BlockX");
                blockY = rec.Value.FieldAs<int>("BlockY");
                fileDataID = rec.Value.FieldAs<int>("FileDataID");
                wmoID = rec.Value.FieldAs<int>("WMOID");

                if (listfile.ContainsKey(fileDataID))
                    continue;

                // try to use a previous filename for the same WMOID
                if (!guesstimator.GetPreviousName(wmoID, out string filename))
                {
                    // lookup up the AreaTable then the WMOAreaTable
                    if (!guesstimator.GetAreaName(wmoID, out filename))
                        filename = "unknown"; // then ultimately, fallback to 'unknown'

                    FileNames[fileDataID] = $"world/minimaps/wmo/autogen-names/{filename}/{wmoID}_{groupNum:D3}_{blockX:D2}_{blockY:D2}.blp";
                }
                else
                {
                    FileNames[fileDataID] = $"{filename}_{groupNum:D3}_{blockX:D2}_{blockY:D2}.blp".Replace("world/wmo/", "world/minimaps/wmo/");
                }
            }
        }
    }
}
