using System.IO;
using NeuralStyle.Core;
using NeuralStyle.Core.Cloud;
using NeuralStyle.Core.Features;
using NeuralStyle.Core.Instagram;
using NeuralStyle.Core.Model;

namespace NeuralStyle.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Logger.NewLog += System.Console.WriteLine;

            var queue = Factory.ConstructQueue("jobs");
            var priorityQueue = Factory.ConstructQueue("priority-jobs");
            var container = Factory.ConstructContainer("images");
            var webContainer = Factory.ConstructContainer("$web");

            var images = @"C:\Data\images";
            var stylePath = $@"{images}\style";
            var inPath = $@"{images}\in";
            var inDonePath = $@"{images}\in\done";
            var outPath = $@"{images}\out";
            var outScaledPath = $@"{images}\out_scaled";
            var webPath = $@"{images}\web\pages";
            var sharePath = $@"{images}\share";
            var templateFile = $@"{images}\web\template.html";
            var mintPath = $@"{images}\mint\girl-playing-chess";

            var allStyles = Directory.GetFiles(stylePath, "*.jpg");
            var todoStyles = Directory.GetFiles($@"{stylePath}\todo");
            var monet = Directory.GetFiles(stylePath, "*monet_jpg");

            var allIn = Directory.GetFiles(inPath, "*.jpg");
            var allInDone = Directory.GetFiles(inDonePath, "*.jpg");

            var singlePic = new[] { $@"{inPath}\todo\kris.jpg" };
            var singleStyle = new[] { $@"{stylePath}\todo\nft_art_01.jpg" };
            var singleShare = new[] { $@"{sharePath}\bird_in_the_wood-dance_1-1200px_cw_0.01_sw_500_tvw_0.001_tmpw_200_clf_1_iter_500_origcolor_0.jpg" };
            

            var testPicsForStyleing = new[] { $@"{inPath}\done\ana-lolita.jpg", $@"{inPath}\done\norwegen_2.jpg", $@"{inPath}\done\dana_1.jpg" };
            var bestStyles = new[]
            {
                $@"{stylePath}\bob_marley.jpg",
                $@"{stylePath}\anca_stefanescu_pegasus.jpg",
                $@"{stylePath}\dieu_deep_in_my.jpg",
                $@"{stylePath}\hume_disin_die_glaubwuerdigkeit.jpg",
                $@"{stylePath}\kandinsky_schwarz_und_violett.jpg",
                $@"{stylePath}\kandinsky_bayerisches_dorf_mit_feld.jpg",
                $@"{stylePath}\cat1.jpg",
                $@"{stylePath}\elena_prokopenko_tanz7.jpg"
            };

            //InstagramAdapter.Test(singleShare[0]).Wait();

            UpdateNames.Ensure_Correct_Filenames(images);
            CreateWebpages.CreateAll(webContainer, sharePath, webPath, templateFile);

            //SortImages.SortNewImages(@"C:\Users\gensb\OneDrive\neuralimages", outPath);
            //CreateMiningMetaData.CreateTextFile(mintPath, "Girl Playing Chess");


            var settings = new JobSettings
            {
                Iterations = 500,
                Size = 1200,
                StyleWeight = 500,
                ContentWeight = 0.01,
                TvWeight = 0.001,
                TemporalWeight = 200,
                ContentLossFunction = 1
            };


            //CreateJobs.CreateMissing(container, queue, inDonePath, stylePath, outPath, settings);
            //SortImages.CreateMissingHardlinkgs(outPath);


            //CreateJobs.CreateNew(container, queue, allIn, allStyles, settings);

            //CreateJobs.CreateNew(container, queue, allInDone, singleStyle, settings);

            //CreateJobs.CreateNew(container, priorityQueue, singlePic, singleStyle, settings);

            //CreateJobs.CreateNew(container, priorityQueue, singlePic, todoStyles, settings);

            //CreateJobs.CreateNew(container, priorityQueue, singlePic, bestStyles, settings);


            Logger.Log("");
            Logger.Log("Done");


            System.Console.ReadKey();
        }
    }
}