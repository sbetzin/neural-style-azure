using System.Collections.Generic;
using NeuralStyle.Core;
using NeuralStyle.Core.Cloud;

namespace NeuralStyle.Console
{
    public class FrameInterpolation
    {
        public static void Start()
        {
            var frameInterpolationQueue = Factory.ConstructQueue("jobs-frame-interpolation");
            var basePath = @"C:\Users\gensb\OneDrive\_nft\video\";
            var targetPath = @"C:\Users\gensb\OneDrive\_nft\video\street_woman_01_move_01\out\amashiro_01_enhanced_mask_single";

            var unixPath = $@"/nft/video/{targetPath.FindRelativeUnixPath(basePath)}";

            var settings = new Dictionary<string, object>
            {
                {"target_path", unixPath},
                {"fps", 30},
                {"times_to_interpolate", 4},
                {"block_height",1},
                {"block_width",1},
                {"loop", true}
            };

            frameInterpolationQueue.CreateJob(settings);
        }
    }
}