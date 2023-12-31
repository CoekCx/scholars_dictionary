﻿using System.IO;

namespace Scholars_Dictionary.Constants
{
    public static class DataConstants
    {
        public static string DataFolderPath = Path.Combine(PathConstants.AppReleasePath, "Data\\");
        public static string DataFolderPathDebug = Path.Combine(PathConstants.AppPath, "Data\\");

        public static string SpeechFolderPath = Path.Combine(PathConstants.AppReleasePath, "Data\\Speech\\");
        public static string SpeechFolderPathDebug = Path.Combine(PathConstants.AppPath, "Data\\Speech\\");

        public static string GetDataFilePath(string fileName)
        {
            var tempPath = Path.Combine(DataFolderPath, fileName);
            if(File.Exists(tempPath))
            {
                return tempPath;
            }

            return Path.Combine(DataFolderPathDebug, fileName);
        }

        public static string GetSpeechFilePath(string fileName)
        {
            if(File.Exists(SpeechFolderPath))
            {
                return Path.Combine(SpeechFolderPath, fileName);
            }

            return Path.Combine(SpeechFolderPathDebug, fileName);
        }
    }
}
