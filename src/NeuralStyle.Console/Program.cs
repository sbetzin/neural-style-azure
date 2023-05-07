using System;
using System.Diagnostics;
using System.Linq;
using NeuralStyle.Core;
using NeuralStyle.Core.Folders;

namespace NeuralStyle.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Logger.NewLog += System.Console.WriteLine;

            //NeuralStyleTransfer.Start();
            FrameInterpolation.Start();
            //_3dPhotoInpainting.Start();
            //MaskTransfer.Start();
            //FolderCheck.IsInFolder(@"C:\Users\gensb\OneDrive\_nft\video2\norwegen-19_move", @"C:\Users\gensb\OneDrive\_nft\video");


            Logger.Log("");
            Logger.Log("Done");

            System.Console.ReadKey();
        }
    }
}