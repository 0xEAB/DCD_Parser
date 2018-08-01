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

// Source: DCD/src/common/messages.d

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

using CoE.em8.Core;
using Mono.Unix;
using MsgPack.Serialization;

namespace DCD_Parser.dcd.common.Messages
{
    static class Messages
    {
        public static readonly SerializationContext msgpackCtx;
        public static readonly MessagePackSerializer<AutocompleteRequest> msgpackReq;
        public static readonly MessagePackSerializer<AutocompleteResponse> msgpackRsp;

        static Messages()
        {
            msgpackCtx = new SerializationContext()
            {
                SerializationMethod = SerializationMethod.Array
            };
            msgpackCtx.EnumSerializationOptions.SerializationMethod = EnumSerializationMethod.ByUnderlyingValue;
            msgpackReq = msgpackCtx.GetSerializer<AutocompleteRequest>();
            msgpackRsp = msgpackCtx.GetSerializer<AutocompleteResponse>();
        }

        /// <returns>true on success</returns>
        public static bool SendRequest(Socket socket, AutocompleteRequest request)
        {
            var message = new MemoryStream();

            try
            {
                var nw = new NetworkStream(socket);

                msgpackReq.Pack(message, request);

                uint length = Convert.ToUInt32(message.Length);
                byte[] buffer = BitConverter.GetBytes(length);

                socket.Send(buffer);
                socket.Send(message.ToArray());

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (ex is IOException)
                    return false;
                else
                    throw ex;
            }
            finally
            {
                message.Close();
            }
        }

        /// <summary>
        /// Gets the response from the server
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static AutocompleteResponse GetResponse(Socket socket)
        {
            byte[] buffer = new byte[1024 * 16];

            var bytesReceived = socket.Receive(buffer);

            if (bytesReceived < 0)
                throw new Exception("Incorrect number of bytes received");
            if (bytesReceived == 0)
                throw new Exception("Server closed the connection, 0 bytes received");

            var mem = new MemoryStream();
            mem.Write(buffer, 0, bytesReceived);
            mem.Position = 0;

            var response = msgpackRsp.Unpack(mem);
            mem.Close();

            return response;
        }

        /// <param name="useTCP">`true` to check a TCP port, `false` for UNIX domain socket</param>
        /// <param name="socketFile">the file name for the UNIX domain socket</param>
        /// <param name="port">the TCP port</param>
        /// <returns>true if a server instance is running</returns>
        public static bool ServerIsRunning(bool useTCP, string socketFile, ushort port)
        {
            AutocompleteRequest request = new AutocompleteRequest()
            {
                Kind = RequestKind.query
            };

            Socket socket = null;

            if (!RuntimePlatform.IsUnix)
                useTCP = true;

            EndPoint endpoint;

            if (useTCP)
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                endpoint = new IPEndPoint(IPAddress.Loopback, port);
            }
            else
            {
                socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
                endpoint = new UnixEndPoint(socketFile);
            }

            try
            {
                socket.Connect(endpoint);

                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);
                socket.Blocking = true;

                if (SendRequest(socket, request))
                {
                    AutocompleteResponse r = GetResponse(socket);
                    return (r.CompletionType == "ack");
                }
                else
                {
                    return false;
                }
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.ConnectionRefused)
                    return false;
                else
                    throw ex;
            }
            finally
            {
                if (socket.Connected)
                    socket.Shutdown(SocketShutdown.Both);

                socket.Close();
            }
        }
    }
}
