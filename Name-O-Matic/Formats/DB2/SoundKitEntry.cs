using System.Collections.Generic;
using System.IO;
using System.Linq;
using DBCD;
using NameOMatic.Constants;
using NameOMatic.Database;

namespace NameOMatic.Formats.DB2
{
    /// <summary>
    /// This is the "much-worse" fallback for SoundKitName (RIP 2019)
    /// </summary>
    partial class SoundKitEntry : IDB2
    {
        public IDictionary<int, string> FileNames { get; }

        public bool IsValid => DBContext.Instance["SoundKitName"] == null || DBContext.Instance["ManifestMP3"] == null;

        private readonly SoundGlobalIndexer GlobalIndex;
        private readonly HashSet<int> MP3FileIds;
        private readonly Dictionary<int, string> Races;
        private readonly Dictionary<int, string> Classes;
        private readonly IDBCDStorage CreatureSoundData;
        private readonly IDBCDStorage NPCSounds;

        public SoundKitEntry()
        {
            FileNames = new Dictionary<int, string>();

            GlobalIndex = SoundGlobalIndexer.Instance;
            MP3FileIds = DBContext.Instance["ManifestMP3"].Keys.ToHashSet();
            Races = DBContext.Instance["ChrRaces"].ToDictionary(x => x.Key, x => x.Value.Field<string>("ClientFileString"));
            Classes = DBContext.Instance["ChrClasses"].ToDictionary(x => x.Key, x => x.Value.Field<string>("Filename"));
            CreatureSoundData = DBContext.Instance["CreatureSoundData"] as IDBCDStorage;
            NPCSounds = DBContext.Instance["NPCSounds"] as IDBCDStorage;
        }

        public void Enumerate()
        {
            GenerateCreatureSounds();
            GenerateEmotesTextSound();
            GenerateVocalUISounds();
            GenerateEmitterSounds();
            GenerateGameObjectSounds();
            GenerateUIPartyPose();
            GenerateZoneIntroMusicTable();
        }

        private void GenerateEmitterSounds()
        {
            var emitters = DBContext.Instance["SoundEmitters"];

            if (emitters == null)
                return;

            foreach (var rec in emitters)
            {
                var name = rec.Value.Field<string>("Name");
                var soundKitId = rec.Value.FieldAs<int>("SoundEntriesID");

                FormatTemplate($"sound/emitters/{name}{{0}}.ogg", soundKitId);
            }
        }

        private void GenerateGameObjectSounds()
        {
            var goDisplayInfo = DBContext.Instance["GameObjectDisplayInfo"];
            var goXsoundKit = DBContext.Instance["GameObjectDisplayInfoXSoundKit"];
            var listfile = ListFile.Instance;

            if (goDisplayInfo == null || goXsoundKit == null)
                return;

            foreach (var rec in goXsoundKit)
            {
                int soundKitId = rec.Value.FieldAs<int>("SoundKitID");
                int displayInfoId = rec.Value.FieldAs<int>("GameObjectDisplayInfoID");
                int eventId = rec.Value.FieldAs<int>("EventIndex");
                int modelId = goDisplayInfo[displayInfoId].FieldAs<int>("FileDataID");

                if (!listfile.TryGetValue(modelId, out string modelName))
                    continue;

                modelName = Path.GetFileNameWithoutExtension(modelName);

                FormatTemplate($"sound/doodads/autogen-names/go_{modelName}_event{eventId:D2}{{0}}.ogg", soundKitId);
            }
        }

