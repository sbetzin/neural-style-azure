﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using NeuralStyle.Core.Cloud;
using NeuralStyle.Core.Model;

namespace NeuralStyle.Core.Features
{
    public static class CreateJobs
    {
        public static void CreateMissing(CloudBlobContainer container, CloudQueue queue, string inPath, string stylePath, string outPath, JobSettings settings)
        {
            var allIn = Directory.GetFiles(inPath, "*.jpg");
            var allStyles = Directory.GetFiles(stylePath, "*.jpg");
            var missing = FindMissingCombinations.Run(inPath, stylePath, outPath);
            Logger.Log($"Found {missing.Count} missing combinations");

            container.UploadImages(allIn);
            container.UploadImages(allStyles);
            
            missing.AsParallel().WithDegreeOfParallelism(20).ForAll(pair => queue.CreateJob(pair.In, pair.Style, settings));
        }

        public static void CreateNew(CloudBlobContainer container, CloudQueue queue, string[] images, string[] styles, JobSettings settings)
        {
            container.UploadImages(images);
            container.UploadImages(styles);

            queue.CreateJobs(images, styles, settings);
        }
    }
}
