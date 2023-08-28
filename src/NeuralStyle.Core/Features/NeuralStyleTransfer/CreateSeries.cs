using System;
using Azure.Storage.Queues;
using NeuralStyle.Core.Model;

namespace NeuralStyle.Core.Features.NeuralStyleTransfer
{
    public static class CreateSeries
    {
        public static void Fixed(string[] singlePic, string[] singleStyle, string basePath, string outPath)
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

                CreateJobs.CreateNew(singlePic, singleStyle, settings, basePath, outPath);
            }
        }
    }
}
