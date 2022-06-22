using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Parser;
using NeuralStyle.Core.Imaging;
using SmartFormat;
using SmartFormat.Core.Settings;

namespace NeuralStyle.Core.Templating
{
    public static class WebpageCreator
    {
        public static string FromTemplate(string templateFile, string outImage)
        {
            var html = File.ReadAllText(templateFile);

            var (inImage, styleImage) = outImage.GetTags();

            var content = new
            {
                OutImage = Path.GetFileNameWithoutExtension(outImage),
                InImage = inImage,
                StyleImage = styleImage,
                Title = $"{inImage} as {styleImage}"
            };

            var parser = new HtmlParser();
            var htmlDocument = parser.ParseDocument(html);
            if (htmlDocument.Body != null)
            {
                htmlDocument.Body.InnerHtml = Smart.Format(htmlDocument.Body.InnerHtml, content);
            }

            if (htmlDocument.Title != null)
            {
                htmlDocument.Title = Smart.Format(htmlDocument.Title, content);
            }

            return htmlDocument.ToHtml();
        }
    }
}
