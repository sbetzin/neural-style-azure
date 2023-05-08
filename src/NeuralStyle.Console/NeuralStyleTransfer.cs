using System.IO;
using System.Linq;
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
            var queue = Factory.ConstructQueue("test");
            var priorityQueue = Factory.ConstructQueue("priority-jobs");

            var container = Factory.ConstructContainer("images");
            //var webContainer = Factory.ConstructContainer("$web");

            var images = @"C:\Data\images";
            var basePath = @"C:\Users\gensb\OneDrive\_nft\";
            var video = @"C:\Users\gensb\OneDrive\_nft\video";
            var stylePath = $@"{images}\style";
            var inPath = $@"{images}\in";
            var videoPath = $@"{images}\video";
            var inTodoPath = $@"{images}\in\todo";
            var inVideoImagesPath = $@"{video}\lofoten_reine_slide\in";
            var outPath = $@"{images}\out";
            var sharePath = $@"{images}\share";

            var singleStyle = new[] { $@"{stylePath}\crow_ayahuasca_2.jpg", };
            var todoStyles = Directory.GetFiles($@"{stylePath}\todo", "*.jpg");
            var allStyles = Directory.GetFiles(stylePath, "*.jpg");
            var specificStyles = Directory.GetFiles(stylePath, "crow*.jpg");

            var allIn = Directory.GetFiles(inPath, "*.jpg");
            var inVideoImages = Directory.GetFiles(inVideoImagesPath, "*.jpg");

            var singlePic = new[] { $@"{inPath}\sergis_01.jpg" };
            var specificStylesInShare = Directory.GetFiles(sharePath, "lofoten_reine*.jpg").ToList().Select(image => image.GetTags().Style).Select(inStyle => $@"{stylePath}\{inStyle}.jpg").Distinct().ToArray();

            specificStylesInShare = specificStylesInShare.Where(File.Exists).ToArray();

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


            //UpdateNames.Ensure_Correct_Filenames(images);
            //SortImages.SortNewImages(@"C:\Users\gensb\OneDrive\neuralimages", "frame*.jpg", outPath, videoPath);


            //CreateJobs.CreateMissing(container, queue, inDonePath, stylePath, outPath, settings);
            //SortImages.CreateMissingHardlinkgs(outPath);

            //CreateSeries.Fixed(queue, container, singlePic, singleStyle);

            //CreateJobs.CreateNew(container, queue, allIn, singleStyle, settings);
            CreateJobs.CreateNew(container, queue, inVideoImages, specificStylesInShare, settings, basePath);

            //CreateJobs.CreateNew(container, queue, allInDone, singleStyle, settings);
            //CreateJobs.CreateNew(container, queue, allInDone, todoStyles, settings);

        }
    }
}