using System.IO;

namespace Scholars_Dictionary.Constants
{
    public static class DataConstants
    {
        public static string DataFolderPath = Path.Combine(PathConstants.AppReleasePath, "Data\\");
        public static string GetDataFilePath(string fileName) => Path.Combine(DataFolderPath, fileName);
    }
}
