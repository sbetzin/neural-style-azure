using System;
using System.Collections.Generic;
using System.IO;

namespace NeuralStyle.ConsoleClient.Features
{
    public static class UpdateTags
    {
        public static void FixExifTags(string images)
        {
            var allImages = images.Get_All_Images(SearchOption.AllDirectories);

            foreach (var image in allImages)
            {
                image.FixTags();
            }
        }

        public static void Update_Tags_in_Existing_Images(string inPath, string stylePath, string outPath)
        {
            var allStyles = stylePath.Get_Images_Without_Extensions(SearchOption.AllDirectories);
            var allIn = inPath.Get_Images_Without_Extensions(SearchOption.AllDirectories);
            var allOut = outPath.Get_All_Images_Without_Extensions(SearchOption.AllDirectories);

            var images = GetImagesWithoutTags(allStyles, allIn, allOut);

            foreach (var (file, image, style) in images)
            {
                file.UpdateTags(image, style);
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static IEnumerable<(string, string, string)> GetImagesWithoutTags(List<string> allStyles, List<string> allIn, List<string> allOut)
        {
            var combinations = allIn.GetCombinations(allStyles);

            foreach (var file in allOut)
            {
                foreach (var (image, style) in combinations)
                {
                    var prefix = image.BuildPrefix(style);

                    if (Path.GetFileNameWithoutExtension(file).StartsWith(prefix))
                    {
                        yield return (file, image, style);
                    }
                }
            }
        }
    }
}
