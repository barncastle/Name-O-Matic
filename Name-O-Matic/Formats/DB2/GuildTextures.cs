using System.Collections.Generic;
using NameOMatic.Constants;
using NameOMatic.Database;

namespace NameOMatic.Formats.DB2
{
    class GuildTextures : IDB2
    {
        public IDictionary<int, string> FileNames { get; }

        public bool IsValid => true;

        public GuildTextures() => FileNames = new Dictionary<int, string>();

        public void Enumerate()
        {
            EnumerateGuildEmblem();
            EnumerateGuildShirtBackground();
            EnumerateGuildShirtBorder();
            EnumerateGuildTabardBackground();
            EnumerateGuildTabardBorder();
            EnumerateGuildTabardEmblem();
        }


        private void EnumerateGuildEmblem()
        {
            var records = DBContext.Instance["GuildEmblem"];
            if (records == null)
                return;

            foreach (var rec in records)
            {
                int textureID = rec.Value.FieldAs<int>("TextureFileDataID");
                if (ListFile.Instance.ContainsKey(textureID))
                    continue;

                FileNames[textureID] = $"textures/guildemblems/emblem_{rec.Value["EmblemID"]}.blp";
            }
        }

        private void EnumerateGuildShirtBackground()
        {
            var records = DBContext.Instance["GuildShirtBackground"];
            if (records == null)
                return;

            foreach (var rec in records)
            {
                int textureID = rec.Value.FieldAs<int>("FileDataID");
                if (ListFile.Instance.ContainsKey(textureID))
                    continue;

                var component = (ComponentSection)rec.Value.FieldAs<int>("Component");
                var itemID = rec.Value.FieldAs<int>("ShirtID");
                var color = rec.Value.FieldAs<int>("Color");

                FileNames[textureID] = $"textures/guildemblems/shirt{itemID}_commonbg_{color:D2}_{component}_u.blp";
            }
        }

        private void EnumerateGuildTabardBorder()
        {
            var records = DBContext.Instance["GuildTabardBorder"];
            if (records == null)
                return;

            foreach (var rec in records)
            {
                int textureID = rec.Value.FieldAs<int>("FileDataID");
                if (ListFile.Instance.ContainsKey(textureID))
                    continue;

                var component = (ComponentSection)rec.Value.FieldAs<int>("Component");
                var color = rec.Value.FieldAs<int>("Color");
                var tier = (ComponentTier)rec.Value.FieldAs<int>("Tier");

                if (tier == ComponentTier.Common)
                    FileNames[textureID] = $"textures/guildemblems/border_{color:D2}_{component}_u.blp";
                else
                    FileNames[textureID] = $"textures/guildemblems/{tier}border_{color:D2}_{component}_u.blp";
            }
        }

        private void EnumerateGuildShirtBorder()
        {
            var records = DBContext.Instance["GuildShirtBorder"];
            if (records == null)
                return;

            foreach (var rec in records)
            {
                int textureID = rec.Value.FieldAs<int>("FileDataID");
                if (ListFile.Instance.ContainsKey(textureID))
                    continue;

                var component = (ComponentSection)rec.Value.FieldAs<int>("Component");
                var itemID = rec.Value.FieldAs<int>("ShirtID");
                var color = rec.Value.FieldAs<int>("Color");
                var tier = (ComponentTier)rec.Value.FieldAs<int>("Tier");

                FileNames[textureID] = $"textures/guildemblems/shirt{itemID}_{tier}border_{color:D2}_{component}_u.blp";
            }
        }

        private void EnumerateGuildTabardBackground()
        {
            var records = DBContext.Instance["GuildTabardBackground"];
            if (records == null)
                return;

            foreach (var rec in records)
            {
                int textureID = rec.Value.FieldAs<int>("FileDataID");
                if (ListFile.Instance.ContainsKey(textureID))
                    continue;

                var component = (ComponentSection)rec.Value.FieldAs<int>("Component");
                var color = rec.Value.FieldAs<int>("Color");
                var tier = (ComponentTier)rec.Value.FieldAs<int>("Tier");

                if (tier == ComponentTier.Common)
                    FileNames[textureID] = $"textures/guildemblems/background_{color:D2}_{component}_u.blp";
                else
                    FileNames[textureID] = $"textures/guildemblems/{tier}bg_{color:D2}_{component}_u.blp";
            }

        }

        private void EnumerateGuildTabardEmblem()
        {
            var records = DBContext.Instance["GuildTabardEmblem"];
            if (records == null)
                return;

            foreach (var rec in records)
            {
                int textureID = rec.Value.FieldAs<int>("FileDataID");
                if (ListFile.Instance.ContainsKey(textureID))
                    continue;

                var component = (ComponentSection)rec.Value.FieldAs<int>("Component");
                var itemID = rec.Value.FieldAs<int>("EmblemID");
                var color = rec.Value.FieldAs<int>("Color");

                FileNames[textureID] = $"textures/guildemblems/emblem_{itemID}_{color:D2}_{component}_u.blp";
            }
        }
    }
}
