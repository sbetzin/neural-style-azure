using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ExifLib;

namespace NeuralStyle.ConsoleClient
{
    public static class ExifAdapter
    {
        public static (string inImage, string styleImage) GetTags(this string file)
        {
            var bytes = File.ReadAllBytes(file);
            var stream = new MemoryStream(bytes);
            using (var bitmap = Image.FromStream(stream))
            {
                string inImage = null;
                string styleImage = null;
                if (bitmap.PropertyItems.Any(item => item.Id == 40091))
                {
                    inImage = Encoding.Unicode.GetString(bitmap.GetPropertyItem(40091).Value);
                }

                if (bitmap.PropertyItems.Any(item => item.Id == 40095))
                {
                    styleImage = Encoding.Unicode.GetString(bitmap.GetPropertyItem(40095).Value);
                }

                return (inImage, styleImage);
            }
        }

        public static (string inImage, string styleImage) GetTags2(this string file)
        {
            try
            {
                using (var reader = new ExifReader(file))
                {
                    reader.GetTagValue(ExifTags.XPSubject, out string styleImage);
                    reader.GetTagValue(ExifTags.XPTitle, out string inImage);

                    styleImage = styleImage.FixUnicodeBug();
                    inImage = inImage.FixUnicodeBug();

                    return (inImage, styleImage);
                }
            }
            catch (ExifLibException e)
            {
                return (null, null);
            }
        }

        public static string FixUnicodeBug(this string value)
        {
            var bytes = Encoding.Unicode.GetBytes(value);
            if (bytes[0] != 255 || bytes[1] != 254)
            {
                return value;
            }

            var correctedBytes = bytes.Skip(2).ToArray();

            return Encoding.Unicode.GetString(correctedBytes);

        }

        public static void UpdateTags(this string file, string image, string style)
        {
            var bytes = File.ReadAllBytes(file);
            var stream = new MemoryStream(bytes);
            using (var bitmap = Image.FromStream(stream))
            {
                var title = CreatePropertyItem(40091, image);
                var subject = CreatePropertyItem(40095, style);

                bitmap.SetPropertyItem(title);
                bitmap.SetPropertyItem(subject);

                bitmap.Save(file);
            }
        }

        private static PropertyItem CreatePropertyItem(int id, string value)
        {
            var newItem = (PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem));
            newItem.Id = id;
            newItem.Value = Encoding.Unicode.GetBytes(value);
            newItem.Len = newItem.Value.Length;
            newItem.Type = 1;

            return newItem;
        }
    }
}