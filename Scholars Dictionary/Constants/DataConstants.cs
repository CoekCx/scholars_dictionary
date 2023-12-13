using System.IO;

namespace Scholars_Dictionary.Constants
{
    public static class DataConstants
    {
        public static string DataFolderPath = Path.Combine(PathConstants.AppReleasePath, "Data\\");
        public static string DataFolderPathDebug = Path.Combine(PathConstants.AppPath, "Data\\");
        public static string GetDataFilePath(string fileName)
        {
            var tempPath = Path.Combine(DataFolderPath, fileName);
            if(File.Exists(tempPath))
            {
                return tempPath;
            }

            return Path.Combine(DataFolderPathDebug, fileName);
        }
    }
}
