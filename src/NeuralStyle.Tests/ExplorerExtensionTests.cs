using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralStyle.ExplorerExtension.Features;
using Xunit;

namespace NeuralStyle.Tests
{

    public class ExplorerExtensionTests
    {
        [Fact]
        public static void TestLargeImage()
        {
            var target = @"C:\Data\images\print\lofoten_reine_abstract_1_1000px_cw_0.01_sw_50_iter_500_origcolor_0.jpg";

            CreateEnlargeJob.CreateLargeImageJob(target);
        }

    }
}
