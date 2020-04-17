using System;
using System.Linq;
using System.Text.RegularExpressions;
using NameOMatic.Database;
using NameOMatic.Extensions;

namespace NameOMatic.Formats.BLP
{
    class BLPGuesstimator
    {
        public readonly Regex FileIdSuffix;

        private static readonly char[] Seperators = { '_', '-' };

        private readonly ListFile _listfile;
        private readonly FileContext _cascontext;
        private readonly Regex UnderscoreReplaceRegex;

        public BLPGuesstimator()
        {
            FileIdSuffix = new Regex(@"\d{7}\.blp$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            UnderscoreReplaceRegex = new Regex("_+", RegexOptions.Compiled);

            _listfile = ListFile.Instance;
            _cascontext = FileContext.Instance;
        }

        public string Guess(int fileId, string basepath)
        {
            // formatting
            basepath = basepath.TrimEnd(Seperators);

            if (basepath[^1] == '/')
                return null;

            var matches = _cascontext.GetMatchingFiles(fileId).Where(x => IsApplicable(x));
            if (!matches.Any())
                return $"{basepath}_{fileId}.blp";

            // get matching chashes that are named and not suffixed with a fid
            // group them by their name - taking into account names that contain the directory
            // then return the most common occurrence
            var filename = matches.Distinct()
                                  .Select(x => FormatName(_listfile[x], basepath))
                                  .MostCommon(StringComparer.OrdinalIgnoreCase);

            // check it isn't a duplicate and isn't empty
            if (string.IsNullOrEmpty(filename) || _listfile.RecordsReverse.ContainsKey(filename))
                return $"{basepath}_{fileId}.blp";

            return UnderscoreReplaceRegex.Replace(filename, "_");
        }

        /// <summary>
        /// Checks the file is a BLP and isn't suffixed with a FileDataID
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        private bool IsApplicable(int fileId)
        {
            if (!_listfile.TryGetValue(fileId, out string fileName))
                return false;

            return fileName.EndsWith(".blp", StringComparison.Ordinal) && !FileIdSuffix.IsMatch(fileName);
        }

        /// <summary>
        /// Returns a formatted combined name taking into account model name prefixing
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="basepath"></param>
        /// <returns></returns>
        private string FormatName(string filename, string basepath)
        {
            var parts = filename.Split('/');
            var fileName = parts[^1];
            var directoryName = parts[^2];

            // strip model name prefix if it exists
            if (fileName.StartsWith(directoryName))
                return $"{basepath}_{fileName.Substring(directoryName.Length)}";

            return basepath.Substring(0, basepath.LastIndexOf('/') + 1) + fileName;
        }
    }
}
