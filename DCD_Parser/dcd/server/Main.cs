/**
 * This file is part of DCD, a development tool for the D programming language.
 * Copyright (C) 2014 Brian Schott
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

// Source: DCD/src/server/main.d

using CoE.em8.Core.CLI;
using CoE.em8.Core.CLI.CLArgs;
using DCD_Parser.dcd.common;
using DCD_Parser.dcd.common.Messages;
using DCD_Parser.Platform;
using System;
using System.Collections.Generic;

namespace DCD_Parser.dcd.server
{
    public class Main
    {
        public static int Main_(string[] args)
        {
            var acfg = new DCDArgs();

            CLAParser cla = new CLAParser(
                (x => throw new Exception("Unknown parameter: " + x)),
                (x => throw new Exception("Unknown parameter: " + x.Key)),
                new CLArg("port", x => acfg.Port = ushort.Parse(x), "p"),
                new CLACollectionStorer("I", acfg.ImportPaths),
                new CLArg("help", x => acfg.Help = true, "h"),
                new CLArg("version", x => acfg.PrintVersion = true),
                new CLArg("ignoreConfig", x => acfg.IgnoreConfig = true),
                new CLArg("socketFile", x => acfg.SocketFile = x)
            );

            try
            {
                cla.Parse(args);
            }
            catch (Exception ex)
            {
                ColorUtil.PrintError(ex.ToString());
                PrintHelp(AppDomain.CurrentDomain.FriendlyName);
                return 1;
            }


            if (acfg.PrintVersion)
            {
                Console.WriteLine(CLI.Program.APP_VERSION_STRING);
                return 0;
            }

            if (acfg.Help)
            {
                PrintHelp(AppDomain.CurrentDomain.FriendlyName);
                return 0;
            }

            // If the user specified a port number, assume that they wanted a TCP
            // connection. Otherwise set the port number to the default and let the
            // useTCP flag deterimen what to do later.
            if (acfg.Port != 0)
                acfg.UseTCP = true;
            else
                acfg.Port = DCDCommonSocket.DEFAULT_PORT_NUMBER;


            if (!PlatformUtil.Unix && acfg.SocketFile != null)
            {
                ColorUtil.PrintError(DCDCommonSocket.NOT_SUPPORTED_ON_WINDOWS);
                return 1;
            }

            if (Messages.ServerIsRunning(acfg.UseTCP, acfg.SocketFile, acfg.Port))
            {
                ColorUtil.PrintError("Another instance of DCD-server is already running.");
                return 1;
            }

            if (!acfg.IgnoreConfig)
                acfg.ImportPaths.AddRange(Server.LoadConfiguredImportDirs());

            ColorUtil.PrintStatus("Starting up ...");


            return 0;
        }


        public static void PrintHelp(string programName)
        {
            Console.WriteLine(@"
    Usage: {0} options

options:
    -I PATH
        Includes PATH in the listing of paths that are searched for file
        imports.

    --help | -h
        Prints this help message.

    --version
        Prints the version number and then exits.

    --port PORTNUMBER | -pPORTNUMBER
        Listens on PORTNUMBER instead of the default port 9166 when TCP sockets
        are used.

    --logLevel LEVEL
        The logging level. Valid values are 'all', 'trace', 'info', 'warning',
        'error', 'critical', 'fatal', and 'off'.

    --tcp
        Listen on a TCP socket instead of a UNIX domain socket. This switch
        has no effect on Windows.

    --socketFile FILENAME
        Use the given FILENAME as the path to the UNIX domain socket. Using
        this switch is an error on Windows.", programName);
        }
    }
}
