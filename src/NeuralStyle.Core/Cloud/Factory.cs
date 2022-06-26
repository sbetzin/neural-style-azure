using System;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;

namespace NeuralStyle.Core.Cloud
{
    public static class Factory
    {
        public static QueueClient ConstructQueue(string queueName)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString");
            var queue = QueueAdapter.GetAzureQueue(connectionString, queueName);

            return queue;
        }

        public static BlobContainerClient ConstructContainer(string containerName)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString");
            var container = BlobAdapter.GetBlobContainer(connectionString, containerName);

            return container;
        }
    }
}