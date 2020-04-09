using System;
using System.Diagnostics;

namespace NameOMatic.Extensions
{
    static class Extensions
    {
        public static void StopAndLog(this Stopwatch sw, string format, int? left = null, int? top = null)
        {
            sw.Stop();
            var (CursorLeft, CursorTop) = (Console.CursorLeft, Console.CursorTop);
            Console.SetCursorPosition(left ?? CursorLeft, top ?? CursorTop);
            Console.WriteLine($"Started {format} enumeration... completed in {sw.Elapsed}");
            Console.SetCursorPosition(CursorLeft, CursorTop);
        }
    }
}
