﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

            var stylePath = @"C:\Data\images\style";
            var inPath = @"C:\Data\images\in";

            var kandinskyStyles = Directory.GetFiles(stylePath, "kandinsky_*.jpg");
            var modernArtStyle = Directory.GetFiles(stylePath, "modern_art_*.jpg");
            var danceStyles = Directory.GetFiles(stylePath, "elena_prokopenko_*.jpg");
            var goghStyles = Directory.GetFiles(stylePath, "gogh_*.jpg");
            var afremovStyles = Directory.GetFiles(stylePath, "Leonid_afremov*.jpg");
            var comicStyles = Directory.GetFiles(stylePath, "comic_*.jpg");
            var picassoStyles = Directory.GetFiles(stylePath, "picasso_*.jpg");
            var corinth = Directory.GetFiles(stylePath, "lovis_corinth_*.jpg");
            var allStyles = Directory.GetFiles(stylePath, "*.jpg");

            var ana = Directory.GetFiles(inPath, "ana*.jpg");
            var sebastian = Directory.GetFiles(inPath, "sebastian_*.jpg");

            var newPics = new[] { $@"{inPath}\kraemerbruecke.jpg" };

            RunIt(blobContainer, queue, newPics, allStyles, 500, 1500, 50.0, 1, 1500, 100, true);
        }

        private static void RunIt(CloudBlobContainer blobContainer, CloudQueue queue, string[] images, string[] styles, int iterations, int size, double styleWeight, int styleScale, int tileSize, int tileOverlap, bool useOriginalColors)
        {
            UploadImages(blobContainer, images);
            UploadImages(blobContainer, styles);

            CreateJobs(queue, images, styles, iterations, size, styleWeight, styleScale, tileSize, tileOverlap, useOriginalColors).Wait();
        }

        private static void UploadImages(CloudBlobContainer blobContainer, string[] images)
        {
            Console.WriteLine($"checking {images.Length} images for upload");
            foreach (var image in images)
            {
                image.UploadToBlob(blobContainer).Wait();
            }
        }


        private static async Task CreateJobs(CloudQueue queue, IEnumerable<string> sourceFiles, IEnumerable<string> styleFiles, int iterations, int size, double styleWeight, double styleScale, int tileSize, int tileOverlap, bool useOriginalColors)
        {
            var jobs = sourceFiles.Product(styleFiles).ToList();

            Console.WriteLine($"Creating {jobs.Count} jobs");
            foreach (var (sourceFile, styleFile) in jobs)
            {
                await CreateJob(queue, sourceFile, styleFile, iterations, size, styleWeight, styleScale, useOriginalColors, tileSize, tileOverlap);
            }
        }

        private static async Task CreateJob(CloudQueue queue, string sourceFile, string styleFile, int iterations, int size, double styleWeight, double styleScale, bool useOriginalColors, int tileSize, int tileOverlap)
        {
            var job = new Job { SourceName = Path.GetFileName(sourceFile), StyleName = Path.GetFileName(styleFile), Iterations = iterations, Size = size, StyleWeight = styleWeight, StyleScale = styleScale, UseOriginalColors = useOriginalColors, TileSize = tileSize, TileOverlap = tileOverlap };
            job.TargetName = CreateTargetName(job);

            var json = JsonConvert.SerializeObject(job);
            var message = new CloudQueueMessage(json);

            await queue.AddMessageAsync(message);

            Console.WriteLine($"   added job for image {sourceFile} with style {styleFile}");
        }

        private static string CreateTargetName(Job job)
        {
            FormattableString name = $"{Path.GetFileNameWithoutExtension(job.SourceName)}_{Path.GetFileNameWithoutExtension(job.StyleName)}_{job.Size}px_sw_{job.StyleWeight:F1}_ss_{job.StyleScale:F1}_iter_{job.Iterations}_origcolor_{job.UseOriginalColors}.jpg";

            return name.ToString(new CultureInfo("en-US"));
        }
    }
}