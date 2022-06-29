using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using NeuralStyle.Core.Imaging;

namespace NeuralStyle.Core.Features
{
    public static class UpdateNames
    {
        public static void Ensure_Correct_Filenames(string path)
        {
            var files = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var correctName = file.CorrectName();
                if (file == correctName)
                {
                    continue;
                }

                Logger.Log($"renaming {file} to {correctName}");
                File.Move(file, correctName);
            }
        }

        public static void FixNames(string inPath, string stylePath, string outPath)
        {
            var allIn = inPath.Get_All_Images(SearchOption.TopDirectoryOnly);
            var allStyle = stylePath.Get_All_Images(SearchOption.TopDirectoryOnly);
            var allOut = outPath.Get_All_Images(SearchOption.AllDirectories);

            var inWithoutExtensions = allIn.Select(image => (FullPath: image, Name: Path.GetFileNameWithoutExtension(image))).ToList();
            var stylesWithoutExtensions = allStyle.Select(style => (FullPath: style, Name: Path.GetFileNameWithoutExtension(style))).ToList();

            var allCombination = inWithoutExtensions.GetCombinations(stylesWithoutExtensions).ToList();

            foreach (var image in allCombination)
            {
                var oldName = image.In.Name.BuildOldPrefix(image.Style.Name);
                var newName = image.In.Name.BuildPrefix(image.Style.Name);
                var outFiles = allOut.Where(i => i.Contains(oldName)).ToList();

                if (!outFiles.Any())
                {
                    continue;
                }

                foreach (var outFile in outFiles)
                {
                    if (!File.Exists(outFile))
                    {
                        continue;
                    }

                    var newOutFile = outFile.Replace(oldName, newName);
                    Logger.Log($"rename {outFile} to {newOutFile}");
                    File.Move(outFile, newOutFile);
                }
            }
        }

        public static void FixNamesByTag(string outPath)
        {
            var allOut = outPath.Get_All_Images(SearchOption.AllDirectories);

            foreach (var outFile in allOut)
            {
                var (inImage, styleImage) = outFile.GetTags();

                var oldName = inImage.BuildOldPrefix(styleImage);
                var newName = inImage.BuildPrefix(styleImage);

                if (!outFile.Contains(oldName))
                {
                    continue;
                    ;
                }

                var newOutFile = outFile.Replace(oldName, newName);
                Logger.Log($"rename {outFile} to {newOutFile}");
                File.Move(outFile, newOutFile);
            }
        }
    }
}