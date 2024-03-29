﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralStyle.Core.Cloud;

namespace NeuralStyle.Console
{
    public static class MaskTransfer
    {
        public static void Start(string basePath)
        {
            var settings = new Dictionary<string, object>
            {
                {"video_name", "close_up_01_move01"},
                {"force_generation", false},
            };

            var frameInterpolationQueue = Factory.ConstructQueue("jobs-mask-transfer");
            frameInterpolationQueue.CreateJob(settings);
        }
    }
}
