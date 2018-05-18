using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace NeuralStyle.ConsoleClient
{
    public static class FileExtensions
    {
        public static async Task<string> UploadToBlob(this string file, CloudBlobContainer container)
        {
            var name = Path.GetFileNameWithoutExtension(file);
            var blob = container.GetBlockBlobReference(name);

            await blob.UploadFromFileAsync(file);
        
            return name;
        }
    }
}