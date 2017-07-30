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
    /// Autocompletion response message
    /// </summary>
    public class AutocompleteResponse
    {
        /// <summary>
        /// The autocompletion type. (Parameters or identifier)
        /// </summary>
        [MessagePackMember(0)]
        public string CompletionType{ get; set; }

        /// <summary>
        /// The path to the file that contains the symbol.
        /// </summary>
        [MessagePackMember(1)]
        public string SymbolFilePath{ get; set; }

        /// <summary>
        /// The byte offset at which the symbol is located.
        /// </summary>
        [MessagePackMember(2)]
        public uint SymbolLocation{ get; set; }

        /// <summary>
        /// The documentation comment
        /// </summary>
        [MessagePackMember(3)]
        public string[] DocComments{ get; set; }

        /// <summary>
        /// The completions
        /// </summary>
        [MessagePackMember(4)]
        public string[] Completions{ get; set; }

        /// <summary>
        /// The kinds of the items in the completions array. Will be empty if the
        /// completion type is a function argument list.
        /// </summary>
        [MessagePackMember(5)]
        public char[] CompletionKinds{ get; set; }

        /// <summary>
        /// Symbol locations for symbol searches.
        /// </summary>
        [MessagePackMember(6)]
        public uint[] Locations{ get; set; }

        /// <summary>
        /// Import paths that are registered by the server.
        /// </summary>
        [MessagePackMember(7)]
        public string[] ImportPaths{ get; set; }

        /// <summary>
        /// Symbol identifier
        /// </summary>
        [MessagePackMember(8)]
        public ulong SymbolIdentifier{ get; set; }
    }
}
