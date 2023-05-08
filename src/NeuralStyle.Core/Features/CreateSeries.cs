using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using NeuralStyle.Core.Model;
using System;
using System.IO;
using System.Linq;

namespace NeuralStyle.Core.Features
{
    public static class CreateSeries
    {
        public static void Fixed(QueueClient queue, string[] singlePic, string[] singleStyle, string basePath, string outPath)
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

                CreateJobs.CreateNew(queue, singlePic, singleStyle, settings, basePath, outPath);
            }
        }

        public static void FromThumbs(QueueClient queue, string thumbsPath, string[] styles, string basePath, string outPath)
        {
            var images = Directory.GetFiles(thumbsPath, "*.jpg", SearchOption.TopDirectoryOnly);

            var min = 3e8;
            var max = 3e9;
            var count = images.Count();
            var stepSize = (max - min) / count;

            foreach (var style in styles)
            {
                
                for (var step = 0; step < count; step++)
                {
                    var styleName = Path.GetFileNameWithoutExtension(style);

                    var settings = new JobSettings
                    {
                        Size = 1000,
                        StyleWeight = 1e5,
                        ContentWeight = 3e4,
                        TvWeight = 1,
                        Model = "vgg19",
                        Optimizer = "lbfgs",
                        Iterations = 500,
                        Init = "content",
                        TargetName = $"{styleName}_c#origcolor#_{step:D4}.jpg",
                    };

                    CreateJobs.CreateNew(queue, images[step], style, settings, basePath, outPath);
                }
            }
        }
    }
}
