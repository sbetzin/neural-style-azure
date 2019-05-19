using System;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using NeuralStyle.Core.Cloud;

namespace NeuralStyle.Core.Features
{
    public static class CreateJobs
    {
        public static void CreateMissing(CloudBlobContainer container, CloudQueue queue, string inPath, string stylePath, string outPath, int iterations, int size, double contentWeight, double styleWeight)
        {
            var allIn = Directory.GetFiles(inPath, "*.jpg");
            var allStyles = Directory.GetFiles(stylePath, "*.jpg");
            var missing = FindMissingCombinations.Run(inPath, stylePath, outPath);
            Logger.Log($"Found {missing.Count} missing combinations");

            container.UploadImages(allIn);
            container.UploadImages(allStyles);

            missing.ForEach(pair => queue.CreateJob(pair.In, pair.Style, iterations, size, contentWeight, styleWeight));
        }

        public static void CreateNew(CloudBlobContainer container, CloudQueue queue, string[] images, string[] styles, int iterations, int size, double contentWeight, double styleWeight)
        {
            container.UploadImages(images);
            container.UploadImages(styles);

            queue.CreateJobs(images, styles, iterations, size, styleWeight, contentWeight);
        }
    }
}
