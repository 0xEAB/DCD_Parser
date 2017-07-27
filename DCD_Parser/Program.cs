using System;
using System.Reflection;
using System.Diagnostics;

namespace DCD_Parser
{
    class Program
    {
        public const string APP_TITLE = "DCD_Parser";
        public const string APP_DESCRIPTION = "An alternative server for the D Completion Daemon (DCD) powered by D_Parser.";
        public const string APP_VERSION = "0.1.0";
        public const string APP_COPYRIGHT = "Copyright (C) 2017  0xEAB";

        public static void Main(string[] args)
        {
            // Print license note
            PrintGPLv3(APP_TITLE, "v" + APP_VERSION, APP_COPYRIGHT);

            //
            Console.ReadLine();

        }

        public static void PrintGPLv3(string appTitle, string appVersion, string copyright)
        {
            Console.WriteLine("{0} [{1}]\n{2}\n", appTitle, appVersion, copyright);
            Console.WriteLine("This program comes with ABSOLUTELY NO WARRANTY.");
            Console.WriteLine("This is free software, and you are welcome to redistribute it under certain conditions.");
            Console.WriteLine("For more information, please refer to <https://www.gnu.org/licenses/gpl-3.0.html>.\n");
        }
    }
}
