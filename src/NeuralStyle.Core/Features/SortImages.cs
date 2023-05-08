using System;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text.RegularExpressions;
using AngleSharp.Text;
using FFMpegCore;
using NeuralStyle.Core.Imaging;
using NeuralStyle.Core.Links;

namespace NeuralStyle.Core.Features
{
    public static class SortImages
    {
        public static void SortNewImages(string sortImagePath, string searchPattern, string outPath)
        {
            var images = sortImagePath.Get_All_Images(searchPattern, SearchOption.AllDirectories);

            foreach (var image in images)
            {
                SortImage(image, outPath);
            }
        }

        private static void SortImage(string image, string outPath)
        {
            Logger.Log($"Sorting {image}");
            var tags = image.GetTags();
            var fileName = Path.GetFileName(image);

            if (tags.In == null || tags.Style == null)
            {
                File.Delete(image);
                return;
            }

            SortStyledImage(image, outPath, tags, fileName);
        }

        private static string GetFrameImageOrigColor(string image)
        {
            var match = Regex.Match(image, @"_(c[01])_");

            return match.Groups[1].Value;
        }

        private static void SortStyledImage(string image, string outPath, (string In, string Style) tags, string fileName)
        {
            var inFile = Path.Combine(outPath, "name", tags.In, fileName);
            var styleFile = Path.Combine(outPath, "style", tags.Style, fileName);

            inFile.EnsureDirectoryExists();
            styleFile.EnsureDirectoryExists();

            if (File.Exists(inFile))
            {
                File.Delete(inFile);
            }

            File.Move(image, inFile);

            HardLink.Create(inFile, styleFile, true);
        }
        
        public static void CreateMissingHardlinkgs(string outPath)
        {
            var namePath = Path.Combine(outPath, "name");

            var images = namePath.Get_All_Images(SearchOption.AllDirectories);

            foreach (var image in images)
            {
                CreateMissingHardlink(outPath, image);
            }
        }

        private static void CreateMissingHardlink(string outPath, string image)
        {
            var fileName = Path.GetFileName(image);
            var tags = image.GetTags();
            if (tags.Style == null)
            {
                return;
            }

            var styleFile = Path.Combine(outPath, "style", tags.Style, fileName);
            styleFile.EnsureDirectoryExists();

            HardLink.Create(image, styleFile, true);
        }
    }
}