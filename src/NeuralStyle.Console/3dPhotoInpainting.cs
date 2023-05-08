using NeuralStyle.Core.Cloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralStyle.Console
{
    public static class _3dPhotoInpainting
    {
        public static void Start()
        {
            var contentName = "sebastian_fenster.jpg";
            var fps = 30;
            var numFrames = 180;
            var longerSizeLen = 1024;
            var depthMode = 0;


            var basicSettings = CreateBasicSettings(contentName, depthMode, fps, numFrames, longerSizeLen);
            var settings = AddZoomSettings(basicSettings);

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
                { "content_name",  contentName},
                { "result_name", $"{name}_zoom_out_move.mp4" },
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
        private static Dictionary<string, object> AddZoomSettings(Dictionary<string, object> settings)
        {

            var addedSettings = new Dictionary<string, object>
            {
                {
                    "coordinates", new[]
                    {
                        new[] { 0.0, 0.0, 0.0 },
                        new[] { 0.0, 0.0, -0.1 },
                        new[] { 0.01, 0, -0.09 },
                        new[] { 0, 0.01, -0.08 },
                        new[] { -0.01, 0, -0.07 },
                        new[] { 0, -0.01, -0.06 },
                        new[] { 0.01, 0, -0.05 },
                        new[] { 0, 0.01, -0.04 },
                        new[] { -0.01, 0, -0.03 },
                        new[] { 0.0, 0.0, 0.0 },
                    }
                },
            };
            var united= settings.Union(addedSettings);

            return united.ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}
