using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using NeuralStyle.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralStyle.Core.Features
{
    public static class CreateSeries
    {
        public static void Fixed(QueueClient queue, BlobContainerClient container, string[] singlePic, string[] singleStyle)
        {
            var min = 3e8;
            var max = 3e9;
            var count = 100;
            var stepSize = (max - min) / count;

            for (var step = 0; step <= count; step++)
            {
                var settings = new JobSettings
                {
                    Size = 1000,
                    StyleWeight = 1e5,
                    ContentWeight = Math.Round(max - (step * stepSize), 0),
                    TvWeight = 1,
                    Model = "vgg19",
                    Optimizer = "lbfgs",
                    Iterations = 500,
                    Init = "content",
                };

                CreateJobs.CreateNew(container, queue, singlePic, singleStyle, settings);
            }
        }

        public static void FromThumbs(QueueClient queue, BlobContainerClient container, string thumbsPath, string[] singleStyle)
        {
            var images = Directory.GetFiles(thumbsPath, "*.jpg", SearchOption.TopDirectoryOnly);

            var min = 3e8;
            var max = 3e9;
            var count = images.Count();
            var stepSize = (max - min) / count;

            for (var step = 0; step < count; step++)
            {
                var settings = new JobSettings
                {
                    Size = 1000,
                    StyleWeight = 1e5,
                    ContentWeight = Math.Round(max - (step * stepSize), 0),
                    TvWeight = 1,
                    Model = "vgg19",
                    Optimizer = "lbfgs",
                    Iterations = 500,
                    Init = "content",
                    TargetName = step.ToString(),
                };

                CreateJobs.CreateNew(container, queue, images[step], singleStyle, settings);
            }
        }
    }
}
