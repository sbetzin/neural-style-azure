using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var stylePath = @"C:\Data\images\style";
            var inPath = @"C:\Data\images\in";
            var outPath = @"C:\Data\images\out";

            var allStyles = Directory.GetFiles(stylePath, "*.jpg").Select(Path.GetFileNameWithoutExtension).ToList();
            var allIn = Directory.GetFiles(inPath, "*.jpg").Select(Path.GetFileNameWithoutExtension).ToList();
            var allOut = Directory.GetFiles(outPath, "*.jpg", SearchOption.AllDirectories);

            var cominations = allStyles.SelectMany(first => allIn, (first, second) => (first, second, $"{second}_{first}_")).ToList();

            foreach (var file in allOut)
            {
                var found = false;
                foreach (var (style, image, comination) in cominations)
                {
                    if (!found && Path.GetFileNameWithoutExtension(file).StartsWith(comination))
                    {
                        found = true;
                        UpdateTags(file, image, style);
                    }
                }

                if (!found)
                {
                    Console.WriteLine($"not found for {file}");
                }

            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static void UpdateTags(string file, string image, string style)
        {
            var bytes = File.ReadAllBytes(file);
            var stream = new MemoryStream(bytes);
            var bitmap = Image.FromStream(stream);

            var title = CreatePropertyItem(40091, image);
            var subject = CreatePropertyItem(40095, style);

            bitmap.SetPropertyItem(title);
            bitmap.SetPropertyItem(subject);

            bitmap.Save(file);
            bitmap.Dispose();
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
