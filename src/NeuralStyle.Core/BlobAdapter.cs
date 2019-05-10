using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace NeuralStyle.Core
{
    public static class BlobAdapter
    {
        public static CloudBlobContainer GetBlobContainer(string connectionString, string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var blobContainer = blobClient.GetContainerReference(containerName).EnsureThatExists();

            return blobContainer;
        }

        private static CloudBlobContainer EnsureThatExists(this CloudBlobContainer blobContainer)
        {
            if (!blobContainer.ExistsAsync().Result)
            {
                blobContainer.CreateAsync().Wait();
            }

            return blobContainer;
        }

        public static void UploadImages(this CloudBlobContainer blobContainer, string[] images)
        {
            Console.WriteLine($"checking {images.Length} images for upload");
            foreach (var image in images)
            {
                image.UploadToBlob(blobContainer).Wait();
            }
        }

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