using System.Collections.Generic;
using NameOMatic.Database;

namespace NameOMatic.Formats.DB2
{
    internal class Movie : IDB2
    {
        public IDictionary<int, string> FileNames { get; }

        public bool IsValid
        {
            get
            {
                return DBContext.Instance["MovieFileData"] != null &&
                       DBContext.Instance["MovieVariation"] != null &&
                       DBContext.Instance["Movie"] != null;
            }
        }

        public Movie() => FileNames = new Dictionary<int, string>();

        public void Enumerate()
        {
            var movieFileData = DBContext.Instance["MovieFileData"];
            var movieVariation = DBContext.Instance["MovieVariation"];
            var movie = DBContext.Instance["Movie"];
            var listfile = ListFile.Instance;

            var build = FileContext.Instance.FormattedBuild;
            var expansion = FileContext.Instance.Expansion;

            foreach (var rec in movieVariation)
            {
                var fileDataID = rec.Value.FieldAs<int>("FileDataID");
                var movieID = rec.Value.FieldAs<int>("MovieID");
                var resolution = 0;

                if (movieFileData.ContainsKey(fileDataID))
                    resolution = movieFileData[fileDataID].FieldAs<int>("Resolution");

                if (listfile.ContainsKey(fileDataID))
                    continue;

                FileNames[fileDataID] = $"interface/cinematics/{expansion}_{build}_{movieID}_{resolution}.avi";

                // preferred resolution
                if (resolution == 1280)
                {
                    var audioID = movie[movieID].FieldAs<int>("AudioFileDataID");
                    var subtitleID = movie[movieID].FieldAs<int>("SubtitleFileDataID");

                    if (!listfile.ContainsKey(audioID))
                        FileNames[audioID] = $"interface/cinematics/{expansion}_{build}_{movieID}.mp3";
                    if (!listfile.ContainsKey(subtitleID))
                        FileNames[audioID] = $"interface/cinematics/{expansion}_{build}_{movieID}.sbt";
                }
            }
        }
    }
}
