using System;
using System.Collections.Generic;
using System.Text;

namespace NameOMatic.Helpers
{
    public static class ConsoleEx
    {
        public static void WriteError(string message)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = oldColor;
        }

        public static void WriteWarning(string message)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = oldColor;
        }
    }
}
