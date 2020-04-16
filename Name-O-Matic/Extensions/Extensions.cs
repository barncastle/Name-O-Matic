using System;
using System.Diagnostics;
using System.Text;
using NameOMatic.Constants;

namespace NameOMatic.Extensions
{
    static class Extensions
    {
        public static void StopAndLog(this Stopwatch sw, string format, int left, int top)
        {
            sw.Stop();

            Console.WriteLine();
            var (CursorLeft, CursorTop) = (Console.CursorLeft, Console.CursorTop);
            Console.SetCursorPosition(left, top);
            Console.WriteLine($"Started {format} enumeration... completed in {sw.Elapsed}");
            Console.SetCursorPosition(CursorLeft, CursorTop);
        }

        public static string ToToken(this IffToken token, bool reverse = false)
        {
            var val = (int)token;
            var chars = new[]
            {
                (char)(byte)(val >> 24),
                (char)(byte)(val >> 16),
                (char)(byte)(val >> 8),
                (char)(byte)(val >> 0)
            };

            if (reverse)
                Array.Reverse(chars);

            return new string(chars);
        }
    }
}
