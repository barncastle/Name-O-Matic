using System.IO;
using DBCD.Providers;
using NameOMatic.Database;

namespace NameOMatic.Helpers
{
    internal class CascDBCProvider : IDBCProvider
    {
        public Stream StreamForTableName(string tableName, string build)
        {
            if (DBContext.FileLookup.TryGetValue(tableName, out int fileId))
                if (FileContext.Instance.FileExists(fileId))
                    return FileContext.Instance.OpenFile(fileId);

            return null;
        }
    }
}
