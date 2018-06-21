using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace NeuralStyle.ConsoleClient.Links
{
    public static class HardLink
    {
        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

        public static void Create(string exitingFile, string targetFile, bool replaceExisting)
        {
            if (replaceExisting)
            {
                if (File.Exists(targetFile))
                {
                    File.Delete(targetFile);
                }
            }

            var result = CreateHardLink(targetFile, exitingFile, IntPtr.Zero);
            if (!result)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }
}