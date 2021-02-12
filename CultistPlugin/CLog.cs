using System;
using System.Collections.Generic;
using System.Text;

namespace CultistPlugin
{
    class CLog
    {
        public static void Info(string message)
        {
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("[---INFO---] " + message);
            System.Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Error(string message)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("[---ERROR---] " + message);
            System.Console.ForegroundColor = ConsoleColor.White;
        }
    }
}