using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NameOMatic.Database;

namespace NameOMatic.Formats.DB2
{
    class CreatureDisplayInfo : IDB2
    {
        public IDictionary<int, string> FileNames { get; }

        public bool IsValid => DBContext.Instance["CreatureModelData"] != null && DBContext.Instance["CreatureDisplayInfo"] != null;

        private readonly BLP.BLPGuesstimator BLPGuesstimator;

        public CreatureDisplayInfo()
        {
            FileNames = new ConcurrentDictionary<int, string>();
            BLPGuesstimator = new BLP.BLPGuesstimator();
        }

        public void Enumerate()
        {
            var creatureModelData = DBContext.Instance["CreatureModelData"];
            var creatureDisplayInfo = DBContext.Instance["CreatureDisplayInfo"];
            var listfile = ListFile.Instance;

            // (creatureModelData.ID, creatureModelData.FileDataID)
            var cmd = creatureModelData.ToDictionary(x => x.Key, x => x.Value.FieldAs<int>("FileDataID"));

            /*
             * get { creatureDisplayInfo.ModelId, creatureDisplayInfo.TextureId[] }
             * ignore records creatureDisplayInfo.ModelId -/-> creatureModelData.ModelId
             * ignore records listfile.contains(creatureModelData.FileDataID) :: already named
             * explode creatureDisplayInfo.TextureId[]
             * select { creatureDisplayInfo.TextureId, creatureModelData.FileName }
             * ignore records creatureDisplayInfo.TextureId == 0 || 
             *                !listfile.contains(creatureDisplayInfo.TextureId)
             */

            var temp = creatureDisplayInfo.Select(c => new
            {
                ModelId = c.Value.FieldAs<int>("ModelID"),
                TextureIds = c.Value.FieldAs<int[]>("TextureVariationFileDataID").Where(i => !listfile.ContainsKey(i))
            })
            .Where(x =>
            {
                return cmd.ContainsKey(x.ModelId) && listfile.ContainsKey(cmd[x.ModelId]);
            })
            .SelectMany(c =>
            {
                return c.TextureIds.Select(ti => new
                {
                    Texture = ti,
                    ModelName = listfile[cmd[c.ModelId]]
                });
            });

            Parallel.ForEach(temp, rec => FileNames[rec.Texture] = BLPGuesstimator.Guess(rec.Texture, Path.ChangeExtension(rec.ModelName, null)));
        }
    }
}
