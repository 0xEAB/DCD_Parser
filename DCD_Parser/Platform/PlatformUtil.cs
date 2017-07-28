using System;

namespace DCD_Parser.Platform
{
    public static class PlatformUtil
    {
        public static bool Unix
        {
            get;
            private set;
        }

        public static PlatformID Platform
        {
            get;
            private set;
        }

        public static string UserHomeDirectory
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.Personal); }
        }

        static PlatformUtil()
        {
            Platform = Environment.OSVersion.Platform;

            /**
             *   4 ... Unix
             *   6 ... OS X
             * 128 ... Unix (old versions of Mono)
             */
            int p = (int)Platform;
            Unix = ((p == 4) || (p == 6) || (p == 128));
        }
    }
}
