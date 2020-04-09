using CASCLib;
using System.Collections.Generic;
using System.IO;
using NameOMatic.Constants;
using NameOMatic.Database.Storage;
using NameOMatic.Helpers;

namespace NameOMatic.Database
{
    class FileContext : Singleton<FileContext>
    {
        public string Build => Storage.Build;
        public string FormattedBuild => Storage.FormattedBuild;
        public Expansion Expansion => Storage.Expansion;
        public string RootEKey => Storage.RootEKey;

        private IStorage Storage;

        public void Load(Options options)
        {
            DeleteLogFile();

            if (string.IsNullOrWhiteSpace(options.BuildConfig) || string.IsNullOrWhiteSpace(options.CDNConfig))
                Storage = new CASCLibWrapper(options.Path, options.Product);
            else
                Storage = new TACTNetWrapper(options.Path, options.BuildConfig, options.CDNConfig);

            Storage.UpdateKeyService();
        }

        private void DeleteLogFile()
        {
            string cascLibLog = new LoggerOptionsDefault().LogFileName;
            if (File.Exists(cascLibLog))
                File.Delete(cascLibLog);
        }

        public bool FileExists(int fileDataId) => Storage.FileExists(fileDataId);

        public IEnumerable<int> GetMatchingFiles(int fileDataId) => Storage.GetMatchingFiles(fileDataId);

        public Stream OpenFile(int fileDataId) => Storage.OpenFile(fileDataId);

    }
}
