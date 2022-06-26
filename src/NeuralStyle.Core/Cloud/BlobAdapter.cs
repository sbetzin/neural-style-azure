using System.IO;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace NeuralStyle.Core.Cloud
{
    public static class BlobAdapter
    {
        public static BlobContainerClient GetBlobContainer(string connectionString, string containerName)
        {
            var container = new BlobContainerClient(connectionString, containerName);

            return container;
        }

        private static BlobContainerClient EnsureThatExists(this BlobContainerClient blobContainer)
        {
            if (!blobContainer.ExistsAsync().Result)
            {
                blobContainer.CreateAsync().Wait();
            }

            return blobContainer;
        }

        public static void UploadImages(this BlobContainerClient blobContainer, string[] images)
        {
            Logger.Log($"checking {images.Length} images for upload");
            foreach (var image in images)
            {
                image.UploadToBlob(blobContainer);
            }
        }

        public static void UploadToBlob(this string file, BlobContainerClient container)
        {
            var name = Path.GetFileName(file);
            var blob = container.GetBlobClient(name);


            if ((bool)blob.Exists())
            {
                var info = new FileInfo(file);
                var props = (BlobProperties)blob.GetProperties();

                if (info.Length == props.ContentLength)
                {
                    return;
                }
            }

            Logger.Log($"   Uploading {file}");

            blob.Upload(file, true);
        }

        public static void UploadTextToBlob(this string file, BlobContainerClient container)
        {
            var name = Path.GetFileName(file);
            var blob = container.GetBlobClient(name);

            var headers = new BlobHttpHeaders { ContentType = "text/html" };


            Logger.Log($"   Uploading {file}");
            blob.Upload(file, headers);
        }
    }
}