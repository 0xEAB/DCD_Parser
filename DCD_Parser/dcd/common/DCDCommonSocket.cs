/**
 * This file is part of DCD, a development tool for the D programming language.
 * Copyright (C) 2015 Brian Schott
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

// Source: DCD/src/common/socket.d

using System;
using System.IO;

using CoE.em8.Core;

namespace DCD_Parser.dcd.common
{
    public class DCDCommonSocket
    {
        public const ushort DEFAULT_PORT_NUMBER = 9166;
        public const string NOT_SUPPORTED_ON_WINDOWS = "Unix domain sockets are not supported on Windows.";

        public static string GenerateSocketName()
        {
            if (RuntimePlatform.IsUnix)
            {
                string socketFileName = string.Format("dcd-{0}.socket", "getuid()");

                if (RuntimePlatform.PlatformID == PlatformID.MacOSX)
                    return Path.Combine("/", "var", "tmp", socketFileName);
                else
                {
                    string xdg = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");
                    return xdg is null ? Path.Combine("/", "tmp", socketFileName) : Path.Combine(xdg, "dcd.socket");
                }
            }
            else
            {
                throw new NotSupportedException(NOT_SUPPORTED_ON_WINDOWS);
            }
        }
    }
}