        private void GenerateVocalUISounds()
        {
            var vocalSounds = DBContext.Instance["VocalUISounds"];
            string build = FileContext.Instance.FormattedBuild;

            if (vocalSounds == null)
                return;

            foreach (var rec in vocalSounds)
            {
                var soundIds = rec.Value.FieldAs<int[]>("NormalSoundID");
                var raceID = rec.Value.FieldAs<int>("RaceID");
                var classID = rec.Value.FieldAs<int>("ClassID");
                var vocalenum = (VocalUISound)rec.Value.FieldAs<int>("VocalUIEnum");

                if (!Races.TryGetValue(raceID, out string raceName))
                    continue;
                if (Classes.TryGetValue(classID, out string className))
                    className = "_" + className;

                FormatTemplate($"sound/character/pc{className}_{raceName}_male/vo_{build}_pc{className}_{raceName}_male_err_{vocalenum}{{0}}.ogg", soundIds[0]);
                FormatTemplate($"sound/character/pc{className}_{raceName}_female/vo_{build}_pc{className}_{raceName}_female_err_{vocalenum}{{0}}.ogg", soundIds[1]);
            }
        }

        private void GenerateEmotesTextSound()
        {
            var emotesSound = DBContext.Instance["EmotesTextSound"];
            var emotesText = DBContext.Instance["EmotesText"]?.ToDictionary(x => x.Key, x => x.Value.Field<string>("Name"));
            string build = FileContext.Instance.FormattedBuild;

            if (emotesSound == null || emotesText == null)
                return;

            foreach (var rec in emotesSound)
            {
                var soundId = rec.Value.FieldAs<int>("SoundID");
                var raceID = rec.Value.FieldAs<int>("RaceID");
                var classID = rec.Value.FieldAs<int>("ClassID");
                var gender = rec.Value.FieldAs<int>("SexID") == 0 ? "male" : "female";
                var text = emotesText[rec.Value.FieldAs<int>("EmotesTextID")];

                if (!Races.TryGetValue(raceID, out string raceName))
                    continue;
                if (Classes.TryGetValue(classID, out string className))
                    className = "_" + className;

                FormatTemplate($"sound/character/pc{className}_{raceName}_{gender}/vo_{build}_pc{className}_{raceName}_{gender}_{text}{{0}}.ogg", soundId);
            }
        }

        private void GenerateUIPartyPose()
        {
            var partypose = DBContext.Instance["UiPartyPose"];
            var map = DBContext.Instance["Map"]?.ToDictionary(x => x.Key, x => x.Value.Field<string>("Directory"));
            var build = FileContext.Instance.FormattedBuild;
            var expansion = (ExpansionFull)FileContext.Instance.Expansion;

            if (partypose == null || map == null)
                return;

            foreach (var rec in partypose)
            {
                var directory = map[rec.Value.FieldAs<int>("MapID")];
                var victoryId = rec.Value.FieldAs<int>("VictorySoundKitID");
                var defeatId = rec.Value.FieldAs<int>("DefeatSoundKitID");

                FormatTemplate($"sound/music/{expansion}/mus_{build}_{directory}_victory.mp3", victoryId);
                FormatTemplate($"sound/music/{expansion}/mus_{build}_{directory}_defeat.mp3", defeatId);
            }
        }

        private void GenerateZoneIntroMusicTable()
        {
            var introMusic = DBContext.Instance["ZoneIntroMusicTable"];
            var expansion = (ExpansionFull)FileContext.Instance.Expansion;

            if (introMusic == null)
                return;

            foreach (var rec in introMusic)
            {
                var name = rec.Value.Field<string>("Name");
                var soundKitId = rec.Value.FieldAs<int>("SoundID");

                FormatTemplate($"sound/music/{expansion}/mus_{name}.mp3", soundKitId);
            }
        }

        private void FormatTemplate(string template, int soundKitID)
        {
            if (GlobalIndex.TryGetValue(soundKitID, out var fileIds))
            {
                for (int i = 0; i < fileIds.Count; i++)
                {
                    if (!ListFile.Instance.ContainsKey(fileIds[i]) && !FileNames.ContainsKey(fileIds[i]) && fileIds[i] > 1730233)
                    {
                        FileNames[fileIds[i]] = string.Format(template, $"_{i + 1:D2}");

                        if (MP3FileIds.Contains(fileIds[i]))
                            FileNames[fileIds[i]] = Path.ChangeExtension(FileNames[fileIds[i]], ".mp3");
                    }
                }
            }
        }
    }
}
