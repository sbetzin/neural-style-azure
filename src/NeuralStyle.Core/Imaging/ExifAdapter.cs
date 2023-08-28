using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Text;
using ExifLib;

namespace NeuralStyle.Core.Imaging
{
    public static class ExifAdapter
    {
        public static (string In, string Style) GetTags(this string file)
        {
            try
            {
                using var reader = new ExifReader(file);
                reader.GetTagValue(ExifTags.XPSubject, out string styleImage);
                reader.GetTagValue(ExifTags.XPTitle, out string inImage);

                styleImage = styleImage.FixUnicodeBug();
                inImage = inImage.FixUnicodeBug();

                return (inImage, styleImage);
            }
            catch (ExifLibException)
            {
                return (string.Empty, string.Empty);
            }
        }

        private static string FixUnicodeBug(this string value)
        {
            var bytes = Encoding.Unicode.GetBytes(value);
            if (bytes[0] != 255 || bytes[1] != 254)
            {
                return value;
            }

            var correctedBytes = bytes.Skip(2).ToArray();

            return Encoding.Unicode.GetString(correctedBytes);
        }

        private static byte[] FixUnicodeBug(this byte[] value)
        {
            if (value[0] != 255 || value[1] != 254)
            {
                return value;
            }

            return value.Skip(2).ToArray();
        }

        public static void UpdateTags(this string file, string image, string style)
        {
            var bytes = File.ReadAllBytes(file);
            var stream = new MemoryStream(bytes);
            using var bitmap = Bitmap.FromStream(stream);
            bitmap.UpdateTag(40091, image);
            bitmap.UpdateTag(40095, style);

            bitmap.Save(file);
        }

        public static void UpdateTag(this Image image, int tagId, string value)
        {
            var property = CreatePropertyItem(tagId, value);
            image.SetPropertyItem(property);
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

        public static void FixTags(this string file)
        {
            var bytes = File.ReadAllBytes(file);
            var stream = new MemoryStream(bytes);

            using var bitmap = Image.FromStream(stream);
            var tags = bitmap.PropertyItems;
            foreach (var tag in tags)
            {
                tag.Value = tag.Value.FixUnicodeBug();

                bitmap.SetPropertyItem(tag);
            }

            bitmap.Save(file);
        }
    }
}