using DCD_Parser.dcd.common;
using DCD_Parser.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (PlatformUtil.Unix)
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
