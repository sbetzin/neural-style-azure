using System.IO;

namespace NeuralStyle.ConsoleClient
{
    public static class StringExtensions
    {
        public static string CorrectName(this string file)
        {
            var path = Path.GetDirectoryName(file);
            var name = Path.GetFileName(file);

            name = name.ToLower().Replace("-", "_").Replace(" ", "_");

            return Path.Combine(path, name);
        }
    }
}