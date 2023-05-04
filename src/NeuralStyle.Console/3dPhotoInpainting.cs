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
            var settings = new Dictionary<string, object>
            {
                {"content_name", "lofoten_reine.jpg"},
                {"result_name", "lofoten_reine_zoom_out_move.mp4"},
                {"recreate_depth_mesh", false},
                {"interpolation_kind","cubic"},
                {"depth_mode",0},
                {"fps",30},
                {"num_frames", 240},
                {"longer_side_len" ,1024},
                {"crop_border" , new []{0.05, 0.05, 0.05, 0.05 }},
                {"coordinates" , new [] {
                    new []{0.0,0.0,0.0},
                    new []{0.0,0.0,-0.1},
                    new []{0.01, 0 ,-0.09},
                    new []{0, 0.01 ,-0.08},
                    new []{-0.01, 0,-0.07},
                    new []{0, -0.01,-0.06},
                    new []{0.01, 0,-0.05},
                    new []{0, 0.01,-0.04},
                    new []{-0.01, 0,-0.03},
                    new []{0.0,0.0,0.0},
                }},
            };

            var frameInterpolationQueue = Factory.ConstructQueue("jobs-3d-inpainting");
            frameInterpolationQueue.CreateJob(settings);
            
        }
    }
}
