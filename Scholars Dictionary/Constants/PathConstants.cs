using System;

namespace Scholars_Dictionary.Constants
{
    public static class PathConstants
    {
        public static string AppReleasePath = RemoveBinDebugOrRelease(AppDomain.CurrentDomain.BaseDirectory);
        public static string AppPath = AppDomain.CurrentDomain.BaseDirectory;

        private static string RemoveBinDebugOrRelease(string path)
        {
            int binIndex = path.IndexOf("bin", StringComparison.OrdinalIgnoreCase);
            if (binIndex != -1)
            {
                // Remove 'bin\Debug' or 'bin\Release' and everything after it
                return path.Substring(0, binIndex);
            }
            return path;
        }
    }
}
