﻿using System.Diagnostics;

namespace NeuralStyle.Service
{
    public static class Shell
    {
        public static string Bash(this string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return result;
        }
    }
}