using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using NeuralStyle.ConsoleClient.Model;
using Newtonsoft.Json;

namespace NeuralStyle.ConsoleClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            var queueClient = storageAccount.CreateCloudQueueClient();
            var blobClient = storageAccount.CreateCloudBlobClient();

            var queue = queueClient.GetQueueReference("jobs");
            queue.EncodeMessage = false;

            var blobContainer = blobClient.GetContainerReference("images");

            var images = @"C:\Data\images";
            var stylePath = @"C:\Data\images\style";
            var inPath = @"C:\Data\images\in";
            var outPath = @"C:\Data\images\out";

            ImageAdapter.Ensure_Correct_Filenames(images);
            //Features.Find_Missing_Combinations(inPath, stylePath, outPath);
            //Features.Update_Tags_in_Existing_Images(inPath, stylePath, outPath);

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

            var newPics = new[] {$@"{inPath}\eric_drache.jpg", $@"{inPath}\eric_tauchen.jpg" , $@"{inPath}\familie_dino_ziehen.jpg" };

            var newStyle = new[] { $@"{stylePath}\elena_prokopenko_tanz7.jpg" };

            RunIt(blobContainer, queue, newPics, allStyles, 500, 900, 0.1, 50.0, false);
        }

       

   

        private static void RunIt(CloudBlobContainer blobContainer, CloudQueue queue, string[] images, string[] styles, int iterations, int size, double contentWeight, double styleWeight, bool useOriginalColors)
        {
            UploadImages(blobContainer, images);
            UploadImages(blobContainer, styles);

            CreateJobs(queue, images, styles, iterations, size, styleWeight, contentWeight, useOriginalColors).Wait();
        }

        private static void UploadImages(CloudBlobContainer blobContainer, string[] images)
        {
            Console.WriteLine($"checking {images.Length} images for upload");
            foreach (var image in images)
            {
                image.UploadToBlob(blobContainer).Wait();
            }
        }

        private static async Task CreateJobs(CloudQueue queue, IEnumerable<string> sourceFiles, IEnumerable<string> styleFiles, int iterations, int size, double contentWeight, double styleWeight, bool useOriginalColors)
        {
            var jobs = sourceFiles.Product(styleFiles).ToList();

            Console.WriteLine($"Creating {jobs.Count} jobs");
            foreach (var (sourceFile, styleFile) in jobs)
            {
                await CreateJob(queue, sourceFile, styleFile, iterations, size, styleWeight, contentWeight, useOriginalColors);
            }
        }

        private static async Task CreateJob(CloudQueue queue, string sourceFile, string styleFile, int iterations, int size, double contentWeight, double styleWeight, bool useOriginalColors)
        {
            var job = new Job { SourceName = Path.GetFileName(sourceFile), StyleName = Path.GetFileName(styleFile), Iterations = iterations, Size = size, StyleWeight = styleWeight, ContentWeight = contentWeight, UseOriginalColors = useOriginalColors };
            job.TargetName = CreateTargetName(job);

            var json = JsonConvert.SerializeObject(job);
            var message = new CloudQueueMessage(json);

            await queue.AddMessageAsync(message);

            Console.WriteLine($"   added job for image {sourceFile} with style {styleFile}");
        }

        private static string CreateTargetName(Job job)
        {
            FormattableString name = $"{Path.GetFileNameWithoutExtension(job.SourceName)}_{Path.GetFileNameWithoutExtension(job.StyleName)}_{job.Size}px_cw_{job.ContentWeight:G}_sw_{job.StyleWeight:G}_iter_{job.Iterations}_origcolor_#origcolor#.jpg";

            return name.ToString(new CultureInfo("en-US"));
        }
    }
}