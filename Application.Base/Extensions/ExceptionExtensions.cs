using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base.Extensions
{
    public static class IOExtensions
    {
        public static bool WaitForUnlock(this FileInfo file, int timeoutMs = 1000)
        {
            var wait = TimeSpan.FromMilliseconds(50);
            var attempts = Math.Floor(timeoutMs / wait.TotalMilliseconds);

            try
            {
                for (var i = 0; i < attempts; i++)
                {
                    if (!IsFileLocked(file))
                    {
                        return true;
                    }

                    Task.Delay(wait).Wait();
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsFileLocked(this FileInfo file)
        {
            if (file == null)
                return false;

            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }
    }
}
