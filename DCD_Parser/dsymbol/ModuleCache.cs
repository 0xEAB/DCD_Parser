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

// Source: dsymbol/src/dsymbol/modulecache.d

using System;
using System.IO;
using CoE.em8.Core.CLI;

namespace DCD_Parser.dsymbol.modulecache
{
    public struct ModuleCache
    {
        /// <returns><c>true</c>, if a file exists at the given path</returns>
        public static bool ExistanceCheck(string path)
        {
            if (File.Exists(path))
                return true;
            
            ColorUtil.PrintWarning(string.Format("Cannot cache modules in {0} because it does not exist", path));
            return false;
        }
    }
}

