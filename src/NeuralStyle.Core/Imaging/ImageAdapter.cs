using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeuralStyle.Core.Imaging
{
    public static class ImageAdapter
    {
        public static List<string> Get_Images_Without_Extensions(this string path, SearchOption searchOption)
        {
            return Directory.GetFiles(path, "*.jpg", searchOption).Select(Path.GetFileNameWithoutExtension).ToList();
        }

        public static List<string> Get_All_Images_Without_Extensions(this string path, SearchOption searchOption)
        {
            return Directory.GetFiles(path, "*.jpg", searchOption).Select(Path.GetFileNameWithoutExtension).ToList();
        }

        public static List<string> Get_All_Images(this string path, SearchOption searchOption)
        {
            return Directory.GetFiles(path, "*.jpg", searchOption).ToList();
        }

        public static void EnsureDirectoryExists(this string inFile)
        {
            var path = Path.GetDirectoryName(inFile);
            if (path == null)
            {
                return;
            }

            Directory.CreateDirectory(path);
        }
    }
}
