using System.Collections.Generic;
using System.IO;
using NeuralStyle.Core;
using NeuralStyle.Core.Cloud;

namespace NeuralStyle.Console
{
    public class FrameInterpolation
    {
        public static void Start(string basePath)
        {
            var settings = new Dictionary<string, object>
            {
                {"target_path", ""},
                {"fps", 30},
                {"times_to_interpolate", 4},
                {"block_height",2},
                {"block_width",2},
                {"loop", true},
                {"out_name" ,"out.mp4"}
            };

            var videoName = "lofoten_reine_slide";
            var maskedOutName = "amashiro_01_enhanced_mask_add_nearest";
           
            //CreateOneInterpolation(settings, basePath, videoName, maskedOutName);
            CreateInterpolationsForOutPath(settings, basePath, videoName);

        }

        private static void CreateInterpolationsForOutPath(Dictionary<string, object> settings, string basePath, string videoName)
        {
            var outPath = Path.Combine(basePath, "video", videoName, "out");
            var folders = Directory.GetDirectories(outPath);

            foreach (var folder in folders)
            {
                var jpegFiles = Directory.GetFiles(folder, "*.jpg");
                var mp4Files = Directory.GetFiles(folder, "*.mp4");

                if (jpegFiles.Length <= 0 || mp4Files.Length != 0)
                {
                    continue;
                }

                var maskedOutName = Path.GetFileName(folder);
                CreateOneInterpolation(settings, basePath, videoName, maskedOutName);
            }
        }

        private static void CreateOneInterpolation(Dictionary<string, object> settings, string basePath, string videoName, string maskedOutName)
        {
            var videoPath = Path.Combine(basePath, "video", videoName, "out", maskedOutName);
            Logger.Log($"creating frame-interpolation job for {videoPath}");

            var frameInterpolationQueue = Factory.ConstructQueue("jobs-frame-interpolation");
            var unixPath = "/nft/" + videoPath.FindRelativeUnixPath(basePath);

            var timesToInterpolate = settings["times_to_interpolate"];
            var outName = $"{maskedOutName}_{timesToInterpolate}.mp4";

            settings["target_path"] = unixPath;
            settings["out_name"] = outName;

            frameInterpolationQueue.CreateJob(settings);
        }
    }
}