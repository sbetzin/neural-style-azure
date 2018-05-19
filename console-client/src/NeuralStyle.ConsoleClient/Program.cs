using System;
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

            CreateStyleBatch(images, queue, @"C:\Data\images\in\Ana.jpg", @"C:\Users\gensb\Desktop\new").Wait();

            //CreateSimple(images, queue).Wait();
        }

        private static async Task CreateStyleBatch(CloudBlobContainer images, CloudQueue queue, string source, string style)
        {


            var styleFiles = Directory.GetFiles(style, "*.jpg");
            foreach (var styleFile in styleFiles)
            {
                await CreateJob(images, queue, source, styleFile);
            }
        }

        private static async Task CreateSimple(CloudBlobContainer images, CloudQueue queue)
        {
            await CreateJob(images, queue, @"C:\Data\images\in\Berge.jpg", @"C:\Data\images\style\karl_otto_goetz_ohne_titel.jpg");
        }

        private static async Task CreateJob(CloudBlobContainer images, CloudQueue queue, string source, string style)
        {
            var sourceName = source.UploadToBlob(images).Result;
            var styleName = style.UploadToBlob(images).Result;

            var job = new Job { SourceName = sourceName, StyleName = styleName, Iterations = 500, Size = 1200, StyleWeight = 50, StyleScale = 1, UseOriginalColors = true };
            job.TargetName = $"{Path.GetFileNameWithoutExtension(job.SourceName)}_{Path.GetFileNameWithoutExtension(job.StyleName)}_{job.Size}px_sw_{job.StyleWeight}_ss_{job.StyleScale}_iter_{job.Iterations}_origcolor_{job.UseOriginalColors}.jpg";

            var json = JsonConvert.SerializeObject(job);
            var message = new CloudQueueMessage(json);

            await queue.AddMessageAsync(message);

            Console.WriteLine($"Created job for {source} with style {style}");
        }
    }
}