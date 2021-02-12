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

            var (queue, container) = Factory.Construct();

            var images = @"C:\Data\images";
            var stylePath = @"C:\Data\images\style";
            var inPath = @"C:\Data\images\in";
            var outPath = @"C:\Data\images\out";

            var kandinskyStyles = Directory.GetFiles(stylePath, "kandinsky_*.jpg");
            var modernArtStyle = Directory.GetFiles(stylePath, "modern_art_*.jpg");
            var danceStyles = Directory.GetFiles(stylePath, "elena_prokopenko_*.jpg");
            var goghStyles = Directory.GetFiles(stylePath, "gogh_*.jpg");
            var afremovStyles = Directory.GetFiles(stylePath, "Leonid_afremov*.jpg");
            var comicStyles = Directory.GetFiles(stylePath, "comic_*.jpg");
            var picassoStyles = Directory.GetFiles(stylePath, "picasso_*.jpg");
            var corinth = Directory.GetFiles(stylePath, "lovis_corinth_*.jpg");
            var allStyles = Directory.GetFiles(stylePath, "*.jpg");
            var monet = Directory.GetFiles(stylePath, "*monet_jpg");
            var ivanov = Directory.GetFiles(stylePath, "eugene_ivanov_*.jpg");

            var allIn = Directory.GetFiles(inPath, "*.jpg");

            var bestStyles = kandinskyStyles.Union(modernArtStyle).Union(picassoStyles).Union(new List<string>
            {
                $@"{stylePath}\abstract.jpg",
                $@"{stylePath}\hume_disin_die_glaubwuerdigkeit.jpg",
                $@"{stylePath}\candy.jpg",
                $@"{stylePath}\dieu_deep_in_my.jpg",
                $@"{stylePath}\elena_prokopenko_tanz7.jpg",
                $@"{stylePath}\expressionismus.jpg",
                $@"{stylePath}\lovis_corinth_walchensee.jpg",
                $@"{stylePath}\matisse_woman_with_hat.jpg",
                $@"{stylePath}\uta_welcker_annies_verwandtschaft.jpg",
                $@"{stylePath}\yosi_losaij_you_and_me.jpg"
            }).ToArray();

            var singlePic = new[] {$@"{inPath}\family_jump_01.jpg"};
            var singleStyle = new[]{$@"{stylePath}\picasso_tete_dune_femme_lisant.jpg"};

            ImageAdapter.Ensure_Correct_Filenames(images);

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

            CreateJobs.CreateMissing(container, queue, inPath, stylePath, outPath, settings);
            //SortImages.CreateMissingHardlinkgs(outPath);

            //CreateJobs.CreateNew(container, queue, singlePic, allStyles, settings);

            //CreateJobs.CreateNew(container, queue, singlePic, singleStyle, settings);
            //CreateJobs.CreateNew(container, queue, singlePic, singleStyle, 500, 500, 0.01, 50.0);
            //CreateJobs.CreateNew(container, queue, singlePic, singleStyle, 500, 1500, 0.01, 50.0);
            //CreateJobs.CreateNew(container, queue, singlePic, singleStyle, 500, 1500, 0.01, 1000.0);
            //CreateJobs.CreateNew(container, queue, singlePic, singleStyle, 500, 1500, 0.01, 10000.0);
            //CreateJobs.CreateNew(container, queue, singlePic, singleStyle, 500, 1500, 0.01, 100000.0);

            Logger.Log("");
            Logger.Log("Done");


            //System.Console.ReadKey();
        }
    }
}