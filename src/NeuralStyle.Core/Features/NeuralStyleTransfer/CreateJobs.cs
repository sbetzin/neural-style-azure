using Azure.Storage.Queues;
using NeuralStyle.Core.Cloud;
using NeuralStyle.Core.Model;

namespace NeuralStyle.Core.Features.NeuralStyleTransfer
{
    public static class CreateJobs
    {
        private static QueueClient CreateQueue()
        {
            return Factory.ConstructQueue("jobs-stylize");
        }

        public static void CreateMissing(string inPath, string stylePath, string outPath, JobSettings settings, string basePath)
        {
            var queue = CreateQueue();
            var missing = FindMissingCombinations.Run(inPath, stylePath, outPath);
            Logger.Log($"Found {missing.Count} missing combinations");


            missing.ForEach(pair => queue.CreateNeuralStyleTransferJob(pair.In, pair.Style, settings, basePath, outPath));
        }

        public static void CreateNew(string[] images, string[] styles, JobSettings settings, string basePath, string outPath)
        {
            var queue = CreateQueue();
            queue.CreateJobs(images, styles, settings, basePath, outPath);
        }

        public static void CreateNew(string image, string[] styles, JobSettings settings, string basePath, string outPath)
        {
            var queue = CreateQueue();
            CreateNew(new[] { image }, styles, settings, basePath, outPath);
        }

        public static void CreateNew(string image, string style, JobSettings settings, string basePath, string outPath)
        {
            CreateNew(new[] { image }, new[] { style }, settings, basePath, outPath);
        }
    }
}
