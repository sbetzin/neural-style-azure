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
            var baseUri = new Uri(folder, UriKind.Absolute);
            var targetUri = new Uri(inFolder, UriKind.Absolute);

            var pairs = baseUri.Segments.Take(7).Zip(targetUri.Segments.Take(7), (s, s1) => s == s1).ToList();

            return pairs.All(result => true);
        }
    }
}
