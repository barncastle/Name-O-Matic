using System.Collections.Generic;
using System.Linq;
using NameOMatic.Database;
using NameOMatic.Extensions;

namespace NameOMatic.Formats.ADTDAT
{
    internal class ADTDATModel : IModel
    {
        public IDictionary<int, string> FileNames { get; private set; }

        public readonly int FileDataId;

        public ACNKHeader[] Headers { get; }

        public ADTDATModel(int fileDataId)
        {
            FileDataId = fileDataId;
            Headers = new ACNKHeader[256];
            FileNames = new Dictionary<int, string>();
        }

        public void GenerateFileNames()
        {
            var areatable = DBContext.Instance["AreaTable"];
            var map = DBContext.Instance["Map"];

            if (areatable == null || map == null)
                return;

            // get most common non-zero area id
            var areaId = Headers.Select(x => x.AreaId).Where(x => x > 0).MostCommon();

            if (areatable.TryGetValue(areaId, out var areaRec))
                if (map.TryGetValue(areaRec.FieldAs<int>("ContinentID"), out var mapRec))
                    FileNames[FileDataId] = $"world/maps/{mapRec["Directory"]}/{FileDataId}.unk";
        }
    }
}
