using System;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace NeuralStyle.Core.Cloud
{
    public static class Factory
    {
        public static (CloudQueue queue, CloudBlobContainer container) Construct()
        {
            var queueName = "jobs";
            var containerName = "images";

            var connectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString");
            var queue = QueueAdapter.GetAzureQueue(connectionString, queueName);
            var container = BlobAdapter.GetBlobContainer(connectionString, containerName);

            return (queue, container);
        }
    }
}