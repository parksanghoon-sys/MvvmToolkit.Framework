using System;
using System.Collections.Generic;
using System.Text;

namespace MvvmToolkit.Core.Ioc.FileProvider.Physical.Internal
{
    internal static class PathUtils
    {
        private static char[] GetInvalidFileNameChars()
            => Path.GetInvalidFileNameChars().Where(c => c != Path.DirectorySeparatorChar && c != Path.AltDirectorySeparatorChar).ToArray();


        private static char[] GetInvalidFilterChars() => GetInvalidFileNameChars()
            .Where(c => c != '*' && c != '|' && c != '?').ToArray();

        internal static string EnsureTrailingSlash(string path)
        {
            if (string.IsNullOrEmpty(path) == false &&
                path[path.Length - 1] != Path.DirectorySeparatorChar)
            {
                return path + Path.DirectorySeparatorChar;
            }
            return path;
        }
    }
}
