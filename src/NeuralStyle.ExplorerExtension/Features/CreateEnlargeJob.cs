using System.IO;
using System.Linq;
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
                return;
            }

            var inImages = new[] { $@"{inPath}\{inName}.jpg" };
            var styleImages = new[] { $@"{stylePath}\{styleName}.jpg" };

            if (!inImages.All(File.Exists) || !styleImages.All(File.Exists))
            {
                return;
            }

            var (queue, container) = Factory.Construct();
            
            CreateJobs.CreateNew(container, queue, inImages, styleImages, 500, 2500, 0.01, 50.0);
        }
    }
}