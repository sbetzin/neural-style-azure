using System;

namespace NeuralStyle.Core
{
    public static class Logger
    {
        public static event Action<string> NewLog;

        public static void Log(string text)
        {
            OnNewLog(text);
        }

        private static void OnNewLog(string text)
        {
            NewLog?.Invoke(text);
        }
    }
}