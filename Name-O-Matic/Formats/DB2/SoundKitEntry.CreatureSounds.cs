using System;
using System.Collections.Generic;
using NameOMatic.Database;
using NameOMatic.Database.Lookups;

namespace NameOMatic.Formats.DB2
{
    /// <summary>
    /// Split out as it is huge
    /// </summary>
    partial class SoundKitEntry
    {
        private CreatureNameLookup CreatureNameLookup;
        private IDictionary<string, string> CreatureSoundTypes;

        private void GenerateCreatureSounds()
        {
            var displayinfo = DBContext.Instance["CreatureDisplayInfo"];
            var modelData = DBContext.Instance["CreatureModelData"];
            var listfile = ListFile.Instance;
            var unmappable = new List<int>(0x1000);

            GetCreatureNameLookup();
            CreatureSoundTypes = GetCreatureSoundLookup();

            if (CreatureSoundData == null || NPCSounds == null || displayinfo == null || modelData == null)
                return;

            foreach (var rec in displayinfo)
            {
                modelData.TryGetValue(rec.Value.FieldAs<int>("ModelID"), out var modelRec);

                if (!CreatureNameLookup.TryGetValue(rec.Key, out string name))
                {
                    // TODO we really need a new lookup for creature display ids not in the Creature table as model names are too generic
                    // I guess we could scrape these from wowhead/mmoc/the client??
                    if (modelRec != null && listfile.TryGetValue(modelRec.FieldAs<int>("FileDataID"), out string modelFile) && modelFile.StartsWith("creature"))
                    {
                        // convert the model filename to a creature sound filename  
                        var parts = modelFile.Split('/');
                        name = parts[parts.Length - 2];
                    }
                    else
                    {
                        // record new creatures as of legion that are missing a join
                        if (rec.Key > 83274) unmappable.Add(rec.Key);
                        continue;
                    }
                }

                string basepath = $"sound/creature/{name}/mon_{name}".Replace(" ", "_");
                ParseCreatureSoundData(rec.Value.FieldAs<int>("SoundID"), basepath); // CreatureDisplayInfo.SoundID
                ParseCreatureSoundData(modelRec?.FieldAs<int>("SoundID"), basepath); // CreatureModelData.SoundID
                ParseNPCSounds(rec.Value.FieldAs<int>("NPCSoundID"), basepath);      // CreatureDisplayInfo.NPCSoundID
            }

            Console.WriteLine($"\t\tNote: {unmappable.Count} CreatureDisplayIds were NOT mapped");
        }

        #region Creature Sound Helpers

        private void ParseCreatureSoundData(int? id, string basepath)
        {
            if (!CreatureSoundData.TryGetValue(id.GetValueOrDefault(), out var rec))
                return;

            ParseNPCSounds(rec.FieldAs<int>("NPCSoundID"), basepath); // CreatureSoundData.NPCSoundID

            // apply the simple action names
            foreach (var column in CreatureSoundTypes)
            {
                var soundkitID = rec.FieldAs<int>(column.Key);
                if (soundkitID > 0)
                    FormatTemplate($"{basepath}_{column.Value}{{0}}_{soundkitID}.ogg", soundkitID);
            }

            // iterate fidget
            // TODO are these differently named depending on index?
            var soundkitIDs = rec.FieldAs<int[]>("SoundFidget");
            for (int i = 0; i < soundkitIDs.Length; i++)
                if (soundkitIDs[i] > 0)
                    FormatTemplate($"{basepath}_fidget{i + 1:D2}{{0}}_{soundkitIDs[i]}.ogg", soundkitIDs[i]);

            // iterate custom attacks
            // TODO are these differently named depending on index?
            soundkitIDs = rec.FieldAs<int[]>("CustomAttack");
            for (int i = 0; i < soundkitIDs.Length; i++)
                if (soundkitIDs[i] > 0)
                    FormatTemplate($"{basepath}_customattack{i + 1:D2}{{0}}_{soundkitIDs[i]}.ogg", soundkitIDs[i]);
        }

        private void ParseNPCSounds(int id, string basepath)
        {
            if (!NPCSounds.TryGetValue(id, out var rec))
                return;

            var soundIds = rec.FieldAs<int[]>("SoundID");
            FormatTemplate($"{basepath}_greetings{{0}}_{soundIds[0]}.ogg", soundIds[0]);
            FormatTemplate($"{basepath}_farewells{{0}}_{soundIds[1]}.ogg", soundIds[1]);
            FormatTemplate($"{basepath}_pissed{{0}}_{soundIds[2]}.ogg", soundIds[2]);
            //FormatTemplate($"{basepath}_?{{0}}.ogg", soundIds[3]); // always 0?
        }

        private IDictionary<string, string> GetCreatureSoundLookup()
        {
            if (CreatureSoundData == null)
                return null;

            var results = new Dictionary<string, string>(CreatureSoundData.AvailableColumns.Length);

            foreach (var column in CreatureSoundData.AvailableColumns)
            {
                // skip non sounds
                if (!column.Contains("Sound"))
                    continue;
                // skip arrays/not SoundKit FKs
                if (column == "NPCSoundID" || column == "SoundFidget" || column == "CreatureSoundDataIDPet")
                    continue;

                // format a rough action name from the column
                string action = column.Replace("Sound", "")
                               .Replace("ID", "")
                               .Replace("Exertion", "Attack")
                               .Replace("Injury", "Wound");

                results.Add(column, action);
            }

            return results;
        }

        private void GetCreatureNameLookup()
        {
            CreatureNameLookup = new CreatureNameLookup();

            var creature = DBContext.Instance["Creature"];
            var displayInfo = DBContext.Instance["CreatureXDisplayInfo"];

            if (creature == null || displayInfo == null)
                return;

            foreach (var rec in displayInfo)
            {
                int cdiID = rec.Value.FieldAs<int>("CreatureDisplayInfoID");
                int cID = rec.Value.FieldAs<int>("CreatureID");

                if (!CreatureNameLookup.ContainsKey(cdiID) && creature.TryGetValue(cID, out var cRow))
                    CreatureNameLookup.Add(cdiID, cRow.Field<string>("Name_lang"));
            }
        }

        #endregion
    }
}
