using System;
using System.IO;
using System.Linq;
using Azure.Storage.Queues;
using NeuralStyle.Core.Cloud;
using NeuralStyle.Core.Features;
using NeuralStyle.Core.Imaging;
using NeuralStyle.Core.Model;

namespace NeuralStyle.Console
{
    public class NeuralStyleTransfer
    {
        public static void Start(string basePath)
        {
            var queue = Factory.ConstructQueue("jobs-stylize");

            var stylePath = $@"{basePath}\style";
            var inPath = $@"{basePath}\in";
            var videoPath = $@"{basePath}\video";
            var sharePath = $@"C:\Data\images\share";

            var outPath = $@"{basePath}\out";
            var resultPath = $@"{outPath}\result";

            var singleStyle = new[] { $@"{stylePath}\surjyasis_basu_01.jpg" };
            var todoStyles = Directory.GetFiles($@"{stylePath}\todo", "*.jpg");
            var allStyles = Directory.GetFiles(stylePath, "*.jpg");
            var specificStyles = Directory.GetFiles(stylePath, "crow*.jpg");

            var allIn = Directory.GetFiles(inPath, "kroatien*.jpg");
            var singlePic = new[] { $@"{inPath}\woman_03.jpg", $@"{inPath}\woman_04.jpg", $@"{inPath}\woman_05.jpg" };

            var settings = GetDefaultSettings();

            //UpdateNames.Ensure_Correct_Filenames(images);
            //SortImages.SortNewImages(outPath, "*.jpg", $@"C:\Data\images\out");

            var videoName = $@"close_up_01_move01";
            var specificStylesInShare = Directory.GetFiles(sharePath, "close_up_01*.jpg").ToList().Select(image => image.GetTags().Style).Select(inStyle => $@"{stylePath}\{inStyle}.jpg").Distinct().Where(File.Exists).ToArray();

            //CreateVideoFrames(queue, settings, specificStylesInShare,  basePath, videoPath, videoName);

            //CreateJobs.CreateMissing(queue, inDonePath, stylePath, outPath, settings);
            //SortImages.CreateMissingHardlinkgs(outPath);

            CreateJobs.CreateNew(queue, allIn, allStyles, settings, basePath, outPath);
            //CreateJobs.CreateNew(queue, allInDone, singleStyle, settings);
            //CreateJobs.CreateNew(queue, allInDone, todoStyles, settings);

        }

        private static void CreateVideoFrames(QueueClient queue, JobSettings settings, string[] styles, string basePath, string videoPath, string videoName)
        {
            var inVideoImages = Directory.GetFiles($@"{videoPath}\{videoName}\in", "*.jpg");

            foreach (var style in styles)
            {
                var styleName = Path.GetFileName(style).Split(new[] { "." }, StringSplitOptions.None)[0];
                CreateJobs.CreateNew(queue, inVideoImages, new[] { style }, settings, basePath, $@"{videoPath}\{videoName}\styles\{styleName}");
            }
        }

        private static JobSettings GetDefaultSettings()
        {
            var settings = new JobSettings
            {
                Size = 1000,
                StyleWeight = 1e5,
                ContentWeight = 3e4,
                TvWeight = 1e1,
                Model = "vgg19",
                Optimizer = "lbfgs",
                Iterations = 500,
                Init = "content",
            };


            //var settings = new JobSettings
            //{
            //     Size = 1000,
            // StyleWeight = 1e5,
            //ContentWeight = 3e4,
            //TvWeight = 1,
            //Model = "vgg19",
            //Optimizer = "lbfgs",
            //Iterations = 500,
            //Init = "content",
            //};
            return settings;
        }
    }
}