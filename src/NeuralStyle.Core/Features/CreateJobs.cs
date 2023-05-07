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
        public static void CreateMissing(BlobContainerClient container, QueueClient queue, string inPath, string stylePath, string outPath, JobSettings settings)
        {
            var allIn = Directory.GetFiles(inPath, "*.jpg");
            var allStyles = Directory.GetFiles(stylePath, "*.jpg");
            var missing = FindMissingCombinations.Run(inPath, stylePath, outPath);
            Logger.Log($"Found {missing.Count} missing combinations");

            container.UploadImages(allIn);
            container.UploadImages(allStyles);

            missing.ForEach(pair => queue.CreateNeuralStyleTransferJob(pair.In, pair.Style, settings));
        }

        public static void CreateNew(BlobContainerClient container, QueueClient queue, string[] images, string[] styles, JobSettings settings)
        {
            //container.UploadImages(images);
            //container.UploadImages(styles);

            queue.CreateJobs(images, styles, settings);
        }

        public static void CreateNew(BlobContainerClient container, QueueClient queue, string image, string[] styles, JobSettings settings)
        {
            CreateNew(container, queue, new[] { image }, styles, settings);
        }

        public static void CreateNew(BlobContainerClient container, QueueClient queue, string image, string style, JobSettings settings)
        {
            CreateNew(container, queue, new[] { image }, new [] {style}, settings);
        }
    }
}
