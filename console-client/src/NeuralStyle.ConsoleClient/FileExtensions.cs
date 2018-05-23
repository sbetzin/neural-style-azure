using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace NeuralStyle.ConsoleClient
{
    public static class FileExtensions
    {
        public static async Task<string> UploadToBlob(this string file, CloudBlobContainer container)
        {
            var name = Path.GetFileName(file);
            var blob = container.GetBlockBlobReference(name);

            await blob.UploadFromFileAsync(file);
        
            return name;
        }

        public static IEnumerable<string> GetFiles(this string fileOrFolder)
        {
            if (File.Exists(fileOrFolder)) return new List<string> {fileOrFolder};

            return Directory.GetFiles(fileOrFolder, "*.jpg");
        }
    }
}