using System.Collections.Generic;

using CoE.em8.Core;
using DCD_Parser.dcd.common;

namespace DCD_Parser.dcd.server
{
    class DCDArgs
    {
        public ushort Port { get; set; } = DCDCommonSocket.DEFAULT_PORT_NUMBER;
        public List<string> ImportPaths { get; } = new List<string>();
        public bool Help { get; set; } = false;
        public bool PrintVersion { get; set; } = false;
        public bool IgnoreConfig { get; set; } = false;
        public bool UseTCP { get; set; }
        public string SocketFile { get; set; }

        public DCDArgs()
        {
            if (RuntimePlatform.IsUnix)
            {
                this.UseTCP = false;
                this.SocketFile = DCDCommonSocket.GenerateSocketName();
            }
            else
            {
                this.UseTCP = true;
            }
        }
    }
}
