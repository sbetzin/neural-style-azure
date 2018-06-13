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

            Update_Tags_in_Existing_Images(inPath, stylePath, outPath);
            Update_Tags_in_Existing_Images(inPath, stylePath, outPath);
        }

      
    }
}
