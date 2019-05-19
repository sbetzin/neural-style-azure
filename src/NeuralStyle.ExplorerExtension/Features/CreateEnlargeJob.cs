using System.IO;
using NeuralStyle.Core;
using NeuralStyle.Core.Cloud;
using NeuralStyle.Core.Features;
using NeuralStyle.Core.Imaging;

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

            CreateJobs.CreateNew(container, queue, new[] {inImage}, new[] {styleImage}, 500, 2500, 0.01, 50.0);
        }
    }
}