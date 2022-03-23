using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using NeuralStyle.Core.Model;
using Newtonsoft.Json;

namespace NeuralStyle.Core.Cloud
{
    public static class QueueAdapter
    {
        public  static void CreateJobs(this CloudQueue queue, IEnumerable<string> sourceFiles, IEnumerable<string> styleFiles, JobSettings settings)
        {
            var jobs = sourceFiles.Product(styleFiles).ToList();

            Logger.Log($"Creating {jobs.Count} jobs");
            foreach (var (sourceFile, styleFile) in jobs)
            {
                queue.CreateJob(sourceFile, styleFile, settings);
            }
        }

        public static CloudQueue GetAzureQueue(string connectionString, string queueName)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();

            var queue = queueClient.GetQueueReference(queueName).EnsureThatExists();
            queue.EncodeMessage = false;

            return queue;
        }

        public static void CreateJob(this CloudQueue queue, string sourceFile, string styleFile, JobSettings settings)
        {
            var job = new Job {SourceName = Path.GetFileName(sourceFile), 
                StyleName = Path.GetFileName(styleFile), 
                Iterations = settings.Iterations, 
                Size = settings.Size, 
                StyleWeight = settings.StyleWeight, 
                ContentWeight = settings.ContentWeight,
                TvWeight =  settings.TvWeight,
                TemporalWeight = settings.TemporalWeight,
                ContentLossFunction = settings.ContentLossFunction
            };
                
            job.TargetName = CreateTargetName(job);

            var json = JsonConvert.SerializeObject(job);
            var message = new CloudQueueMessage(json);

            queue.AddMessage(message);

            Logger.Log($"   added job for image {sourceFile} with style {styleFile}");
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
            var prefix = Path.GetFileNameWithoutExtension(job.SourceName).BuildPrefix(Path.GetFileNameWithoutExtension(job.StyleName));

            FormattableString name = $"{prefix}{job.Size}px_cw_{job.ContentWeight:G}_sw_{job.StyleWeight:G}_tvw_{job.TvWeight:G}_tmpw_{job.TemporalWeight:G}_clf_{job.ContentLossFunction:G}_iter_{job.Iterations}_origcolor_#origcolor#.jpg";

            return name.ToString(new CultureInfo("en-US"));
        }
    }
}