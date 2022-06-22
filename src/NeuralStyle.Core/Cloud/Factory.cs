using System;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace NeuralStyle.Core.Cloud
{
    public static class Factory
    {
        public static CloudQueue ConstructQueue(string queueName)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString");
            var queue = QueueAdapter.GetAzureQueue(connectionString, queueName);

            return queue;
        }

        public static CloudBlobContainer ConstructContainer(string containerName)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString");
            var container = BlobAdapter.GetBlobContainer(connectionString, containerName);

            return container;
        }
    }
}