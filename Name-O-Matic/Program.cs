using CommandLine;
using System;
using NameOMatic.Database;
using NameOMatic.Formats.ADTDAT;
using NameOMatic.Formats.DB2;
using NameOMatic.Formats.M2;
using NameOMatic.Formats.WDT;
using NameOMatic.Formats.WMO;
using NameOMatic.Helpers.Collections;
using NameOMatic.Helpers.WoWTools;
using NameOMatic.Constants;
using NameOMatic.Formats;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace NameOMatic
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = Parser.Default.ParseArguments<Options>(args);
            if (options.Tag == ParserResultType.Parsed)
                options.WithParsed(Run);
        }

        static void Run(Options options)
        {
            // load listfile and storage
            ListFile.Instance.Load();
            FileContext.Instance.Load(options);

            // get diff records between prev and current builds
            var diff = new DiffEnumerator(options.DiffHash);

            // start work
            var lookup = new FileNameLookup();
            var tokens = new UniqueLookup<string, int>();

            foreach (var fileNamer in GetFileNamers(diff, options))
            {
                lookup.Merge(fileNamer.Enumerate());
                tokens.Merge(fileNamer.Tokens);
            }

            // special case guaranteed to be correct           
            lookup.ReplaceRange(ManifestInterfaceData.Enumerate());

            // export
            lookup.Export($"filenames_{FileContext.Instance.Build}_{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.txt");
            tokens.Export("Output", $"tokens_{FileContext.Instance.Build}_{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.txt");
            Console.WriteLine($"Generated {lookup.Count} filenames");
            Console.WriteLine($"Found {tokens.Count} new tokens");
            Console.ReadLine();
        }

        static IEnumerable<IFileNamer> GetFileNamers(DiffEnumerator diff, Options options)
        {
            var defaultParams = new Type[] { typeof(DiffEnumerator) };
            var results = new SortedList<string, IFileNamer>(0x10);
            var assembly = Assembly.GetEntryAssembly();

            foreach (var ti in assembly.DefinedTypes)
            {
                if (ti.ImplementedInterfaces.Contains(typeof(IFileNamer)))
                {
                    var constructor = ti.GetConstructor(defaultParams)?.Invoke(new object[] { diff });
                    constructor ??= ti.GetConstructor(Type.EmptyTypes).Invoke(null);
                    var filenamer = (IFileNamer)constructor;

                    if (filenamer.Enabled && options.IsValidFormat(filenamer.Format))
                        results.Add(ti.Name, (IFileNamer)constructor);
                }
            }

            return results.Values;
        }
    }
}
