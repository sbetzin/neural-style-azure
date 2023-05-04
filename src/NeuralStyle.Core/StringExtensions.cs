using System;
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
            name = name.RemoveScalingName();

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

        public static string RemoveScalingName(this string file)
        {
            if (!file.Contains("-art-scale") && !file.Contains("-low_res-scale") && !file.Contains("-very_compressed-scale"))
            {
                return file;
            }

            var parts = file.Split(new string[] { "-art-scale", "-low_res-scale", "-very_compressed-scale" }, StringSplitOptions.RemoveEmptyEntries);

            return $"{parts[0]}.jpg";
        }

        public static string FindRelativeUnixPath(this string fullPath, string basePath)
        {
            var baseUri = new Uri(basePath, UriKind.Absolute);
            var targetUri = new Uri(fullPath , UriKind.Absolute);

            var relativeUri = baseUri.MakeRelativeUri(targetUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

           return relativePath;
        }
    }
}