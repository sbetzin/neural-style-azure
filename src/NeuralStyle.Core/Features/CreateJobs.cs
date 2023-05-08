using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using NeuralStyle.Core.Cloud;
using NeuralStyle.Core.Model;

namespace NeuralStyle.Core.Features
{
    public static class CreateJobs
    {
        public static void CreateMissing(QueueClient queue, string inPath, string stylePath, string outPath, JobSettings settings, string basePath)
        {
            var missing = FindMissingCombinations.Run(inPath, stylePath, outPath);
            Logger.Log($"Found {missing.Count} missing combinations");


            missing.ForEach(pair => queue.CreateNeuralStyleTransferJob(pair.In, pair.Style, settings, basePath, outPath));
        }

        public static void CreateNew(QueueClient queue, string[] images, string[] styles, JobSettings settings, string basePath, string outPath)
        {
            //container.UploadImages(images);
            //container.UploadImages(styles);

            queue.CreateJobs(images, styles, settings, basePath, outPath);
        }

        public static void CreateNew(QueueClient queue, string image, string[] styles, JobSettings settings, string basePath,string outPath)
        {
            CreateNew(queue, new[] { image }, styles, settings, basePath, outPath);
        }

        public static void CreateNew(QueueClient queue, string image, string style, JobSettings settings, string basePath, string outPath)
        {
            CreateNew(queue, new[] { image }, new[] { style }, settings,basePath, outPath);
        }
    }
}
