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
            var id = Guid.NewGuid().ToString();

            var blob = container.GetBlockBlobReference(id);

            await blob.UploadFromFileAsync(file);
        
            return id;
        }
    }
}