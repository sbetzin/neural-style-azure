using System.Collections.Generic;
using System.IO;
using NeuralStyle.Core;
using NeuralStyle.Core.Cloud;

namespace NeuralStyle.Console
{
    public class FrameInterpolation
    {
        public static void Start()
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

            CreateOneInterpolation(settings, @"C:\Users\gensb\OneDrive\_nft\video\lofoten_reine_slide\out\amashiro_01_enhanced_mask_add_nearest");

            //CreateInterpolationsForOutPath(settings, @"C:\Users\gensb\OneDrive\_nft\video\norwegen-19_move\out");

        }

        private static void CreateInterpolationsForOutPath(Dictionary<string, object> settings, string outPath)
        {
            var folders = Directory.GetDirectories(outPath);

            foreach (var folder in folders)
            {
                var jpegFiles = Directory.GetFiles(folder, "*.jpg");
                var mp4Files = Directory.GetFiles(folder, "*.mp4");

                if (jpegFiles.Length > 0 && mp4Files.Length == 0)
                {
                    CreateOneInterpolation(settings, folder);
                }
            }
        }

        private static void CreateOneInterpolation(Dictionary<string, object> settings, string videoPath)
        {
            Logger.Log($"creating frame-interpolation job for {videoPath}");

            var frameInterpolationQueue = Factory.ConstructQueue("jobs-frame-interpolation");
            var basePath = @"C:\Users\gensb\OneDrive\_nft";
            var unixPath = "/nft/" + videoPath.FindRelativeUnixPath(basePath);

            var folderName = new DirectoryInfo(videoPath).Name;
            var timesToInterpolate = settings["times_to_interpolate"];
            var outName = $"{folderName}_{timesToInterpolate}.mp4";

            settings["target_path"] = unixPath;
            settings["out_name"] = outName;

            frameInterpolationQueue.CreateJob(settings);
        }
    }
}