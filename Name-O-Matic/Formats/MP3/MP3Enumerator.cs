using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NameOMatic.Constants;
using NameOMatic.Database;
using NameOMatic.Extensions;
using NameOMatic.Helpers.Collections;
using NameOMatic.Helpers.WoWTools;
using TagLib;
using static TagLib.File;

namespace NameOMatic.Formats.MP3
{
    internal static class MP3Enumerator
    {
        public static IDictionary<int, string> Enumerate(DiffEnumerator diff)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var (CursorLeft, CursorTop) = (Console.CursorLeft, Console.CursorTop);
            Console.Write("Started MP3 enumeration... ");

            var filenames = new Dictionary<int, string>();

            diff.FileType = "mp3";
            foreach (var fileID in diff)
            {
                if (!ListFile.Instance.ContainsKey(fileID) && TryRead(fileID, out var title))
                    filenames.Add(fileID, title);
            }

            sw.StopAndLog("MP3", CursorLeft, CursorTop);
            return filenames;
        }

        private static bool TryRead(int fileID, out string title)
        {
            title = null;

            if (!FileContext.Instance.FileExists(fileID))
                return false;

            try
            {
                var file = Create(new MP3FileAbstraction(fileID));
                if (!string.IsNullOrEmpty(file.Tag.Title))
                {
                    var type = file.Tag.Title.Split('_')[1].ToLower();

                    title = type switch
                    {
                        "event" => $"sound/music/event/{file.Tag.Title}.mp3",
                        _ => $"sound/music/{FileContext.Instance.Expansion}/{file.Tag.Title}.mp3",
                    };

                    return true;
                }
            }
            catch (CorruptFileException) { }

            return false;
        }
    }
}
