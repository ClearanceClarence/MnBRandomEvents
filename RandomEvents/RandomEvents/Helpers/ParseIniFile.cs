using System.IO;
using System.Reflection;
using TaleWorlds.Library;

namespace Bannerlord.RandomEvents.Helpers
{
    /// <summary>
    /// Provides methods to parse and manage the INI configuration file for random events.
    /// </summary>
    public abstract class ParseIniFile
    {
        /// <summary>
        /// Retrieves the path to the configuration file. If the file does not exist, a default one is created.
        /// </summary>
        /// <returns>
        /// The full path to the configuration file.
        /// </returns>
        public static string GetTheConfigFile()
        {
            var strExeFilePath = Assembly.GetExecutingAssembly().Location;
            var strWorkPath = Path.GetDirectoryName(strExeFilePath);
            // ReSharper disable once AssignNullToNotNullAttribute
            var finalPath = Path.GetFullPath(Path.Combine(strWorkPath, @"..\..\ModuleData\RandomEvents_EventConfiguration.ini"));

            if (!File.Exists(finalPath))
            {
                InformationManager.DisplayMessage(new InformationMessage("Random Events ini file not found, Generating a new one.", RandomEventsSubmodule.Ini_Color));

                CreateDefaultIniFile(finalPath);
            }

            return finalPath;
        }

        /// <summary>
        /// Creates a default configuration file at the specified path.
        /// </summary>
        /// <param name="filePath">The path where the default INI file will be created.</param>
        private static void CreateDefaultIniFile(string filePath)
        {
            File.WriteAllText(filePath, DefaultIni.Content());
        }
    }
}