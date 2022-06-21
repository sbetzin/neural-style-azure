using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeuralStyle.Core;
using NeuralStyle.Core.Cloud;
using NeuralStyle.Core.Features;
using NeuralStyle.Core.Imaging;
using NeuralStyle.Core.Model;

namespace NeuralStyle.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Logger.NewLog += System.Console.WriteLine;

            var (queue, container) = Factory.Construct("jobs");

            var images = @"C:\Data\images";
            var stylePath = @"C:\Data\images\style";
            var inPath = @"C:\Data\images\in";
            var inDonePath = @"C:\Data\images\in\done";
            var outPath = @"C:\Data\images\out";
            var outScaledPath = @"C:\Data\images\out_scaled";

            var allStyles = Directory.GetFiles(stylePath, "*.jpg");
            var monet = Directory.GetFiles(stylePath, "*monet_jpg");

            var allIn = Directory.GetFiles(inPath, "*.jpg");
            var allInDone = Directory.GetFiles(inDonePath, "*.jpg");

            var singlePic = new[] { $@"{inPath}\blumen-01.jpg" };
            var singleStyle = new[] { $@"{stylePath}\lovis_corinth_morgensonne.jpg" };

            UpdateNames.Ensure_Correct_Filenames(images);

            //SortImages.SortNewImages(@"C:\Users\gensb\OneDrive\neuralimages", outPath);


            var settings = new JobSettings()
            {
                Iterations = 500,
                Size = 1200,
                StyleWeight = 500,
                ContentWeight = 0.01,
                TvWeight = 0.001,
                TemporalWeight = 200,
                ContentLossFunction = 1
            };


            //CreateJobs.CreateMissing(container, queue, inDonePath, stylePath, outPath, settings);
            //SortImages.CreateMissingHardlinkgs(outPath);

           CreateJobs.CreateNew(container, queue, allIn, allStyles, settings);
           
            //CreateJobs.CreateNew(container, queue, singlePic, singleStyle, settings);
            //CreateJobs.CreateNew(container, queue, singlePic, allStyles, settingsHighCw);
            //CreateJobs.CreateNew(container, queue, singlePic, singleStyle, new JobSettings(){Iterations = 500, ContentLossFunction =1, StyleWeight = 500, ContentWeight = 0.01, Size =1200, TemporalWeight = 200, TvWeight =0.001});
            //CreateJobs.CreateNew(container, queue, singlePic, singleStyle, new JobSettings(){Iterations = 500, ContentLossFunction =1, StyleWeight = 500, ContentWeight = 0.001, Size =1200, TemporalWeight = 200, TvWeight =0.001});
            //CreateJobs.CreateNew(container, queue, singlePic, singleStyle, new JobSettings(){Iterations = 500, ContentLossFunction =1, StyleWeight = 500, ContentWeight = 0.0001, Size =1200, TemporalWeight = 200, TvWeight =0.001});
            //CreateJobs.CreateNew(container, queue, singlePic, singleStyle, new JobSettings(){Iterations = 500, ContentLossFunction =1, StyleWeight = 500, ContentWeight = 0.00001, Size =1200, TemporalWeight = 200, TvWeight =0.001});
            //CreateJobs.CreateNew(container, queue, singlePic, singleStyle, new JobSettings(){Iterations = 500, ContentLossFunction =1, StyleWeight = 500, ContentWeight = 0.000001, Size =1200, TemporalWeight = 200, TvWeight =0.001});
            //CreateJobs.CreateNew(container, queue, singlePic, singleStyle, new JobSettings(){Iterations = 500, ContentLossFunction =1, StyleWeight = 5000, ContentWeight = 0.000001, Size =1200, TemporalWeight = 200, TvWeight =0.001});
            //CreateJobs.CreateNew(container, queue, singlePic, singleStyle, new JobSettings(){Iterations = 500, ContentLossFunction =1, StyleWeight = 50000, ContentWeight = 0.000001, Size =1200, TemporalWeight = 200, TvWeight =0.001});
            //CreateJobs.CreateNew(container, queue, singlePic, singleStyle, new JobSettings(){Iterations = 500, ContentLossFunction =1, StyleWeight = 500000, ContentWeight = 0.000001, Size =1200, TemporalWeight = 200, TvWeight =0.001});


            Logger.Log("");
            Logger.Log("Done");


            System.Console.ReadKey();
        }
    }
}