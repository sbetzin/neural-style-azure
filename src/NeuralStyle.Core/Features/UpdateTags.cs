using System;
using System.Collections.Generic;
using System.IO;

namespace NeuralStyle.Core.Features
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
            var allOut = outPath.Get_All_Images(SearchOption.AllDirectories);

            var images = GetImagesWithoutTags(allStyles, allIn, allOut);

            foreach (var (file, inImage, styleImage) in images)
            {
                Console.WriteLine($"Updating tag for {file} with in={inImage}, style={styleImage}");
                file.UpdateTags(inImage, styleImage);
            }

            Console.WriteLine("Done updating not existing tags");
            Console.ReadLine();
        }

        private static IEnumerable<(string, string, string)> GetImagesWithoutTags(List<string> allStyles, List<string> allIn, List<string> allOut)
        {
            var combinations = allIn.GetCombinations(allStyles);

            foreach (var file in allOut)
            {
                var tags = file.GetTags();
                if (tags.Style != null && tags.In!= null)
                {
                    continue;
                }

                foreach (var (inImage, styleImage) in combinations)
                {
                    var prefix = inImage.BuildPrefix(styleImage);

                    if (Path.GetFileNameWithoutExtension(file).StartsWith(prefix))
                    {
                        yield return (file, inImage, styleImage);
                    }
                }
            }
        }
    }
}
