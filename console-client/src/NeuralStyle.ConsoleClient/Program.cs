using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using NeuralStyle.ConsoleClient.Model;
using Newtonsoft.Json;

namespace NeuralStyle.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=neuralstylefiles;AccountKey=mMxv0dYg1xyEqE5VsrZejnH1PKQL5NsvG2gwYAfyHCrN1LDGYTXztCLoyfXa7ObB9BpPvXhGBtBg2A6owaV3gQ==;EndpointSuffix=core.windows.net");

            var queueClient = storageAccount.CreateCloudQueueClient();

            var blobClient = storageAccount.CreateCloudBlobClient();

            var queue = queueClient.GetQueueReference("jobs");
            queue.EncodeMessage = false;

            var images = blobClient.GetContainerReference("images");

            var sourceId = @"C:\Data\images\in\Ana.jpg".UploadToBlob(images).Result;
            var styleId = @"C:\Data\images\style\karl_otto_goetz_ohne_titel.jpg".UploadToBlob(images).Result;

            var job = new Job() { Source = sourceId, Style = styleId, Sizes = new List<int> { 1500 } };

            var json = JsonConvert.SerializeObject(job);
            var message = new CloudQueueMessage(json);

            queue.AddMessageAsync(message).Wait();
        }
    }
}
