using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

            var images = blobClient.GetContainerReference("images");

            CreateBatch(images, queue, @"C:\Data\images\in\ana.jpg", @"C:\Data\images\style\kandinsky_schwarz-und-violett.jpg", 500, 1400, 50.0, 1.0, 750, 100).Wait();
        }

        private static async Task CreateBatch(CloudBlobContainer images, CloudQueue queue, string source, string style, int iterations, int size, double styleWeight, double styleScale, int tileSize, int tileOverlap)
        {
            var sourceFiles = source.GetFiles();
            var styleFiles = style.GetFiles();

            await CreateJobs(images, queue, sourceFiles, styleFiles, iterations, size, styleWeight, styleScale, tileSize, tileOverlap);
        }

        private static async Task CreateJobs(CloudBlobContainer images, CloudQueue queue, IEnumerable<string> sourceFiles, IEnumerable<string> styleFiles, int iterations, int size, double styleWeight, double styleScale, int tileSize, int tileOverlap)
        {
            foreach (var (sourceFile, styleFile) in sourceFiles.Product(styleFiles))
            {
                await CreateJob(images, queue, sourceFile, styleFile, iterations, size, styleWeight, styleScale, true, tileSize, tileOverlap);
            }
        }

        private static async Task CreateJob(CloudBlobContainer images, CloudQueue queue, string source, string style, int iterations, int size, double styleWeight, double styleScale, bool useOriginalColors, int tileSize, int tileOverlap)
        {
            var sourceName = source.UploadToBlob(images).Result;
            var styleName = style.UploadToBlob(images).Result;

            var job = new Job { SourceName = sourceName, StyleName = styleName, Iterations = iterations, Size = size, StyleWeight = styleWeight, StyleScale = styleScale, UseOriginalColors = useOriginalColors, TileSize = tileSize, TileOverlap = tileOverlap };
            job.TargetName = CreateTargetName(job);

            var json = JsonConvert.SerializeObject(job);
            var message = new CloudQueueMessage(json);

            await queue.AddMessageAsync(message);

            Console.WriteLine($"Created job for {source} with style {style}");
        }

        private static string CreateTargetName(Job job)
        {
            FormattableString name = $"{Path.GetFileNameWithoutExtension(job.SourceName)}_{Path.GetFileNameWithoutExtension(job.StyleName)}_{job.Size}px_sw_{job.StyleWeight:F1}_ss_{job.StyleScale:F1}_iter_{job.Iterations}_origcolor_{job.UseOriginalColors}.jpg";

            return name.ToString(new CultureInfo("en-US"));
        }
    }
}