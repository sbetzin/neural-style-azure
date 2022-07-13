﻿using System.IO;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
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
            var singleStyle = new[] { $@"{stylePath}\kandinsky_bayerisches_dorf_mit_feld.jpg", };
            var singleShare = new[] { $@"{sharePath}\norwegen_2-elena_prokopenko_tanz7-1200px_cw_0.01_sw_5_tvw_0.001_tmpw_200_clf_1_iter_500_origcolor_0.jpg" };


            var testPicsForStyleTest = new[] {
                $@"{inPath}\done\helen_gadjilova_01.jpg",
                $@"{inPath}\done\helen_gadjilova_02.jpg",
                $@"{inPath}\done\helen_gadjilova_03.jpg",
                $@"{inPath}\done\helen_gadjilova_04.jpg",
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

            //var settings = new JobSettings
            //{
            //    Size = 1000,
            //    StyleWeight = 1e6,
            //    ContentWeight = 1e1,
            //    TvWeight = 1e0,
            //    Model = "vgg19",
            //    Optimizer = "lbfgs",
            //    Iterations = 500,
            //    Init = "content",
            //};


            var settings = new JobSettings
            {
                Size = 1000,
                StyleWeight = 4e1,
                ContentWeight = 1e8,
                TvWeight = 1e0,
                Model = "vgg19",
                Optimizer = "lbfgs",
                Iterations = 500,
                Init = "content",
            };


            //var settings = new JobSettings
            //{
            //    Size = 1000,
            //    StyleWeight = 1e5,
            //    ContentWeight = 1e0,
            //    TvWeight = 1e-1,
            //    Model = "vgg19",
            //    Optimizer = "lbfgs",
            //    Iterations = 500,
            //    Init = "content",
            //};

            //var settings = new JobSettings
            //{
            //    Size = 1000,
            //    StyleWeight = 1e5,
            //    ContentWeight = 1e4,
            //    TvWeight = 1e0,
            //    Model = "vgg19",
            //    Optimizer = "adam",
            //    Iterations = 500,
            //    Init = "content",
            //};

            //PostInstaMessage();

            //UpdateNames.Ensure_Correct_Filenames(images);
            //SortImages.SortNewImages(@"C:\Users\gensb\OneDrive\neuralimages", "*1200px*.jpg", outPath);

            //CreateWebpages.CreateAll(webContainer, sharePath, webPath, templateFile);
            //CreateMiningMetaData.CreateTextFile(mintPath, "Girl Playing Chess");


            //CreateJobs.CreateMissing(container, queue, inDonePath, stylePath, outPath, settings);
            //SortImages.CreateMissingHardlinkgs(outPath);

            //CreateSeries(queue, container, singlePic, singleStyle);


            //CreateGenerativeArt(container, queue, images);

            //CreateJobs.CreateNew(container, queue, allIn, allStyles, settings);

            //CreateJobs.CreateNew(container, queue, allInDone, singleStyle, settings);

            CreateJobs.CreateNew(container, queue, singlePic, singleStyle, settings);
            //CreateJobs.CreateNew(container, queue, testPicsForStyleTest, todoStyles, settings);

            //CreateJobs.CreateNew(container, priorityQueue, testPicsForStyleTest, singleStyle, settings);

            //CreateJobs.CreateNew(container, priorityQueue, singlePic, bestStyles, settings);


            Logger.Log("");
            Logger.Log("Done");
            //System.Console.ReadKey();
        }

        private static void CreateSeries(QueueClient queue, BlobContainerClient container, string[] singlePic, string[] singleStyle)
        {
            var min = 1e-1;
            var max = 1e1;
            var count = 1;

            var stepSize = (max - min) / count;

            for (var step = 1; step <= count; step++)
            {
                var settings = new JobSettings
                {
                    Size = 1000,
                    ContentWeight = max,
                    StyleWeight = min + step * stepSize,
                    TvWeight = 1e0,
                    Model = "vgg19",
                    Optimizer = "lbfgs",
                    Iterations = 500,
                    Init = "content",
                };

                CreateJobs.CreateNew(container, queue, singlePic, singleStyle, settings);
            }

        }

        private static void CreateGenerativeArt(BlobContainerClient container, QueueClient queue, string images)
        {
            var settingsMix = new JobSettings
            {
                Size = 400,
                ContentWeight = 1e9,
                StyleWeight = 1e9,
                TvWeight = 1e-2,
                Model = "vgg19",
                Optimizer = "lbfgs",
                Iterations = 500,
                Init = "random",
            };

            var settingsTransfer = new JobSettings
            {
                Size = 1000,
                StyleWeight = 1e5,
                ContentWeight = 1e0,
                TvWeight = 1e0,
                Model = "vgg19",
                Optimizer = "lbfgs",
                Iterations = 500,
                Init = "content",
            };

            //CreateJobs.CreateNew(container, queue, new[] { $@"{images}\genart\d1\diana_fedoriaka.jpg" }, new[] { $@"{images}\genart\d1\yana_hryhorenko.jpg" }, settingsTransfer);
            //CreateJobs.CreateNew(container, queue, new[] { $@"{images}\genart\d2\oliakoval.jpg" }, new[] { $@"{images}\genart\d2\masha_reva.jpg" }, settingsTransfer);
            //CreateJobs.CreateNew(container, queue, new[] { $@"{images}\genart\d3\veronique.jpg" }, new[] { $@"{images}\genart\d3\pasha_photo_from_painting.jpg" }, settingsTransfer);
            //CreateJobs.CreateNew(container, queue, new[] { $@"{images}\genart\r1\woman_window.jpg" }, new[] { $@"{images}\genart\r1\monsters.jpg" }, settingsTransfer);
            //CreateJobs.CreateNew(container, queue, new[] { $@"{images}\genart\r1\woman_window.jpg" }, new[] { $@"{images}\genart\r1\superposition.jpg" }, settingsTransfer);
            //CreateJobs.CreateNew(container, queue, new[] { $@"{images}\genart\r2\volobevza.jpg" }, new[] { $@"{images}\genart\r2\vpidust.jpg" }, settingsTransfer);
            //CreateJobs.CreateNew(container, queue, new[] { $@"{images}\genart\r2\vpidust.jpg" }, new[] { $@"{images}\genart\r2\volobevza.jpg" }, settingsTransfer);
            //CreateJobs.CreateNew(container, queue, new[] { $@"{images}\genart\r3\zhannet_podobed.jpg" }, new[] { $@"{images}\genart\r3\krispodobed.jpg" }, settingsTransfer);
            //CreateJobs.CreateNew(container, queue, new[] { $@"{images}\genart\r3\krispodobed.jpg" }, new[] { $@"{images}\genart\r3\zhannet_podobed.jpg" }, settingsTransfer);

        }

        private static void PostInstaMessage()
        {
            var text = @"Pic of the day!
Original photo was taken in Norway - Stavanger 

#web3 #xrp #xrpnft #xrpnfts #sologenic #digitalart # #nftcommunity #nft #nftcollector #nftcollectors #nftcollectibles #nftart #nft #crypto";
            //InstagramAdapter.NewPost(singleShare[0], text).Wait();

            //UpdateNames.Ensure_Correct_Filenames(images);
            //SortImages.SortNewImages(@"C:\Users\gensb\OneDrive\neuralimages", "*ove*.jpg", outPath);
            //CreateWebpages.CreateAll(webContainer, sharePath, webPath, templateFile);

            //CreateMiningMetaData.CreateTextFile(mintPath, "Girl Playing Chess");
        }
    }
}