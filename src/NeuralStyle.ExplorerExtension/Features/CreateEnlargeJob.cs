using System.IO;
using NeuralStyle.Core;
using NeuralStyle.Core.Cloud;
using NeuralStyle.Core.Features;
using NeuralStyle.Core.Imaging;
using NeuralStyle.Core.Model;

namespace NeuralStyle.ExplorerExtension.Features
{
    public static class CreateEnlargeJob
    {
        public static void CreateLargeImageJob(string targetImage)
        {
            var stylePath = @"C:\Data\images\style";
            var inPath = @"C:\Data\images\in";

            var (inName, styleName) = targetImage.GetTags();

            if (inName == string.Empty || styleName == string.Empty)
            {
                Logger.Log($"target image {targetImage} has no valid tags");
                return;
            }

            var inImage = $@"{inPath}\{inName}.jpg";
            var styleImage = $@"{stylePath}\{styleName}.jpg";

            if (!File.Exists(inImage))
            {
                Logger.Log($"{inImage} not found");
                return;
            }

            if (!File.Exists(styleImage))
            {
                Logger.Log($"{styleImage} not found");
                return;
            }

            var (queue, container) = Factory.Construct();

            var settings = new JobSettings()
            {
                Iterations = 500,
                Size = 2500,
                StyleWeight = 0.01,
                ContentWeight = 50.0,
                TvWeight = 0.001,
                TemporalWeight = 200,
                ContentLossFunction = 1
            };

            CreateJobs.CreateNew(container, queue, new[] {inImage}, new[] {styleImage}, settings);
        }
    }
}