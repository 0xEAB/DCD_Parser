using System;
using CoE.em8.Core.CLI.License;
using CoE.em8.Core.CLI.License.GNU;

namespace DCD_Parser.CLI
{
    class Program
    {
        public const string APP_TITLE = "DCD_Parser";
        public const string APP_DESCRIPTION = "An alternative server for the D Completion Daemon (DCD) powered by D_Parser.";
        public const string APP_VERSION_NUMBER = "0.1.0";
        public const string APP_VERSION_SUFFIX = "-alpha";
        public const string APP_VERSION_STRING = "v" + APP_VERSION_NUMBER + APP_VERSION_SUFFIX;
        public const string APP_COPYRIGHT = "Copyright (C) 2017-2018  0xEAB";

        public static int Main(string[] args)
        {
            // Print license note
            ILicenseInfo license = new GPLv3(APP_TITLE, APP_VERSION_STRING, APP_COPYRIGHT);
            Console.WriteLine(license.ToString());

            // Print description
            Console.WriteLine(APP_DESCRIPTION);
            Console.WriteLine();

            Console.ReadLine();

            return dcd.server.Main.Main_(args);
        }
    }
}
