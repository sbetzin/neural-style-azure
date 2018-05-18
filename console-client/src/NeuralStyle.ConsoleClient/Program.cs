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
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=neuralstylefiles;AccountKey=mMxv0dYg1xyEqE5VsrZejnH1PKQL5NsvG2gwYAfyHCrN1LDGYTXztCLoyfXa7ObB9BpPvXhGBtBg2A6owaV3gQ==;EndpointSuffix=core.windows.net");

            var queueClient = storageAccount.CreateCloudQueueClient();

            var blobClient = storageAccount.CreateCloudBlobClient();

            var queue = queueClient.GetQueueReference("jobs");
            queue.EncodeMessage = false;

            var images = blobClient.GetContainerReference("images");

            //CreateStyleBatch(images, queue, @"C:\Data\images\in\Ana.jpg").Wait();

            CreateSimple(images, queue).Wait();
        }

        private static async Task CreateStyleBatch(CloudBlobContainer images, CloudQueue queue, string source)
        {
            var styles = Directory.GetFiles(@"C:\Data\images\style", "*.jpg");
            foreach (var style in styles)
            {
                await CreateJob(images, queue, source, style);
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

            var job = new Job { SourceName = sourceName, StyleName = styleName, Iterations = 500, Size = 1200 };
            job.TargetName = $"{job.SourceName}_{job.StyleName}_{job.Size}px_sw_{job.StyleWeight}_ss_{job.StyleScale}_iter_{job.Iterations}_origcolor_{job.UseOriginalColors}";

            var json = JsonConvert.SerializeObject(job);
            var message = new CloudQueueMessage(json);

            await queue.AddMessageAsync(message);

            Console.WriteLine($"Created job for {source} with style {style}");
        }
    }
}