using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NameOMatic.Constants;
using NameOMatic.Extensions;
using NameOMatic.Helpers.Collections;

namespace NameOMatic.Formats.DB2
{
    class DB2Enumerator : IFileNamer
    {
        public string Format { get; } = "DB2";
        public bool Enabled { get; } = false;
        public FileNameLookup FileNames { get; private set; }
        public UniqueLookup<string, int> Tokens { get; }

        public DB2Enumerator()
        {
            FileNames = new FileNameLookup();
            Tokens = new UniqueLookup<string, int>();
        }

        public FileNameLookup Enumerate()
        {
            Stopwatch sw = Stopwatch.StartNew();
            var (CursorLeft, CursorTop) = (Console.CursorLeft, Console.CursorTop);
            Console.WriteLine("Started DB2 enumeration... ");

            foreach (var db2 in GetAll<IDB2>())
            {
                if (!db2.IsValid)
                    continue;

                Console.WriteLine("\t" + db2.GetType().Name);
                db2.Enumerate();
                FileNames.AddRange(db2.FileNames);
            }

            sw.StopAndLog("DB2", CursorLeft, CursorTop);
            return FileNames;
        }


        private IEnumerable<T> GetAll<T>()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                foreach (var ti in assembly.DefinedTypes)
                    if (ti.ImplementedInterfaces.Contains(typeof(T)))
                        yield return (T)assembly.CreateInstance(ti.FullName);
            }
        }
    }
}
