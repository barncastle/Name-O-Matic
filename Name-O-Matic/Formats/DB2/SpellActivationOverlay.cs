using System.Collections.Generic;
using System.Text.RegularExpressions;
using NameOMatic.Database;

namespace NameOMatic.Formats.DB2
{
    class SpellActivationOverlay : IDB2
    {
        public IDictionary<int, string> FileNames { get; }

        public bool IsValid => DBContext.Instance["SpellActivationOverlay"] != null && DBContext.Instance["SpellName"] != null;

        private readonly Regex NormaliseRegex;

        public SpellActivationOverlay()
        {
            FileNames = new Dictionary<int, string>();
            NormaliseRegex = new Regex("[^a-z0-9 -]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public void Enumerate()
        {
            var activationOverlay = DBContext.Instance["SpellActivationOverlay"];
            var spellName = DBContext.Instance["SpellName"];
            var listfile = ListFile.Instance;

            int spellID, fileDataID;
            foreach (var rec in activationOverlay)
            {
                fileDataID = rec.Value.FieldAs<int>("OverlayFileDataID");
                if (listfile.ContainsKey(fileDataID))
                    continue;

                spellID = rec.Value.FieldAs<int>("SpellID");

                string name = spellName[spellID].Field<string>("Name_lang");
                name = NormaliseRegex.Replace(name, "").Replace(" ", "_");

                FileNames[fileDataID] = $"textures/spellactivationoverlays/{name}.blp";
            }
        }
    }
}
