using System;
using System.Collections.Generic;
using NeuralStyle.Console.MovePathes;
using NeuralStyle.Core.Cloud;

namespace NeuralStyle.Console
{
    public static class _3dPhotoInpainting
    {
        public static void Start(string basePath, string contentName)
        {
            var fps = 30;
            var numFrames = 180;
            var longerSizeLen = 1024;
            var depthMode = 2;


            var settings = CreateBasicSettings(contentName, depthMode, fps, numFrames, longerSizeLen);
            settings.AddZoomMovement();

            CreateJob(settings);
        }

        private static void CreateJob(Dictionary<string, object> settings)
        {
            var frameInterpolationQueue = Factory.ConstructQueue("jobs-3d-inpainting");
            frameInterpolationQueue.CreateJob(settings);
        }

        private static Dictionary<string, object> CreateBasicSettings(string contentName, int depthMode = 0, int fps = 30, int numFrames = 180, int longerSizeLen = 1024)
        {
            var name = contentName.Split(new[] { "." }, StringSplitOptions.None)[0];

            var settings = new Dictionary<string, object>
            {
                { "content_name", contentName },
                { "result_name", $"{name}_d{depthMode}_#movename#.mp4" },
                { "recreate_depth_mesh", false },
                { "interpolation_kind", "cubic" },
                { "depth_mode", depthMode },
                { "fps", fps },
                { "num_frames", numFrames },
                { "longer_side_len", longerSizeLen },
                { "crop_border", new[] { 0.05, 0.05, 0.05, 0.05 } }
            };

            return settings;
        }
    }
}