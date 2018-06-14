using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using NeuralStyle.ConsoleClient.Model;
using Newtonsoft.Json;

namespace NeuralStyle.ConsoleClient
{
    public static class QueueAdapter
    {
        public static CloudQueue GetAzureQueue(string connectionString, string queueName)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();

            var queue = queueClient.GetQueueReference(queueName).EnsureThatExists();
            queue.EncodeMessage = false;

            return queue;
        }

        public static async Task CreateJob(this CloudQueue queue, string sourceFile, string styleFile, int iterations, int size, double contentWeight, double styleWeight)
        {
            var job = new Job {SourceName = Path.GetFileName(sourceFile), StyleName = Path.GetFileName(styleFile), Iterations = iterations, Size = size, StyleWeight = styleWeight, ContentWeight = contentWeight};
            job.TargetName = CreateTargetName(job);

            var json = JsonConvert.SerializeObject(job);
            var message = new CloudQueueMessage(json);

            await queue.AddMessageAsync(message);

            Console.WriteLine($"   added job for image {sourceFile} with style {styleFile}");
        }

        private static CloudQueue EnsureThatExists(this CloudQueue queue)
        {
            if (!queue.ExistsAsync().Result)
            {
                queue.CreateAsync().Wait();
            }

            return queue;
        }

        private static string CreateTargetName(Job job)
        {
            FormattableString name = $"{Path.GetFileNameWithoutExtension(job.SourceName)}_{Path.GetFileNameWithoutExtension(job.StyleName)}_{job.Size}px_cw_{job.ContentWeight:G}_sw_{job.StyleWeight:G}_iter_{job.Iterations}_origcolor_#origcolor#.jpg";

            return name.ToString(new CultureInfo("en-US"));
        }
    }
}