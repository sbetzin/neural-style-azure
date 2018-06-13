using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NeuralStyle.ConsoleClient
{
    public static class Features
    {
        public static void Find_Missing_Combinations(string inPath, string stylePath, string outPath)
        {
            var allOut = outPath.Get_All_Images();
            var allStyles = stylePath.Get_Images_Without_Extensions();
            var allIn = inPath.Get_Images_Without_Extensions();

            var allCombination = GetCombinations(allIn, allStyles).ToDictionary(combi => combi.prefix, combi => (combi.inImage, combi.styleImage));
            var allOutImages = allOut.Select(outFile => outFile.GetTags2()).Where(result => result.inImage != null && result.styleImage != null);
            var allOutImagesPrefixes = allOutImages.Select(result => $"{result.inImage}_{result.styleImage}_").Distinct().ToList();

            foreach (var prefix in allOutImagesPrefixes)
            {
                allCombination.Remove(prefix);
            }

        }

        public static void Update_Tags_in_Existing_Images(string inPath, string stylePath, string outPath)
        {
            var allStyles = stylePath.Get_Images_Without_Extensions();
            var allIn = inPath.Get_Images_Without_Extensions();
            var allOut = outPath.Get_All_Images_Without_Extensions();

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
            var cominations = GetCombinations(allIn, allStyles);

            foreach (var file in allOut)
            {
                foreach (var (style, image, comination) in cominations)
                {
                    if (Path.GetFileNameWithoutExtension(file).StartsWith(comination))
                    {
                        yield return (file, image, style);
                    }
                }
            }
        }

        private static List<(string inImage, string styleImage, string prefix)> GetCombinations(List<string> allIn, List<string> allStyles)
        {
            return allStyles.SelectMany(styleImage => allIn, (styleImage, inImage) => (styleImage, inImage, $"{inImage}_{styleImage}_")).ToList();

        }
    }
}
