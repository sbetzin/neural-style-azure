using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeuralStyle.Core.Imaging;

namespace NeuralStyle.Core.Features
{
    public static class FindMissingCombinations
    {
        public static List<(string In, string Style)> Run(string inPath, string stylePath, string outPath)
        {
            var allIn = inPath.Get_All_Images(SearchOption.TopDirectoryOnly);
            var allStyle = stylePath.Get_All_Images(SearchOption.TopDirectoryOnly);
            var allOut = outPath.Get_All_Images(SearchOption.AllDirectories);

            var inWithoutExtensions = allIn.Select(image => (FullPath: image, Name: Path.GetFileNameWithoutExtension(image))).ToList();
            var stylesWithoutExtensions = allStyle.Select(style => (FullPath: style, Name: Path.GetFileNameWithoutExtension(style))).ToList();

            var allCombination = inWithoutExtensions.GetCombinations(stylesWithoutExtensions).ToList();
            var allWithPrefixes = allCombination.Select(combination => (In: combination.In, Style: combination.Style, Prefix: combination.In.Name.BuildPrefix(combination.Style.Name))).ToList();
            var combinationLookup = allWithPrefixes.ToDictionary(combination => combination.Prefix, combination => combination);

            var allPrefixes = allOut.Select(outFile => outFile.GetTags()).Where(result => result.In != null && result.Style != null).Select(result => result.In.BuildPrefix(result.Style)).Distinct().ToList();

            foreach (var prefix in allPrefixes)
            {
                combinationLookup.Remove(prefix);
            }

            return combinationLookup.Values.Select(combination => (combination.In.FullPath, combination.Style.FullPath)).ToList();
        }

      

    }
}
