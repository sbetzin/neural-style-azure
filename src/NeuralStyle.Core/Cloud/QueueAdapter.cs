using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Azure.Storage;
using Azure.Storage.Queues;
using NeuralStyle.Core.Model;
using Newtonsoft.Json;

namespace NeuralStyle.Core.Cloud
{
    public static class QueueAdapter
    {
        public static void CreateJobs(this QueueClient queue, IEnumerable<string> sourceFiles, IEnumerable<string> styleFiles, JobSettings settings)
        {
            var jobs = styleFiles.Product(sourceFiles).ToList();

            Logger.Log($"Creating {jobs.Count} jobs");
            foreach (var (styleFile, sourceFile) in jobs)
            {
                queue.CreateNeuralStyleTransferJob(sourceFile, styleFile, settings);
            }
        }

        public static QueueClient GetAzureQueue(string connectionString, string queueName)
        {
            var queueClient = new QueueClient(connectionString, queueName);

            return queueClient;
        }

        public static void CreateNeuralStyleTransferJob(this QueueClient queue, string sourceFile, string styleFile, JobSettings settings)
        {

            var job = new NeuralStyleTransferJob
            {
                ContentName = Path.GetFileName(sourceFile),
                StyleName = Path.GetFileName(styleFile),
                Iterations = settings.Iterations,
                Size = settings.Size,
                StyleWeight = settings.StyleWeight,
                ContentWeight = settings.ContentWeight,
                TvWeight = settings.TvWeight,
                Model = settings.Model,
                Optimizer = settings.Optimizer,
                Init = settings.Init,
            };

            if (settings.TargetName != string.Empty)
            {
                job.TargetName = settings.TargetName;
            }
            else
            {
                job.TargetName = CreateTargetName(job);
            }

            var json = JsonConvert.SerializeObject(job);

            queue.SendMessage(json);

            Logger.Log($"   added job for image {sourceFile} with style {styleFile}");
        }

        public static void CreateJob(this QueueClient queue, Dictionary<string, object> job)
        {
            var json = JsonConvert.SerializeObject(job);

            queue.SendMessage(json);
            Logger.Log("Added job");
        }

        private static QueueClient EnsureThatExists(this QueueClient queue)
        {
            if (!queue.ExistsAsync().Result)
            {
                queue.CreateAsync().Wait();
            }

            return queue;
        }

        private static string CreateTargetName(NeuralStyleTransferJob neuralStyleTransferJob)
        {
            var prefix = Path.GetFileNameWithoutExtension(neuralStyleTransferJob.ContentName).BuildPrefix(Path.GetFileNameWithoutExtension(neuralStyleTransferJob.StyleName));

            FormattableString name = $"{prefix}{neuralStyleTransferJob.Size}px_cw_{neuralStyleTransferJob.ContentWeight:G}_sw_{neuralStyleTransferJob.StyleWeight:G}_tv_{neuralStyleTransferJob.TvWeight}_model_{neuralStyleTransferJob.Model}_opt_{neuralStyleTransferJob.Optimizer}_origcolor_#origcolor#.jpg";

            return name.ToString(new CultureInfo("en-US"));
        }
    }
}