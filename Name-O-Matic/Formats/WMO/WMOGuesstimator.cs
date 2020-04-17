using System;
using System.Collections.Generic;
using System.Linq;
using NameOMatic.Database;
using NameOMatic.Extensions;
using NameOMatic.Helpers;

namespace NameOMatic.Formats.WMO
{
    using WMOAreaMap = Dictionary<int, (string AreaName, int AreaTableID)>;

    class WMOGuesstimator : Singleton<WMOGuesstimator>
    {
        private Dictionary<int, string> WMONameMap;
        private WMOAreaMap WMOAreaMap;

        public WMOGuesstimator()
        {
            WMONameMap = new Dictionary<int, string>(0x1000);
            BuildLookups();
        }


        public bool GetPreviousName(int wmoid, out string value) => WMONameMap.TryGetValue(wmoid, out value);

        public bool GetAreaName(int wmoid, out string value)
        {
            if (WMOAreaMap.TryGetValue(wmoid, out var rec))
            {
                // try areatable
                if (DBContext.Instance["AreaTable"].TryGetValue(rec.AreaTableID, out var row))
                    if (!string.IsNullOrEmpty(value = row.Field<string>("ZoneName")))
                        return true;

                // try wmoareatable
                return !string.IsNullOrEmpty(value = rec.AreaName);
            }

            value = "";
            return false;
        }


        private void BuildLookups()
        {
            var listfile = ListFile.Instance;

            // (WMOID, known MinimapTexture filename)
            WMONameMap = DBContext.Instance["WMOMinimapTexture"].GroupBy(x => x.Value.FieldAs<int>("WMOID")).Select(x =>
            {
                foreach (var rec in x)
                {
                    int fid = rec.Value.FieldAs<int>("FileDataID");
                    if (fid > 0 && listfile.TryGetValue(fid, out string filename))
                        return new { WMOID = x.Key, FileName = filename[0..^14] };
                }

                return new { WMOID = 0, FileName = "" };
            })
            .Where(x => x.WMOID > 0)
            .ToDictionary(x => x.WMOID, x => x.FileName);

            // [WMOID, (WMOAreaTable->AreaName, AreaTable->ID)]
            WMOAreaMap = DBContext.Instance["WMOAreaTable"].GroupBy(x => x.Value.FieldAs<int>("WMOID")).ToDictionary(x => x.Key, x =>
            {
                return
                (
                    AreaName: x.Select(y => y.Value.Field<string>("AreaName_lang")).Where(y => y == "").MostCommon(comparer: StringComparer.OrdinalIgnoreCase),
                    AreaTableID: x.Select(y => y.Value.FieldAs<int>("AreaTableID")).Where(y => y > 0).MostCommon()
                );
            });
        }
    }
}
