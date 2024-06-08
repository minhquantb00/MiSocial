using Application.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Base.Utils
{
    public static class PathHelper
    {
        private static readonly char[] _invalidPathChars;
        private static readonly char[] _invalidFileNameChars;
        private static readonly Regex _invalidCharsPattern;

        static PathHelper()
        {
            _invalidPathChars = Path.GetInvalidPathChars();
            _invalidFileNameChars = Path.GetInvalidFileNameChars();

            var invalidChars = Regex.Escape(new string(_invalidPathChars) + new string(_invalidFileNameChars));
            _invalidCharsPattern = new Regex(string.Format(@"[{0}]+", invalidChars));
        }
        public static bool IsRootedPath(string basepath)
        {
            return (string.IsNullOrEmpty(basepath) || basepath[0] == '/' || basepath[0] == '\\');
        }
        public static bool IsAppRelativePath(string path)
        {
            if (path == null)
                return false;

            int len = path.Length;

            if (len == 0) return false;

            if (path[0] != '~')
                return false;

            if (len == 1)
                return true;

            return path[1] == '\\' || path[1] == '/';
        }

        public static bool HasScheme(string virtualPath)
        {
            int indexOfColon = virtualPath.IndexOf(':');
            if (indexOfColon == -1)
                return false;
            int indexOfSlash = virtualPath.IndexOf('/');
            return (indexOfSlash == -1 || indexOfColon < indexOfSlash);
        }

        public static string NormalizeAppRelativePath(string path)
        {
            if (path.IsEmpty())
                return path;

            path = path.Replace('\\', '/');

            if (!path.StartsWith("~/"))
            {
                if (path.StartsWith("~"))
                    path = path.Substring(1);

                path = (path.StartsWith("/") ? "~" : "~/") + path;
            }

            return path;
        }

        public static bool IsSafeAppRootPath(string path)
        {
            if (path.EmptyNull().Length > 2 && !path.IsCaseInsensitiveEqual("con") && !HasInvalidPathChars(path))
            {
                try
                {
                    var mappedPath = CommonHelper.MapPath(path);
                    var appPath = CommonHelper.MapPath("~/");
                    return !mappedPath.IsCaseInsensitiveEqual(appPath);
                }
                catch { }
            }

            return false;
        }

        public static string SanitizeFileName(string name)
        {
            if (name.IsEmpty())
                return name;

            return _invalidCharsPattern.Replace(name, "-");
        }

        public static bool HasInvalidPathChars(string path, bool checkWildcardChars = false)
        {
            if (path == null)
                return false;

            return path.IndexOfAny(_invalidPathChars) >= 0
                || (checkWildcardChars && ContainsWildcardChars(path, 0));
        }

        public static bool HasInvalidFileNameChars(string fileName, bool checkWildcardChars = false)
        {
            if (fileName == null)
                return false;

            return fileName.IndexOfAny(_invalidFileNameChars) >= 0
                || (checkWildcardChars && ContainsWildcardChars(fileName, 0));
        }

        private static bool ContainsWildcardChars(string path, int startIndex = 0)
        {
            for (int i = startIndex; i < path.Length; i++)
            {
                switch (path[i])
                {
                    case '*':
                    case '?':
                        return true;
                }
            }

            return false;
        }

        public static bool IsAbsolutePhysicalPath(string path)
        {
            if ((path == null) || (path.Length < 3))
            {
                return false;
            }

            return (((path[1] == ':') && IsDirectorySeparatorChar(path[2])) || IsUncSharePath(path));
        }

        internal static bool IsUncSharePath(string path) =>
            (((path.Length > 2) && IsDirectorySeparatorChar(path[0])) && IsDirectorySeparatorChar(path[1]));


        private static bool IsDirectorySeparatorChar(char ch)
        {
            if (ch != '\\')
            {
                return (ch == '/');
            }

            return true;
        }
    }
}
