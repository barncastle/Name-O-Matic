using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NameOMatic.Constants;
using NameOMatic.Database;

namespace NameOMatic.Formats.DB2
{
    class ItemDisplayInfo : IDB2
    {
        public IDictionary<int, string> FileNames { get; }

        public bool IsValid
        {
            get
            {
                return DBContext.Instance["ItemDisplayInfo"] != null &&
                       DBContext.Instance["TextureFileData"] != null &&
                       DBContext.Instance["ModelFileData"] != null;
            }
        }

        public ItemDisplayInfo() => FileNames = new ConcurrentDictionary<int, string>();

        public void Enumerate()
        {
            var itemDisplayInfo = DBContext.Instance["ItemDisplayInfo"];
            var textureFileData = DBContext.Instance["TextureFileData"];
            var modLookup = CreateLookup(DBContext.Instance["ModelFileData"], "ModelResourcesID");
            var matLookup = CreateLookup(textureFileData, "MaterialResourcesID");
            var listfile = ListFile.Instance;

            var temp = itemDisplayInfo.SelectMany(idi =>
            {
                var modelResources = idi.Value.FieldAs<int[]>("ModelResourcesID").Where(i => modLookup.ContainsKey(i));
                var materialResources = idi.Value.FieldAs<int[]>("ModelMaterialResourcesID").Where(i => matLookup.ContainsKey(i));

                return modelResources.Zip(materialResources, (model, mat) => new
                {
                    ModelFileIDs = modLookup[model],
                    TextureFileIDs = matLookup[mat]
                });
            })
            .Where(x =>
            {
                return !x.TextureFileIDs.All(y => listfile.ContainsKey(y));
            });

            Parallel.ForEach(temp, rec =>
            {
                var modelNames = GetModelFileNames(rec.ModelFileIDs).ToArray();

                string directoryAndPath = GetLongestCommonFileName(modelNames);
                if (directoryAndPath == "")
                    return;

                foreach (var texture in rec.TextureFileIDs)
                {
                    string suffix = TextureSuffix.Get(textureFileData[texture].FieldAs<int>("UsageType"));
                    FileNames[texture] = $"{directoryAndPath}_{texture}{suffix}.blp";
                }
            });
        }

        private IEnumerable<string> GetModelFileNames(int[] modelIds)
        {
            for (int i = 0; i < modelIds.Length; i++)
                if (ListFile.Instance.TryGetValue(modelIds[i], out string filename))
                    yield return filename;
        }

        private string GetLongestCommonFileName(string[] modelNames)
        {
            if (modelNames.Length == 0)
                return "";
            if (modelNames.Length == 1)
                return Path.ChangeExtension(modelNames[0], null);

            // extract the directory name of the first model
            string path = Path.GetDirectoryName(modelNames[0]);
            string[][] parts = new string[modelNames.Length][];

            // split the filenames by "_" blizzard's most common seperator
            int minlen = byte.MaxValue;
            for (int i = 0; i < modelNames.Length; i++)
            {
                // validate everything uses the same directory
                if (!modelNames[i].StartsWith(path, StringComparison.OrdinalIgnoreCase))
                    return "";

                parts[i] = modelNames[i].ToUpperInvariant().Split('_');
                minlen = Math.Min(minlen, parts[i].Length);
            }

            // find the lcs across all model names
            for (int i = 0; i < minlen; i++)
                for (int j = 0; j < parts.Length; j++)
                    if (parts[0][i] != parts[j][i])
                        return Path.Combine(path, string.Join('_', parts[0], 0, i));

            // fallback to the first model if all names are identical - this is impossible
            return Path.ChangeExtension(modelNames[0], null);
        }

        private Dictionary<int, int[]> CreateLookup(IDictionary<int, DBCD.DBCDRow> source, string key)
        {
            return source.GroupBy(x => x.Value.FieldAs<int>(key)).ToDictionary(x => x.Key, x => x.Select(y => y.Key).ToArray());
        }
    }
}
