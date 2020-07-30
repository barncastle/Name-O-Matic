using System;
using System.Collections.Generic;
using DBCD;
using DBCD.Providers;
using NameOMatic.Helpers;

namespace NameOMatic.Database
{
    internal class DBContext : Singleton<DBContext>
    {
        private readonly Dictionary<string, IDBCDStorage> DBSets;
        private readonly DBCD.DBCD DBCD;

        public DBContext()
        {
            DBSets = new Dictionary<string, IDBCDStorage>(StringComparer.OrdinalIgnoreCase);
            DBCD = new DBCD.DBCD(new CascDBCProvider(), new GithubDBDProvider());
        }

        public IDictionary<int, DBCDRow> this[string name]
        {
            get
            {
                if (!DBSets.TryGetValue(name, out var storage))
                    TryLoad(name, out storage);
                return storage;
            }
        }

        public bool TryLoad(string name, int fileId, out IDBCDStorage dbset)
        {
            if (!FileLookup.ContainsKey(name))
                FileLookup.Add(name, fileId);

            return TryLoad(name, out dbset);
        }

        public bool TryLoad(string name, out IDBCDStorage dbset)
        {
            if (DBSets.TryGetValue(name, out dbset))
                return dbset != null;

            if (FileLookup.TryGetValue(name, out int fileId))
            {
                if (FileContext.Instance.FileExists(fileId))
                {
                    DBSets.Add(name, dbset = DBCD.Load(name, FileContext.Instance.Build));
                    return true;
                }
            }

            DBSets.Add(name, dbset = null);
            return false;
        }

        public bool TryGet(string name, out IDBCDStorage storage) => DBSets.TryGetValue(name, out storage) && storage != null;

        public static readonly Dictionary<string, int> FileLookup = new Dictionary<string, int>
        {
            ["AreaTable"] = 1353545,
            ["CharSections"] = 1365366,
            ["ChrClasses"] = 1361031,
            ["ChrRaces"] = 1305311,
            ["ComponentModelFileData"] = 1349053,
            ["ComponentTextureFileData"] = 1278239,
            ["Creature"] = 841631,
            ["CreatureDisplayInfo"] = 1108759,
            ["CreatureDisplayInfoExtra"] = 1264997,
            ["CreatureModelData"] = 1365368,
            ["CreatureSoundData"] = 1344466,
            ["CreatureXDisplayInfo"] = 1864302,
            ["EmotesText"] = 1347273,
            ["EmotesTextSound"] = 1286524,
            ["GameObjectDisplayInfo"] = 1266277,
            ["GameObjectDisplayInfoXSoundKit"] = 1345272,
            ["GuildEmblem"] = 2734754,
            ["GuildShirtBackground"] = 2921008,
            ["GuildShirtBorder"] = 2921475,
            ["GuildTabardBackground"] = 2909769,
            ["GuildTabardBorder"] = 2920485,
            ["GuildTabardEmblem"] = 2910470,
            ["ItemDisplayInfo"] = 1266429,
            ["ItemDisplayInfoMaterialRes"] = 1280614,
            ["LightSkybox"] = 1308501,
            ["LiquidType"] = 1371380,
            ["LiquidTypeXTexture"] = 2261065,
            ["ManifestInterfaceData"] = 1375801,
            ["Map"] = 1349477,
            ["ModelFileData"] = 1337833,
            ["Movie"] = 1332556,
            ["MovieFileData"] = 1301154,
            ["MovieVariation"] = 1339819,
            ["NPCSounds"] = 1282621,
            ["SpellActivationOverlay"] = 1261603,
            ["SpellName"] = 1990283,
            ["SoundEmitters"] = 1092316,
            ["SoundKitEntry"] = 1237435,
            ["SoundKitName"] = 1665033,
            ["SpellVisualEffectName"] = 897948,
            ["SpellVisualKitAreaModel"] = 897951,
            ["TextureFileData"] = 982459,
            ["TactKey"] = 1302850,
            ["TactKeyLookup"] = 1302851,
            ["VocalUISounds"] = 1267067,
            ["WMOAreaTable"] = 1355528,
            ["WMOMinimapTexture"] = 1323241,
            ["ZoneMusic"] = 1310254,
        };
    }
}
