using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeuralStyle.Core;
using NeuralStyle.Core.Cloud;
using NeuralStyle.Core.Imaging;

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
            var sebastian = Directory.GetFiles(inPath, "sebastian_*.jpg");
            var berge = Directory.GetFiles(inPath, "berge*.jpg");
            var vdma = Directory.GetFiles(inPath, "vdma_gruppe.jpg");


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

            var singlePic = new[] {$@"{inPath}\eric_jump.jpg"};
            var singleStyle = new[]
            {
                $@"{stylePath}\abstract_woman.jpg",
                $@"{stylePath}\andrew_atroshenko_fantastic.jpg",
                $@"{stylePath}\elena_prokopenko_tanz7.jpg",
                $@"{stylePath}\moderne_violist_girl.jpg",
                $@"{stylePath}\picasso_self_portrait_with_palette.jpg"
            };

            ImageAdapter.Ensure_Correct_Filenames(images);

            //SortImages.SortNewImages(@"C:\Data\OneDrive\neuralimages", outPath);

            //CreateJobs.CreateMissing(container, queue, inPath, stylePath, outPath, 500, 900, 0.01, 50.0);
            //CreateJobs.CreateNew(container, queue, singlePic, singleStyle, 500, 3000, 0.01, 50.0);
            //UpdateNames.FixNames(inPath, stylePath, outPath);
            //UpdateNames.FixNamesByTag(outPath);

            Logger.Log("");
            Logger.Log("Done");


            //Console.ReadLine();
        }
    }
}