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
        public static List<(string In, string Style)> Find_Missing_Combinations(string inPath, string stylePath, string outPath)
        {
            var allIn = inPath.Get_All_Images();
            var allStyle = stylePath.Get_All_Images();
            var allOut = outPath.Get_All_Images();

            var inWithoutExtensions = allIn.Select(image => (FullPath: image, Name: Path.GetFileNameWithoutExtension(image))).ToList();
            var stylesWithoutExtensions = allStyle.Select(style => (FullPath: style, Name: Path.GetFileNameWithoutExtension(style))).ToList();

            var allCombination = GetCombinations(inWithoutExtensions, stylesWithoutExtensions).ToList();
            var allWithPrefixes = allCombination.Select(combination => (In: combination.In, Style: combination.Style, Prefix: BuildPrefix(combination.In.Name, combination.Style.Name))).ToList();
            var combinationLookup = allWithPrefixes.ToDictionary(combination => combination.Prefix, combination => combination);

            var allPrefixes = allOut.Select(outFile => outFile.GetTags()).Where(result => result.In != null && result.Style != null).Select(result => BuildPrefix(result.In, result.Style)).Distinct().ToList();

            foreach (var prefix in allPrefixes)
            {
                combinationLookup.Remove(prefix);
            }

            return combinationLookup.Values.Select(combination => (combination.In.FullPath, combination.Style.FullPath)).ToList();
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
            var combinations = GetCombinations(allIn, allStyles);

            foreach (var file in allOut)
            {
                foreach (var (image, style) in combinations)
                {
                    var prefix = BuildPrefix(image, style);

                    if (Path.GetFileNameWithoutExtension(file).StartsWith(prefix))
                    {
                        yield return (file, image, style);
                    }
                }
            }
        }

        private static string BuildPrefix(string image, string style)
        {
            var prefix = $"{image}_{style}_";
            return prefix;
        }

        private static List<(T In, TR Style)> GetCombinations<T, TR>(List<T> allIn, List<TR> allStyles)
        {
            return allStyles.SelectMany(styleImage => allIn, (styleImage, inImage) => (In: inImage, Style: styleImage)).ToList();

        }

        public static void FixExifTags(string images)
        {
            var allImages = images.Get_All_Images();

            foreach (var image in allImages)
            {
                image.FixTags();
            }
        }
    }
}
