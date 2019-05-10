using System.IO;

namespace NeuralStyle.Core
{
    public static class StringExtensions
    {
        public static string CorrectName(this string file)
        {
            var path = Path.GetDirectoryName(file);
            var name = Path.GetFileName(file);

            name = name.ToLower().Replace(" ", "_");

            return Path.Combine(path, name);
        }

        public static string BuildOldPrefix(this string image, string style)
        {
            return $"{image}_{style}_";
        }

        public static string BuildPrefix(this string image, string style)
        {
            return $"{image}-{style}-";
        }
    }
}