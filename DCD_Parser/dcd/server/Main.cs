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

using System;

using CoE.em8.Core;
using CoE.em8.Core.CLI;
using CoE.em8.Core.CLI.CLArgs;
using DCD_Parser.dcd.common;
using DCD_Parser.dcd.common.Messages;
using System.Diagnostics;
using System.Net.Sockets;
using System.IO;
using System.Net;
using Mono.Unix;
using System.Collections.Generic;

namespace DCD_Parser.dcd.server
{
    public class Main
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("", "CS0164")]
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


            if (!RuntimePlatform.IsUnix && acfg.SocketFile != null)
            {
                ColorUtil.PrintError(DCDCommonSocket.NOT_SUPPORTED_ON_WINDOWS);
                return 1;
            }

            if (Messages.ServerIsRunning(acfg.UseTCP, acfg.SocketFile, acfg.Port))
            {
                ColorUtil.PrintError("Another instance of DCD-server is already running.");
                return 1;
            }

            ColorUtil.PrintNotice("Starting up ...");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (!acfg.IgnoreConfig)
                acfg.ImportPaths.AddRange(Server.LoadConfiguredImportDirs());


            Socket socket;
            if (acfg.UseTCP)
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    Blocking = true
                };
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                socket.Bind(new IPEndPoint(IPAddress.Loopback, acfg.Port));
                ColorUtil.PrintStatus("Listening on port " + acfg.Port);
            }
            else
            {
                if (!RuntimePlatform.IsUnix)

                {
                    ColorUtil.PrintError(DCDCommonSocket.NOT_SUPPORTED_ON_WINDOWS);
                    return 1;
                }
                else
                {
                    socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Tcp);
                    if (File.Exists(acfg.SocketFile))
                    {
                        ColorUtil.PrintNotice("Cleaning up old socket file at " + acfg.SocketFile);
                        File.Delete(acfg.SocketFile);
                    }
                    socket.Blocking = true;
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    socket.Bind(new UnixEndPoint(acfg.SocketFile));
                    //setAttributes(socketFile, S_IRUSR | S_IWUSR);
                    ColorUtil.PrintStatus("Listening at" + acfg.SocketFile);
                }
            }

            socket.Listen(32);

            try
            {
                //ModuleCache cache = ModuleCache(new ASTAllocator);
                //cache.addImportPaths(importPaths);
                //infof("Import directories:\n    %-(%s\n    %)", cache.getImportPaths());

                byte[] buffer = new byte[1024 * 1024 * 4]; // 4 megabytes should be enough for anybody...

                sw.Stop();
                //info(cache.symbolsAllocated, " symbols cached.");
                ColorUtil.PrintNotice("Startup completed in " + sw.ElapsedMilliseconds + " milliseconds.");

                serverLoop: while (true)
                {
                    var s = socket.Accept();
                    s.Blocking = true;

                    if (acfg.UseTCP)
                    {
                        // Only accept connections from localhost
                        IPAddress clientAddr = ((IPEndPoint)s.RemoteEndPoint).Address;

                        // Shut down if somebody tries connecting from outside
                        if (!IPAddress.IsLoopback(clientAddr))
                        {
                            ColorUtil.PrintError("Connection attempted from " + clientAddr);
                            return 1;
                        }
                    }

                    try
                    {
                        int bytesReceived = s.Receive(buffer);

                        var requestWatch = new Stopwatch();
                        requestWatch.Start();

                        int messageLength = BitConverter.ToInt32(buffer, 0);
                        if (messageLength < 0)
                        {
                            ColorUtil.PrintError("Received header of too large package: " + (uint)messageLength + '/' + int.MaxValue + " bytes");
                            return 1;
                        }

                        int packageLength = Convert.ToInt32(messageLength) + 4;

                        List<byte> msgBuffer = new List<byte>(packageLength);

                        for (int i = 4; i < packageLength; i++)
                            msgBuffer.Add(buffer[i]);

                        while (bytesReceived < packageLength)
                        {
                            int b = s.Receive(buffer);

                            if (b < 0)
                            {
                                bytesReceived = (int)SocketError.SocketError;
                                break;
                            }
                            else
                            {
                                for (int i = 0; i < b; i++)
                                {
                                    msgBuffer.Add(buffer[i]);
                                    bytesReceived++;

                                    if (bytesReceived == messageLength)
                                        break;
                                }
                            }
                        }

                        if (bytesReceived < 0)
                        {
                            ColorUtil.PrintWarning("Socket recieve failed");
                            break;
                        }

                        var message = new MemoryStream(msgBuffer.ToArray());
                        AutocompleteRequest request = Messages.msgpackReq.Unpack(message);

                        // TODO: ...

                    }
                    finally
                    {
                        if (s.Connected)
                            s.Shutdown(SocketShutdown.Both);

                        s.Close();
                    }
                }

                return 0;
            }
            finally
            {
                ColorUtil.PrintNotice("Shutting down sockets...");

                if (socket.Connected)
                    socket.Shutdown(SocketShutdown.Both);

                socket.Close();

                if (!acfg.UseTCP)
                    File.Delete(acfg.SocketFile);

                ColorUtil.PrintStatus("Sockets shut down.");
            }
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
