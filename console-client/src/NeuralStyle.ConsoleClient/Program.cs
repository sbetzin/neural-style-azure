using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeuralStyle.ConsoleClient.Features;

namespace NeuralStyle.ConsoleClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var queueName = "jobs";
            var containerName = "images";

            var connectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString");
            var queue = QueueAdapter.GetAzureQueue(connectionString, queueName);
            var container = BlobAdapter.GetBlobContainer(connectionString, containerName);

            var images = @"C:\Data\images";
            var stylePath = @"C:\Data\images\style";
            var inPath = @"C:\Data\images\in";
            var outPath = @"C:\Data\images\out";

            var kandinskyStyles = Directory.GetFiles(stylePath, "kandinsky_*.jpg");
            var modernArtStyle = Directory.GetFiles(stylePath, "modern_art_*.jpg");
            var danceStyles = Directory.GetFiles(stylePath, "elena_prokopenko_*.jpg");
            var goghStyles = Directory.GetFiles(stylePath, "gogh_*.jpg");
            var afremovStyles = Directory.GetFiles(stylePath, "Leonid_afremov*.jpg");
            var comicStyles = Directory.GetFiles(stylePath, "comic_*.jpg");
            var picassoStyles = Directory.GetFiles(stylePath, "picasso_*.jpg");
            var corinth = Directory.GetFiles(stylePath, "lovis_corinth_*.jpg");
            var allStyles = Directory.GetFiles(stylePath, "*.jpg");
            var monet = Directory.GetFiles(stylePath, "*monet_jpg");
            var ivanov = Directory.GetFiles(stylePath, "eugene_ivanov_*.jpg");

            var allIn = Directory.GetFiles(inPath, "*.jpg");
            var ana = Directory.GetFiles(inPath, "ana*.jpg");
            var ana_maske = Directory.GetFiles(inPath, "ana_maske.jpg");
            var sebastian = Directory.GetFiles(inPath, "sebastian_*.jpg");
            var berge = Directory.GetFiles(inPath, "berge*.jpg");
            var vdma = Directory.GetFiles(inPath, "vdma_gruppe.jpg");


            var bestStyles = kandinskyStyles.Union(modernArtStyle).Union(picassoStyles).Union(new List<string>
            {
                $@"{stylePath}\abstract.jpg",
                $@"{stylePath}\hume_disin_die_glaubwuerdigkeit.jpg",
                $@"{stylePath}\albert_weisgerber_englischer_garten.jpg",
                $@"{stylePath}\candy.jpg",
                $@"{stylePath}\dieu_deep_in_my.jpg",
                $@"{stylePath}\elena_prokopenko_tanz7.jpg",
                $@"{stylePath}\expressionismus.jpg",
                $@"{stylePath}\lovis_corinth_walchensee.jpg",
                $@"{stylePath}\matisse_woman_with_hat.jpg",
                $@"{stylePath}\uta_welcker_annies_verwandtschaft.jpg",
                $@"{stylePath}\yosi_losaij_you_and_me.jpg",
            }).ToList();

            var newPics = new[] { $@"{inPath}\hund_gross.jpg" };
            var newStyle = new[] { $@"{stylePath}\abstract_7.jpg",$@"{stylePath}\abstract_8.jpg",$@"{stylePath}\abstract_9.jpg",$@"{stylePath}\abstract_10.jpg",$@"{stylePath}\abstract_11.jpg" };

            ImageAdapter.Ensure_Correct_Filenames(images);

            //SortImages.SortNewImages(@"C:\Data\OneDrive\neuralimages", outPath);

            //CreateJobs.CreateMissing(container, queue, inPath, stylePath, outPath, 500, 900, 0.01, 50.0);
            CreateJobs.CreateNew(container, queue, vdma, allStyles, 500, 750, 0.01, 50.0);
        }
    }
}