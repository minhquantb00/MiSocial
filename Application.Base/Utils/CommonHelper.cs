using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base.Utils
{
    public static partial class CommonHelper
    {
        private static bool? _isDevEnvironment;

        [ThreadStatic]
        private static Random _random;

        private static Random GetRandomizer()
        {
            if (_random == null)
            {
                _random = new Random();
            }

            return _random;
        }

        public static string GenerateRandomDigitCode(int length)
        {
            var buffer = new int[length];
            for (int i = 0; i < length; ++i)
            {
                buffer[i] = GetRandomizer().Next(10);
            }

            return string.Join("", buffer);
        }

        public static int GenerateRandomInteger(int min = 0, int max = 2147483647)
        {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }

        public static string MapPath(string path, bool findAppRoot = true)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');

            var testPath = Path.Combine(baseDirectory, path);

            if (findAppRoot)
            {
                var dir = FindSolutionRoot(baseDirectory);

                if (dir != null)
                {
                    baseDirectory = Path.Combine(dir.FullName, "Presentation\\SmartStore.Web");
                    testPath = Path.Combine(baseDirectory, path);
                }
            }

            return testPath;
        }

        private static DirectoryInfo FindSolutionRoot(string currentDir)
        {
            var dir = Directory.GetParent(currentDir);
            while (true)
            {
                if (dir == null || IsSolutionRoot(dir))
                    break;

                dir = dir.Parent;
            }

            return dir;
        }

        private static bool IsSolutionRoot(DirectoryInfo dir)
        {
            return File.Exists(Path.Combine(dir.FullName, "SmartStoreNET.sln"));
        }

    }
}
