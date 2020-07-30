using System;
using System.Collections.Generic;
using System.Linq;
using NameOMatic.Database;
using NameOMatic.Database.Lookups;

namespace NameOMatic.Formats.M2
{
    class M2Guesstimator
    {
        private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;
        private readonly PrefixLookup PrefixMap;

        private readonly HashSet<int> SkyboxIDs;
        private readonly HashSet<int> CelestialSkyboxIDs;
        private readonly HashSet<int> SpellVisualKitAreaModelIDs;
        private readonly HashSet<int> CreatureModelDataIDs;
        private readonly HashSet<string> ChrRaces;

        public M2Guesstimator()
        {
            PrefixMap = new PrefixLookup();
            PrefixMap.Populate();

            SkyboxIDs = CreateLookup("LightSkybox", "SkyboxFileDataID");
            CelestialSkyboxIDs = CreateLookup("LightSkybox", "CelestialSkyboxFileDataID");
            SpellVisualKitAreaModelIDs = CreateLookup("SpellVisualKitAreaModel", "ModelFileDataID");
            CreatureModelDataIDs = CreateLookup("CreatureModelData", "FileDataID");
            ChrRaces = DBContext.Instance["ChrRaces"].Select(x => x.Value.Field<string>("ClientFileString")).ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        public void Analyse(M2Model model)
        {
            if (IsCharacter(model))
                return;
            if (IsSky(model))
                return;
            if (IsSpell(model))
                return;
            if (IsTradeSkillNode(model))
                return;
            if (HasPrefix(model))
                return;
            if (IsCreature(model))
                return;

            Console.WriteLine($"Directory not found :: {model.FileDataId} {model.InternalName}");
        }

        public void Dispose()
        {
            SkyboxIDs.Clear();
            CelestialSkyboxIDs.Clear();
            SpellVisualKitAreaModelIDs.Clear();
            CreatureModelDataIDs.Clear();
        }


        private bool IsCharacter(M2Model model)
        {
            string name = model.InternalName;

            if (name.EndsWith("male", IgnoreCase))
            {
                string gender = name.EndsWith("female", IgnoreCase) ? "female" : "male";

                string race = name.Substring(0, name.Length - gender.Length);
                if (ChrRaces.Contains(race))
                {
                    model.Directory = $"character/{race}/{gender}/";
                    return true;
                }
            }

            return false;
        }

        private bool IsSky(M2Model model)
        {
            if (SkyboxIDs.Contains(model.FileDataId) || 
                CelestialSkyboxIDs.Contains(model.FileDataId) ||
                model.InternalName.EndsWith("sky01", StringComparison.OrdinalIgnoreCase))
            {
                model.Directory = "environments/stars/";
                return true;
            }

            return false;
        }

        private bool IsSpell(M2Model model)
        {
            if (SpellVisualKitAreaModelIDs.Contains(model.FileDataId) /*|| Database.SpellVisualEffectName.Contains(model.FileDataId)*/)
            {
                model.Directory = "spells/";
                return true;
            }

            if (model.InternalName.IndexOf("_arenaflagr", IgnoreCase) > -1)
            {
                model.Directory = "spells/";
                return true;
            }

            if (model.InternalName.StartsWith("SpellVisualPlaceholder", IgnoreCase))
            {
                model.Suffix = "_" + model.FileDataId;
                model.Directory = "spells/";
                return true;
            }

            return false;
        }

        private bool IsTradeSkillNode(M2Model model)
        {
            if (model.InternalName.IndexOf("HerbNode", IgnoreCase) > -1 ||
               model.InternalName.IndexOf("MiningNode", IgnoreCase) > -1)
            {
                model.Directory = "world/skillactivated/tradeskillnodes/";
                return true;
            }

            return false;
        }

        private bool IsCreature(M2Model model)
        {
            if (CreatureModelDataIDs.Contains(model.FileDataId))
            {
                model.Directory = $"creature/{model.InternalName}/";
                return true;
            }

            return false;
        }

        private bool HasPrefix(M2Model model)
        {
            string[] parts = model.InternalName.Split('_', PrefixMap.MaxTokenCount);

            string prefix;
            for (int i = parts.Length; i > 0; i--)
            {
                prefix = string.Join('_', parts, 0, i);
                if (PrefixMap.TryGetValue(prefix, out string dir))
                {
                    model.Directory = dir;
                    return true;
                }
            }

            // fallback for number prefixed models
            if(parts.Length == 1 && char.IsDigit(parts[0][0]))
            {
                foreach(var map in PrefixMap)
                {
                    if(model.InternalName.StartsWith(map.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        model.Directory = map.Value;
                        return true;
                    }
                }
            }

            return false;
        }

        private HashSet<int> CreateLookup(string database, string key)
        {
            return DBContext.Instance[database].Values.Select(x => x.FieldAs<int>(key)).ToHashSet();
        }
    }
}
