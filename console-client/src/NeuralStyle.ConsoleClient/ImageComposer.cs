using System;
using System.Drawing;

namespace NeuralStyle.ConsoleClient
{
    public static class ImageComposer
    {
        public static Bitmap CombineBitmap(Bitmap[] images)
        {
            Bitmap finalImage = null;

            try
            {
                var width = 0;
                var height = 0;

                foreach (var image in images)
                {
                    width += image.Width;
                    height = image.Height > height ? image.Height : height;
                }

                finalImage = new Bitmap(width, height);

                using (var g = Graphics.FromImage(finalImage))
                {
                    g.Clear(Color.Black);

                    var offset = 0;
                    foreach (var image in images)
                    {
                        g.DrawImage(image, new Rectangle(offset, 0, image.Width, image.Height));
                        offset += image.Width;
                    }
                }

                return finalImage;
            }
            catch (Exception)
            {
                return finalImage;
            }
        }
    }
}