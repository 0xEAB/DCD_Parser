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

namespace DCD_Parser.dcd.common.Messages
{
    /// <summary>
    /// Request kind
    /// </summary>
    public enum RequestKind : ushort
    {
        uninitialized =     0b00000000_00000000,
        /// Autocompletion
        autocomplete =      0b00000000_00000001,
        /// Clear the completion cache
        clearCache =        0b00000000_00000010,
        /// Add import directory to server
        addImport =         0b00000000_00000100,
        /// Shut down the server
        shutdown =          0b00000000_00001000,
        /// Get declaration location of given symbol
        symbolLocation =    0b00000000_00010000,
        /// Get the doc comments for the symbol
        doc =               0b00000000_00100000,
        /// Query server status
        query =             0b00000000_01000000,
        /// Search for symbol
        search =            0b00000000_10000000,
        /// List import directories
        listImports =       0b00000001_00000000,
        /// local symbol usage
        localUse =          0b00000010_00000000
    }
}
