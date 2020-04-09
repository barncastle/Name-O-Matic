using System.Collections.Generic;
using System.IO;
using NameOMatic.Constants;

namespace NameOMatic.Database.Storage
{
    interface IStorage
    {
        string Build { get; }
        string FormattedBuild { get; }
        Expansion Expansion { get; }
        string RootEKey { get; }

        bool FileExists(int fileDataId);
        Stream OpenFile(int fileDataId);
        void UpdateKeyService();
        IEnumerable<int> GetMatchingFiles(int fileDataId);
    }
}
