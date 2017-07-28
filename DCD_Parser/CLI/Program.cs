using System;
using System.Reflection;
using System.Diagnostics;
using CoE.em8.Core.CLI.License;
using CoE.em8.Core.CLI.License.GNU;

namespace DCD_Parser.CLI
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
            ILicenseInfo license = new GPLv3(APP_TITLE, "v" + APP_VERSION, APP_COPYRIGHT);
            Console.WriteLine(license.ToString());

            // Print description
            Console.WriteLine(APP_DESCRIPTION);
            Console.WriteLine();

            Console.ReadLine();

            Environment.Exit(dcd.server.Main.Main_(args));
        }
    }
}
