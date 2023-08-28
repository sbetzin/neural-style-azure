using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralStyle.Core.Folders
{
    public static class FolderCheck
    {
        public static bool IsInFolder(string folder, string inFolder)
        {
            var baseUri = new Uri(inFolder, UriKind.Absolute);
            var targetUri = new Uri(folder, UriKind.Absolute);
            var segments = baseUri.Segments.Length;

            var pairs = baseUri.Segments.Take(segments).Zip(targetUri.Segments.Take(segments), (s, s1) => s.RemoveFolderSeparator() == s1.RemoveFolderSeparator()).ToList();

            var allTrue=  pairs.All(result => result);

            return allTrue;

        }

        private static string RemoveFolderSeparator(this string folder)
        {
            return folder.Replace(@"/", "");
        }
    }
}
