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

using MsgPack.Serialization;
using System;

namespace DCD_Parser.dcd.common.Messages
{
    /// <summary>
    /// Autocompletion response message
    /// </summary>
    public class AutocompleteResponse
    {
        public class Completion
        {
            /// <summary
            /// The name of the symbol for a completion, for calltips just the function name.
            /// </summary>
            [MessagePackMember(0)]
            public string Identifier { get; set; }

            /// <summary>
            /// The kind of the item. Will be char.init for calltips.
            /// </summary>
            [MessagePackMember(1)]
            public byte Kind { get; set; }

            /// <summary>
            /// Definition for a symbol for a completion including attributes or the arguments for calltips.
            /// </summary>
            [MessagePackMember(2)]
            public string Definition { get; set; }

            /// <summary>
            /// The path to the file that contains the symbol.
            /// </summary>
            [MessagePackMember(3)]
            public string SymbolFilePath { get; set; }

            /// <summary>
            /// The byte offset at which the symbol is located or symbol location for symbol searches.
            /// </summary>
            [MessagePackMember(4)]
            public UIntPtr SymbolLocation { get; set; }

            /// <summary>
            /// Documentation associated with this symbol.
            /// </summary>
            [MessagePackMember(5)]
            public string Documentation { get; set; }
        }

        /// <summary>
        /// The autocompletion type. (Parameters or identifier)
        /// </summary>
        [MessagePackMember(0)]
        public string CompletionType { get; set; }

        /// <summary>
        /// The path to the file that contains the symbol.
        /// </summary>
        [MessagePackMember(1)]
        public string SymbolFilePath { get; set; }

        /// <summary>
        /// The byte offset at which the symbol is located.
        /// </summary>
        [MessagePackMember(2)]
        public uint SymbolLocation { get; set; }

        /// <summary>
        /// The completions
        /// </summary>
        [MessagePackMember(3)]
        public Completion[] Completions { get; set; }

        /// <summary>
        /// Import paths that are registered by the server.
        /// </summary>
        [MessagePackMember(4)]
        public string[] ImportPaths { get; set; }

        /// <summary>
        /// Symbol identifier
        /// </summary>
        [MessagePackMember(5)]
        public ulong SymbolIdentifier { get; set; }
    }
}
