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
        public static void Start()
        {
            var queue = Factory.ConstructQueue("jobs-stylize");

            var basePath = @"C:\Users\gensb\OneDrive\_nft";
            var stylePath = $@"{basePath}\style";
            var inPath = $@"{basePath}\in";
            var videoPath = $@"{basePath}\video";
            var sharePath = $@"{basePath}\share";

            var outPath = $@"{basePath}\out";
            var resultPath = $@"{outPath}\result";

            var singleStyle = new[] { $@"{stylePath}\crow_ayahuasca_2.jpg", };
            var todoStyles = Directory.GetFiles($@"{stylePath}\todo", "*.jpg");
            var allStyles = Directory.GetFiles(stylePath, "*.jpg");
            var specificStyles = Directory.GetFiles(stylePath, "crow*.jpg");

            var allIn = Directory.GetFiles(inPath, "*.jpg");
            var singlePic = new[] { $@"{inPath}\sergis_01.jpg" };

            var settings = GetDefaultSettings();

            //UpdateNames.Ensure_Correct_Filenames(images);
            SortImages.SortNewImages(resultPath, "*.jpg", outPath);

            var videoName = $@"lofoten_reine_slide";
            var specificStylesInShare = Directory.GetFiles(sharePath, "lofoten_reine*.jpg").ToList().Select(image => image.GetTags().Style).Select(inStyle => $@"{stylePath}\{inStyle}.jpg").Distinct().Where(File.Exists).ToArray();

            CreateVideoFrames(queue, settings, specificStylesInShare,  basePath, videoPath, videoName);

            //CreateJobs.CreateMissing(queue, inDonePath, stylePath, outPath, settings);
            //SortImages.CreateMissingHardlinkgs(outPath);

            //CreateJobs.CreateNew(queue, allIn, singleStyle, settings);
            //CreateJobs.CreateNew(queue, allInDone, singleStyle, settings);
            //CreateJobs.CreateNew(queue, allInDone, todoStyles, settings);

        }

        private static void CreateVideoFrames(QueueClient queue, JobSettings settings, string[] styles, string basePath, string videoPath, string videoName)
        {
            var inVideoImages = Directory.GetFiles($@"{videoPath}\{videoName}\in", "*.jpg");
           
            foreach (var style in styles)
            {
                var styleName = Path.GetFileName(style).Split(new[] { "." }, StringSplitOptions.None)[0];
                CreateJobs.CreateNew(queue, inVideoImages, new[] { style }, settings, basePath, $@"{videoPath}\{videoName}\out\{styleName}");
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