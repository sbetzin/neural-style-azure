using NeuralStyle.Core;

namespace NeuralStyle.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Logger.NewLog += System.Console.WriteLine;

            NeuralStyleTransfer.Start();
            //FrameInterpolation.Start();
            //_3dPhotoInpainting.Start();

            Logger.Log("");
            Logger.Log("Done");

            System.Console.ReadKey();
        }
    }
}