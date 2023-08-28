using System.IO;
using System.Text;
using NeuralStyle.Core.Imaging;

namespace NeuralStyle.Core.Features.WebpageCreation
{
    public static class CreateMiningMetaData
    {
        public static void CreateTextFile(string mintPath, string name)
        {
            var images = mintPath.Get_All_Images(SearchOption.TopDirectoryOnly);
            var result = new StringBuilder();
            
            foreach (var image in images)
            {
                var (inImage, styleImage) = image.GetTags();

                styleImage = styleImage.Replace("_", " ");
                result.AppendLine($"{name} - {styleImage}");
                result.AppendLine($"{name} was created by applying the style {styleImage} using AI with a neural net style transfer model.");
                result.AppendLine($"https://pages.artme.ai/{Path.GetFileNameWithoutExtension(image)}.html");
                result.AppendLine($"{styleImage}");
                result.AppendLine();

            }

            var metadataFile = Path.Combine(mintPath, "metadata.txt");
            File.WriteAllText(metadataFile, result.ToString());
        }
    }
}
