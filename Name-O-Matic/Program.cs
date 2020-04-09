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
            if(options.IsValidFormat("ADTDAT"))
                lookup.Merge(new ADTDATEnumerator(diff).Enumerate());
            if(options.IsValidFormat("DB2"))
                lookup.Merge(new DB2Enumerator().Enumerate());
            if (options.IsValidFormat("M2"))
                lookup.Merge(new M2Enumerator(diff).Enumerate());
            if (options.IsValidFormat("WDT"))
                lookup.Merge(new WDTEnumerator(diff).Enumerate());
            if (options.IsValidFormat("WMO"))
                lookup.Merge(new WMOEnumerator(diff).Enumerate());

            // special case guaranteed to be correct           
            lookup.ReplaceRange(ManifestInterfaceData.Enumerate());

            // export
            lookup.Export($"filenames_{FileContext.Instance.Build}_{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.txt");
            Console.WriteLine($"Generated {lookup.Count} filenames");
        }
    }
}
