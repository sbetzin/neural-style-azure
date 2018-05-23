using System;
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

            CreateBatch(images, queue, @"C:\Data\images\in\Berge.jpg", @"C:\Data\images\style").Wait();

            //CreateSimple(images, queue).Wait();
        }

        private static async Task CreateBatch(CloudBlobContainer images, CloudQueue queue, string source, string style)
        {
            var sourceFiles = source.GetFiles();
            var styleFiles = style.GetFiles();

            foreach (var (sourceFile, styleFile) in sourceFiles.Product(styleFiles))
            {
                await CreateJob(images, queue, sourceFile, styleFile);
            }
        }

        private static async Task CreateJob(CloudBlobContainer images, CloudQueue queue, string source, string style)
        {
            var sourceName = source.UploadToBlob(images).Result;
            var styleName = style.UploadToBlob(images).Result;

            var job = new Job { SourceName = sourceName, StyleName = styleName, Iterations = 500, Size = 750, StyleWeight = 50, StyleScale = 0.2, UseOriginalColors = true };
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