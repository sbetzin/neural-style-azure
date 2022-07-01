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

            var queue = Factory.ConstructQueue("test");
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

            var singlePic = new[] { $@"{inPath}\done\helen_gadjilova_04.jpg" };
            var singleStyle = new[]
            {
                $@"{stylePath}\todo\guillermo_arismendi_test_01.jpg",
            };
            var singleShare = new[] { $@"{sharePath}\norwegen_2-elena_prokopenko_tanz7-1200px_cw_0.01_sw_5_tvw_0.001_tmpw_200_clf_1_iter_500_origcolor_0.jpg" };


            var testPicsForStyleTest = new[] {
                $@"{inPath}\done\helen_gadjilova_01.jpg",
                $@"{inPath}\done\helen_gadjilova_02.jpg",
                $@"{inPath}\done\helen_gadjilova_03.jpg",
                $@"{inPath}\done\helen_gadjilova_04.jpg",
                $@"{inPath}\done\helen_gadjilova_05.jpg",
                $@"{inPath}\done\ana-blume.jpg",
                $@"{inPath}\done\lofoten_reine.jpg",
                $@"{inPath}\done\ana-schwanger.jpg",
                $@"{inPath}\done\norwegen_2.jpg",
                $@"{inPath}\done\blumen-02.jpg",
                $@"{inPath}\done\angel-with-sword.jpg",
                $@"{inPath}\done\friesland-muehle.jpg",

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

            var text = @"Pic of the day!
Original photo was taken in Norway - Stavanger 

#web3 #xrp #xrpnft #xrpnfts #sologenic #digitalart # #nftcommunity #nft #nftcollector #nftcollectors #nftcollectibles #nftart #nft #crypto";
            //InstagramAdapter.NewPost(singleShare[0], text).Wait();

            //UpdateNames.Ensure_Correct_Filenames(images);
            //SortImages.SortNewImages(@"C:\Users\gensb\OneDrive\neuralimages", "*ove*.jpg", outPath);
            //CreateWebpages.CreateAll(webContainer, sharePath, webPath, templateFile);

            //CreateMiningMetaData.CreateTextFile(mintPath, "Girl Playing Chess");


            //var settings = new JobSettings
            //{
            //    Size = 900,
            //    StyleWeight = 1e5,
            //    ContentWeight = 1e0,
            //    TvWeight = 1e0,
            //    Model = "vgg19",
            //    Optimizer = "lbfgs",
            //    Iterations = 500,
            //    Init = "content",
            //};

            var settings = new JobSettings
            {
                Size = 1200,
                StyleWeight = 1e7,
                ContentWeight = 1e1,
                TvWeight = 1e2,
                Model = "vgg19",
                Optimizer = "adam",
                Iterations = 500,
                Init = "content",
            };


            //CreateJobs.CreateMissing(container, queue, inDonePath, stylePath, outPath, settings);
            //SortImages.CreateMissingHardlinkgs(outPath);

            //CreateJobs.CreateNew(container, queue, allIn, allStyles, settings);

            //CreateJobs.CreateNew(container, queue, allInDone, singleStyle, settings);

            //CreateJobs.CreateNew(container, priorityQueue, singlePic, singleStyle, settings);
            CreateJobs.CreateNew(container, queue, singlePic, singleStyle, settings);

            //CreateJobs.CreateNew(container, priorityQueue, testPicsForStyleTest, singleStyle, settings);

            //CreateJobs.CreateNew(container, priorityQueue, singlePic, bestStyles, settings);


            Logger.Log("");
            Logger.Log("Done");


            //System.Console.ReadKey();
        }
    }
}