using System.IO;
using Azure.Storage.Blobs;
using NeuralStyle.Core.Cloud;
using NeuralStyle.Core.Imaging;
using NeuralStyle.Core.Templating;

namespace NeuralStyle.Core.Features
{
    public static class CreateWebpages
    {
        public static void CreateAll(BlobContainerClient webContainer, string outScaledPath, string webPath, string templateFile)
        {
            var outImages = outScaledPath.Get_All_Images(SearchOption.AllDirectories);

            foreach (var outImage in outImages)
            {
                Create(webContainer, webPath, templateFile, outImage);
            }
        }

        private static void Create(BlobContainerClient webContainer, string webPath, string templateFile, string outImage)
        {
            var webPage = WebpageCreator.FromTemplate(templateFile, outImage);
            var webFile = Path.Combine(webPath, $"{Path.GetFileNameWithoutExtension(outImage)}.html");

            if (File.Exists(webFile))
            {
                var existingPage = File.ReadAllText(webFile);
                if (webPage == existingPage)
                {
                    return;
                }
            }

            Logger.Log($"Creating web page for {outImage}");

            File.WriteAllText(webFile, webPage);
            webFile.UploadTextToBlob(webContainer);

        }
    }
}