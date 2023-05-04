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
            var video = @"C:\Data\Google Drive\nft\video";
            var stylePath = $@"{images}\style";
            var inPath = $@"{images}\in";
            var videoPath = $@"{images}\video";
            var inDonePath = $@"{images}\in\done";
            var inTodoPath = $@"{images}\in\todo";
            var inVideoImagesPath = $@"{video}\norwegen-19_move\in";
            var outPath = $@"{images}\out";
            var outScaledPath = $@"{images}\out_scaled";
            var webPath = $@"{images}\web\pages";
            var sharePath = $@"{images}\share";
            var templateFile = $@"{images}\web\template.html";
            var mintPath = $@"{images}\mint\girl-playing-chess";
            var thumbsPath = $@"{images}\thumbs\ana_leon_01";

            var allStyles = Directory.GetFiles(stylePath, "*.jpg");
            var todoStyles = Directory.GetFiles($@"{stylePath}\todo");

            var allIn = Directory.GetFiles(inPath, "*.jpg");
            var allInDone = Directory.GetFiles(inDonePath, "*.jpg");
            //var inVideoImages = Directory.GetFiles(inVideoImagesPath, "*.jpg");

            var singlePic = new[] { $@"{inPath}\sergis_01.jpg" };
            var singleStyle = new[] { $@"{stylePath}\eugene_ivanov_2224.jpg", };
            var specificStylesInShare = Directory.GetFiles(sharePath, "norwegen-19*.jpg").ToList().Select(image => image.GetTags().Style).Select(inStyle => $@"{stylePath}\{inStyle}.jpg").Distinct().ToArray();

            specificStylesInShare = specificStylesInShare.Where(File.Exists).ToArray();

            var testPicsForStyleTest = new[]
            {
                $@"{inPath}\done\lofoten_reine.jpg",
                $@"{inPath}\done\ana-schwanger.jpg",
                $@"{inPath}\done\ana-lolita.jpg",
                $@"{inPath}\done\norwegen_2.jpg",
                $@"{inPath}\done\friesland-kanal.jpg",
                $@"{inPath}\done\friesland-muehle.jpg",
                $@"{inPath}\done\friesland-schiffe.jpg",
                $@"{inPath}\done\frau-02.jpg",
                $@"{inPath}\done\ove.jpg",
                $@"{inPath}\done\lofoten-12.jpg",
                $@"{inPath}\done\leon_01.jpg",
            };

            var bestStyles = new[]
            {
                $@"{stylePath}\bob_marley.jpg",
                $@"{stylePath}\anca_stefanescu_pegasus.jpg",
                $@"{stylePath}\dieu_deep_in_my.jpg",
                $@"{stylePath}\hume_disin_die_glaubwuerdigkeit.jpg",
                $@"{stylePath}\kandinsky_schwarz_und_violett.jpg",
                $@"{stylePath}\kandinsky_bayerisches_dorf_mit_feld.jpg",
                $@"{stylePath}\cat1.jpg",
                $@"{stylePath}\elena_prokopenko_tanz7.jpg",
                $@"{stylePath}\john_beckley_who_is_there.jpg",
                $@"{stylePath}\angel_botello_mother_and_child.jpg",
            };

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
            SortImages.SortNewImages(@"C:\Users\gensb\OneDrive\neuralimages", "frame*.jpg", outPath, videoPath);


            //CreateJobs.CreateMissing(container, queue, inDonePath, stylePath, outPath, settings);
            //SortImages.CreateMissingHardlinkgs(outPath);

            //CreateSeries.Fixed(queue, container, singlePic, singleStyle);
            //CreateSeries.FromThumbs(queue, container, thumbsPath, singleStyle);


            //CreateJobs.CreateNew(container, queue, allIn, specificStylesInShare, settings);
            //CreateJobs.CreateNew(container, queue, inVideoImages, specificStylesInShare, settings);

            //CreateJobs.CreateNew(container, queue, allInDone, singleStyle, settings);
            //CreateJobs.CreateNew(container, queue, allInDone, todoStyles, settings);

        }
    }
}