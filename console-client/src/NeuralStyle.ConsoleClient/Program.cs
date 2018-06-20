using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace NeuralStyle.ConsoleClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var queueName = "jobs-large";
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

            var allIn = Directory.GetFiles(inPath, "*.jpg");
            var ana = Directory.GetFiles(inPath, "ana*.jpg");
            var sebastian = Directory.GetFiles(inPath, "sebastian_*.jpg");
            var berge = Directory.GetFiles(inPath, "berge*.jpg");

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

            var newPics = new[] { $@"{inPath}\as_karrikatur.jpg" };
            var newStyle = new[] { $@"{stylePath}\dieu_deep_in_my_large.jpg" };

            ImageAdapter.Ensure_Correct_Filenames(images);

            Create_Missing_Jobs(container, queue, inPath, stylePath, outPath, 500, 1000, 0.01, 50.0);

            //Features.Update_Tags_in_Existing_Images(inPath, stylePath, outPath);
            //Features.FixExifTags(images);

            //RunIt(container, queue, newPics, allStyles, 500, 1000, 0.01, 50.0);
        }

        private static void Create_Missing_Jobs(CloudBlobContainer container, CloudQueue queue, string inPath, string stylePath, string outPath, int iterations, int size, double contentWeight, double styleWeight)
        {
            var allIn = Directory.GetFiles(inPath, "*.jpg");
            var allStyles = Directory.GetFiles(stylePath, "*.jpg");
            var missing = Features.Find_Missing_Combinations(inPath, stylePath, outPath);
            Console.WriteLine($"Found {missing.Count} missing combinations");

            UploadImages(container, allIn);
            UploadImages(container, allStyles);

            missing.ForEach(pair => queue.CreateJob(pair.In, pair.Style, iterations, size, contentWeight, styleWeight).Wait());
        }

        private static void RunIt(CloudBlobContainer blobContainer, CloudQueue queue, string[] images, string[] styles, int iterations, int size, double contentWeight, double styleWeight)
        {
            UploadImages(blobContainer, images);
            UploadImages(blobContainer, styles);

            CreateJobs(queue, images, styles, iterations, size, styleWeight, contentWeight).Wait();
        }

        private static void UploadImages(CloudBlobContainer blobContainer, string[] images)
        {
            Console.WriteLine($"checking {images.Length} images for upload");
            foreach (var image in images)
            {
                image.UploadToBlob(blobContainer).Wait();
            }
        }

        private static async Task CreateJobs(CloudQueue queue, IEnumerable<string> sourceFiles, IEnumerable<string> styleFiles, int iterations, int size, double contentWeight, double styleWeight)
        {
            var jobs = sourceFiles.Product(styleFiles).ToList();

            Console.WriteLine($"Creating {jobs.Count} jobs");
            foreach (var (sourceFile, styleFile) in jobs)
            {
                await queue.CreateJob(sourceFile, styleFile, iterations, size, styleWeight, contentWeight);
            }
        }
    }
}