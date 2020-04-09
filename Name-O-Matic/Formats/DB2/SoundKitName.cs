using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NameOMatic.Database;

namespace NameOMatic.Formats.DB2
{
    class SoundKitName : IDB2
    {
        public IDictionary<int, string> FileNames { get; }

        public bool IsValid
        {
            get
            {
                return DBContext.Instance["SoundKitName"] != null &&
                       DBContext.Instance["SoundEmitters"] != null &&
                       DBContext.Instance["SoundKitEntry"] != null;
            }
        }

        private readonly Regex VO_MaleFemale; // VO_801_Patch_01_M => sound/creature/patch/vo_801_patch_01_m.ogg
        private readonly Regex VO_Action; // VO_801_PATCH_WOUND => sound/creature/patch/vo_801_patch_wound_01.ogg
        private readonly Regex MON_Action; // Mon_DemonFly_Attack => sound/creature/demonfly/mon_demon_fly_attack_05.ogg
        private readonly Regex MUS_BfA; // MUS_80_Nazmir_Void => sound/music/battleforazeroth/mus_80_nazmir_void_03.ogg

        private readonly SoundGlobalIndexer GlobalIndex;

        public SoundKitName()
        {
            FileNames = new ConcurrentDictionary<int, string>();
            GlobalIndex = SoundGlobalIndexer.Instance;

            VO_MaleFemale = new Regex(@"^VO_\d{2,3}_(.*)_\d{1,2}_[mf]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            VO_Action = new Regex(@"^VO_\d{2,3}_(.*_([a-z]+))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MON_Action = new Regex(@"^MON_(.*_([a-z]+))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MUS_BfA = new Regex(@"^MUS_8[0-9.]{1,3}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public void Enumerate()
        {
            var soundkitname = DBContext.Instance["SoundKitName"];
            var soundKitEntries = DBContext.Instance["SoundKitEntry"];
            var soundEmitters = DBContext.Instance["SoundEmitters"].Values.Select(x => x.FieldAs<int>("SoundEntriesID")).ToHashSet();
            var listfile = ListFile.Instance;

            Parallel.ForEach(soundKitEntries, rec =>
            {
                int fileID = rec.Value.FieldAs<int>("SoundKitID");
                int skitID = rec.Value.FieldAs<int>("FileDataID");
                Match match;

                if (listfile.ContainsKey(fileID) || FileNames.ContainsKey(fileID))
                    return;
                if (!soundkitname.ContainsKey(skitID))
                    return;

                string soundname = soundkitname[skitID]["Name"].ToString().ToLowerInvariant();

                // SoundEmitter.db2
                if (soundEmitters.Contains(skitID))
                {
                    FormatTemplate("sound/emitters/{0}.ogg", skitID, fileID, soundname);
                }
                // VO_801_CreatureName_01_M
                else if ((match = VO_MaleFemale.Match(soundname)).Success)
                {
                    FormatTemplate($"sound/creature/{match.Groups[1].Value}/{soundname}.ogg", skitID, fileID, soundname);
                }
                // VO_801_CreatureName_WOUND || Mon_CreatureName_Attack
                else if ((match = VO_Action.Match(soundname)).Success || (match = MON_Action.Match(soundname)).Success)
                {
                    string directory = match.Groups[1].Value.ToLowerInvariant();

                    var suffix = Suffices.FirstOrDefault(x => directory.EndsWith(x));
                    if (string.IsNullOrEmpty(suffix))
                        return;

                    directory = directory.Substring(0, directory.IndexOf(suffix));
                    FormatTemplate($"sound/creature/{directory}/{{0}}.ogg", skitID, fileID, soundname);
                }
                // MUS_80_Nazmir_Void
                else if ((match = MUS_BfA.Match(soundname)).Success)
                {
                    FormatTemplate("sound/music/battleforazeroth{0}.mp3", skitID, fileID, soundname);
                }
                // StartsWith(prefix)
                else
                {
                    foreach (var template in PreffixLookup)
                    {
                        if (soundname.StartsWith(template.Key))
                        {
                            FormatTemplate(template.Value, skitID, fileID, soundname);
                            break;
                        }
                    }
                }
            });
        }

        private void FormatTemplate(string template, int soundKitID, int fileDataID, string soundname)
        {
            if (GlobalIndex[soundKitID].Count <= 1)
            {
                FileNames[fileDataID] = string.Format(template, soundname);
            }
            else
            {
                // get the filedataid position in the soundkit group
                int index = GlobalIndex[soundKitID].IndexOf(fileDataID);
                if (index > -1)
                    FileNames[fileDataID] = string.Format(template, $"{soundname}_{index + 1:D2}");
            }
        }

        private static readonly Dictionary<string, string> PreffixLookup = new Dictionary<string, string>()
        {
            ["fx_"] = "sound/spells/{0}.ogg",
            ["spell_"] = "sound/spells/{0}.ogg",
            ["go_"] = "sound/doodad/{0}.ogg",
            ["event_dmf_"] = "sound/event/dmf/{0}.ogg",
            ["event_"] = "sound/event/{0}.ogg",
            ["ui_"] = "sound/interface/{0}.ogg",
        };

        /// <summary>
        /// Common creature animation sound suffices
        /// </summary>
        private static readonly string[] Suffices = new[]
        {
            "_aggro", "_attack", "_attackcrit", "_attackcrit_ranged",
            "_attack_ranged", "_battlecry", "_battleroar", "_battleshout",
            "_battle_cry", "_battle_roar", "_battle_shout", "_charge",
            "_clickable", "_clickables", "_death", "_emerge", "_farewells",
            "_greetings", "_idle", "_idle_loop", "_injury", "_laugh", "_loop",
            "_pissed", "_preaggro", "_stand", "_wound", "_woundcrit", "_wound_crit",
            "_run", "_eat", "_mount", "_mountspecial",
        };

    }
}
