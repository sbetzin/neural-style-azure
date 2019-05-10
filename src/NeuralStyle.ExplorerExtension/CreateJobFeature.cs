using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralStyle.Core;
using NeuralStyle.Core.Cloud;
using NeuralStyle.Core.Features;
using NeuralStyle.Core.Imaging;

namespace NeuralStyle.ExplorerExtension
{
    public static class CreateJobFeature
    {
        public static void CreateLargeImageJob(string targetImage)
        {
            var stylePath = @"C:\Data\images\style";
            var inPath = @"C:\Data\images\in";

            var (image, style) = targetImage.GetTags();

            if (image == string.Empty || style == string.Empty)
            {
                return;
            }

            var inImages = new[] { $@"{inPath}\{image}" };
            var styleImages = new string[] { $@"{stylePath}\{style}" };

            var (queue, container) = Factory.Construct();


            CreateJobs.CreateNew(container, queue, inImages, styleImages, 500, 2500, 0.01, 50.0);
        }
    }
}
