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
 * but WITHOUT ANY WARRANTY{ get; set; } without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

// Source: DCD/src/common/messages.d

using MsgPack.Serialization;

namespace DCD_Parser.dcd.common.Messages
{
    /// <summary>
    ///  Autocompletion request message
    /// </summary>
    public class AutocompleteRequest
    {
        /// <summary>
        /// File name used for error reporting
        /// </summary>
        [MessagePackMember(0)]
        public string FileName{ get; set; }

        /// <summary>
        /// Command coming from the client
        /// </summary>
        [MessagePackMember(1)]
        public RequestKind Kind{ get; set; }

        /// <summary>
        /// Paths to be searched for import files
        /// </summary>
        [MessagePackMember(2)]
        public string[] ImportPaths{ get; set; }

        /// <summary>
        /// The source code to auto complete
        /// </summary>
        [MessagePackMember(3)]
        public byte[] SourceCode{ get; set; }

        /// <summary>
        /// The cursor position
        /// </summary>
        [MessagePackMember(4)]
        public uint CursorPosition{ get; set; }

        /// <summary>
        /// Name of symbol searched for
        /// </summary>
        [MessagePackMember(5)]
        public string SearchName{ get; set; }
    }
}
