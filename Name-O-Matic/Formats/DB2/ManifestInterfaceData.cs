using System;
using System.Collections.Generic;
using System.Diagnostics;
using NameOMatic.Database;

namespace NameOMatic.Formats.DB2
{
    internal static class ManifestInterfaceData
    {
        public static IDictionary<int, string> Enumerate()
        {
            var filenames = new Dictionary<int, string>();
            var manifestData = DBContext.Instance["ManifestInterfaceData"];
            var listfile = ListFile.Instance;

            if (manifestData == null)
                return filenames;

            Stopwatch sw = Stopwatch.StartNew();
            Console.Write("Started ManifestInterfaceData enumeration... ");

            string filename;
            foreach (var rec in manifestData)
            {
                if (!listfile.ContainsKey(rec.Key))
                {
                    filename = rec.Value.Field<string>("FilePath") + rec.Value.Field<string>("FileName");
                    filenames.Add(rec.Key, filename.Replace("\\", "/"));
                }
            }

            sw.Stop();
            Console.WriteLine("completed in " + sw.Elapsed);

            return filenames;
        }
    }
}
