using System.Threading.Tasks;
using AzureStorage = Microsoft.WindowsAzure.Storage;
using AzureStorageQueue = Microsoft.WindowsAzure.Storage.Queue;
using PirateKing.Core;
using PirateKing.Guards;

namespace PirateKing.CloudQueue
{
    public class CloudQueueClient : ICloudQueueClient
    {
        private readonly AzureStorageQueue.CloudQueueClient azureQueueClient;

        public CloudQueueClient(string connectionString)
        {
            Validate.NotNullOrEmpty(connectionString, nameof(connectionString));
            // Create queue
            // Retrieve storage account from connection string.
            AzureStorage.CloudStorageAccount storageAccount = AzureStorage.CloudStorageAccount.Parse(connectionString);
            // Create the queue client.
            this.azureQueueClient = storageAccount.CreateCloudQueueClient();
        }

        public async Task<bool> EnqueueMessage(string queueName, object content)
        {
            Validate.NotNullOrEmpty(queueName, nameof(queueName));
            Validate.NotNull(content, nameof(content));

            string message = content.Serialize();

            try
            {
                AzureStorageQueue.CloudQueue queue = azureQueueClient.GetQueueReference(queueName);
                AzureStorageQueue.CloudQueueMessage queueMessage = new AzureStorageQueue.CloudQueueMessage(message);

                await queue.CreateIfNotExistsAsync();
                await queue.AddMessageAsync(queueMessage);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public interface ICloudQueueClient
    {
        Task<bool> EnqueueMessage(string queueName, object content);
    }
}