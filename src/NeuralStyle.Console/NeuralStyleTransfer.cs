using System;
using System.IO;
using Azure.Storage.Queues;
using NeuralStyle.Core;
using NeuralStyle.Core.Cloud;
using NeuralStyle.Core.Features.NeuralStyleTransfer;
using NeuralStyle.Core.Model;

namespace NeuralStyle.Console
{
    public class NeuralStyleTransfer
    {
        public static void Start()
        {
            var stylePath = BasePathes.StylePath();
            var inPath = BasePathes.InPath();

            var allStyles = BasePathes.GetAllStyles();
            var inImages = BasePathes.GetInImages("kroatien_06*.jpg");

            var specificStyles = Directory.GetFiles(stylePath, "crow*.jpg");
            var singlePic = new[] { $@"{inPath}\woman_03.jpg", $@"{inPath}\woman_04.jpg", $@"{inPath}\woman_05.jpg" };
            var singleStyle = new[] { $@"{stylePath}\surjyasis_basu_01.jpg" };

            var settings = CreateSettings.GetDefaultSettings();

            //UpdateNames.Ensure_Correct_Filenames(images);
            //SortImages.SortNewImages(outPath, "*.jpg", $@"C:\Data\images\out");

            var videoName = @"close_up_01_move01";
            //var specificStylesInShare = Directory.GetFiles(sharePath, "close_up_01*.jpg").ToList().Select(image => image.GetTags().Style).Select(inStyle => $@"{stylePath}\{inStyle}.jpg").Distinct().Where(File.Exists).ToArray();

            //CreateVideoFrames(settings, specificStylesInShare,  basePath, videoPath, videoName);

            //CreateJobs.CreateMissing(inDonePath, stylePath, outPath, settings);
            //SortImages.CreateMissingHardlinkgs(outPath);

            CreateJobs.CreateNew(inImages, allStyles, settings, BasePathes.BasePath(), BasePathes.OutPath());
            //CreateJobs.CreateNew(allInDone, singleStyle, settings);
            //CreateJobs.CreateNew(allInDone, todoStyles, settings);
        }

        private static void CreateVideoFrames(JobSettings settings, string[] styles, string basePath, string videoPath, string videoName)
        {
            var inVideoImages = Directory.GetFiles($@"{videoPath}\{videoName}\in", "*.jpg");

            foreach (var style in styles)
            {
                var styleName = Path.GetFileName(style).Split(new[] { "." }, StringSplitOptions.None)[0];
                CreateJobs.CreateNew(inVideoImages, new[] { style }, settings, basePath, $@"{videoPath}\{videoName}\styles\{styleName}");
            }
        }


    }
}