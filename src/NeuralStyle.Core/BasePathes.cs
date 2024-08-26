using System.IO;

namespace NeuralStyle.Core
{
    public static class BasePathes
    {
        public static string BasePath()
        {
            return @"C:\Data\OneDrive\_nft";
        }

        public static string StylePath()
        {
            return $@"{BasePath()}\style";
        }

        public static string InPath()
        {
            return $@"{BasePath()}\in";
        }

        public static string OutPath()
        {
            return $@"{BasePath()}\out";
        }

        public static string VideoPath()
        {
            return $@"{BasePath()}\video";
        }

        public static string SharePath()
        {
            return $@"{BasePath()}\share";
        }

        public static string[] GetAllStyles()
        {
            return Directory.GetFiles(StylePath(), "*.jpg");
        }

        public static string[] GetInImages(string searchPatthern)
        {
            return Directory.GetFiles(InPath(), searchPatthern);
        }
       
    }
}