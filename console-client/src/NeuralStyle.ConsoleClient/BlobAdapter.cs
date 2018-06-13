using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace NeuralStyle.ConsoleClient
{
    public static class BlobAdapter
    {
        public static async Task UploadToBlob(this string file, CloudBlobContainer container)
        {
            var name = Path.GetFileName(file);
            var blob = container.GetBlockBlobReference(name);

            if (blob.ExistsAsync().Result)
            {
                var info = new FileInfo(file);

                if (info.Length == blob.Properties.Length) return;
            }

            Console.WriteLine($"   Uploading {file}");

            await blob.UploadFromFileAsync(file);
        }
    }
}