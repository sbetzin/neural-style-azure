using System;
using System.IO;
using NeuralStyle.ConsoleClient.Links;

namespace NeuralStyle.ConsoleClient.Features
{
    public static class SortImages
    {
        public static void SortNewImages(string sortImagePath, string outPath)
        {
            var images = sortImagePath.Get_All_Images(SearchOption.AllDirectories);

            foreach (var image in images)
            {
                SortImage(image, outPath);
            }
        }

        private static void SortImage(string image, string outPath)
        {
            Console.WriteLine($"Sorting {image}");
            var tags = image.GetTags();
            var fileName = Path.GetFileName(image);

            if (tags.In == null || tags.Style == null)
            {
                File.Delete(image);
                return;
            }

            var inFile = Path.Combine(outPath,"name", tags.In, fileName);
            var styleFile = Path.Combine(outPath,"style", tags.Style, fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(inFile));
            Directory.CreateDirectory(Path.GetDirectoryName(styleFile));

            File.Move(image, inFile);

            HardLink.Create(inFile, styleFile, true);
        }
    }
}