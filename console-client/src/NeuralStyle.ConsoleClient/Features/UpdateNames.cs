using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NeuralStyle.ConsoleClient.Features
{
    public static class UpdateNames
    {
        public static void FixNames( string inPath, string stylePath, string outPath)
        {
            var allIn = inPath.Get_All_Images(SearchOption.TopDirectoryOnly);
            var allStyle = stylePath.Get_All_Images(SearchOption.TopDirectoryOnly);
            var allOut = outPath.Get_All_Images(SearchOption.AllDirectories);

            var inWithoutExtensions = allIn.Select(image => (FullPath: image, Name: Path.GetFileNameWithoutExtension(image))).ToList();
            var stylesWithoutExtensions = allStyle.Select(style => (FullPath: style, Name: Path.GetFileNameWithoutExtension(style))).ToList();

            var allCombination = inWithoutExtensions.GetCombinations(stylesWithoutExtensions).ToList();

            foreach (var image in allCombination)
            {
                var oldName = image.In.Name.BuildPrefix(image.Style.Name);
                var newName = image.In.Name.BuildNewPrefix(image.Style.Name);
                var outFile = allOut.FirstOrDefault(i => i.Contains(oldName));
                
                if (outFile != null) {
                    if (File.Exists(outFile))
                    {
                        var newOutFile = outFile.Replace(oldName, newName);
                        Console.WriteLine($"rename {outFile} to {newOutFile}");
                        File.Move(outFile, newOutFile);
                    }
                }

            }
        }
    }
}
