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

// Source: DCD/src/server/server.d

using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

using CoE.em8.Core;
using CoE.em8.Core.CLI;
using DCD_Parser.dsymbol.modulecache;

namespace DCD_Parser.dcd.server
{
    public static class Server
    {
        public const string CONFIG_FILE_NAME = "dcd.conf";

        private static Regex envVarRegex = new Regex(@"\$\{([_a-zA-Z][_a-zA-Z 0-9]*)\}");

        /// <summary>
        /// Locates the configuration file
        /// </summary>
        /// <returns>The configuration location.</returns>
        public static string GetConfigurationLocation()
        {
            if (RuntimePlatform.IsUnix)
            {
                string configDir = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
                if (configDir == null)
                {
                    configDir = Environment.GetEnvironmentVariable("HOME");
                    if (configDir != null)
                        configDir = Path.Combine(configDir, ".config", "dcd", CONFIG_FILE_NAME);
                    if (!Directory.Exists(configDir))
                        configDir = Path.Combine("/etc/", CONFIG_FILE_NAME);
                }
                else
                {
                    configDir = Path.Combine(configDir, "dcd", CONFIG_FILE_NAME);
                }
                return configDir;
            }
            else
            {
                return CONFIG_FILE_NAME;
            }
        }

        /// <summary>
        /// Prints a warning message to the user when an old config file is detected.
        /// </summary>
        public static void WarnAboutOldConfigLocation()
        {
            if (RuntimePlatform.IsUnix)
            {
                if (File.Exists(Path.Combine(RuntimePlatform.UserHomeDirectory, ".config/dcd")))
                {
                    ColorUtil.PrintWarning(
                        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
                        "!! Upgrade warning:",
                        "!! '~/.config/dcd' should be moved to '$XDG_CONFIG_HOME/dcd/dcd.conf'",
                        "!! or '$HOME/.config/dcd/dcd.conf'",
                        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"
                    );
                }
            }
        }

        /// <summary>
        /// Loads import directories from the configuration file
        /// </summary>
        public static string[] LoadConfiguredImportDirs()
        {
            WarnAboutOldConfigLocation();
            string configLocation = GetConfigurationLocation();
            if (!File.Exists(configLocation))
                return new string[0];
            Console.WriteLine("Loading configuration from ", configLocation);


            //FileStream f = new FileStream(configLocation,FileMode.Open);
            return File.ReadAllLines(configLocation)
                .Where(a => a.Length > 0 && a[0] != '#')
                .Select(a => a = ExpandEnvVars(a))
                .Where(ModuleCache.ExistanceCheck).ToArray();
        }

        private static string ExpandEnvVars(string l)
        {
            return envVarRegex.Replace(l, m => Environment.GetEnvironmentVariable(m.Groups[1].Value));
        }
    }
}
