using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace PirateKing.Functions.Orchestrators
{
    /// <summary>
    /// Orchestrator function that syncs assets
    /// </summary>
    public class SyncAssetsOrchestrator
    {
        private static readonly TimeSpan retryInterval = TimeSpan.FromSeconds(30);
        private const int maxRetryCount = 3;

        [FunctionName(nameof(SyncAssetsOrchestrator))]
        public async Task OrchestrateSyncAssetsAsync(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            ILogger logger)
        {
            var retryOption = new RetryOptions(retryInterval, maxRetryCount);

            Task syncAccountCatalogTask = context.CallActivityWithRetryAsync(
                nameof(Activities.SyncAccountCatalogs), 
                retryOption, 
                null);

            try
            {
                await Task.WhenAll(syncAccountCatalogTask);
            }
            catch { }

            logger.LogInformation($"{nameof(Activities.SyncAccountCatalogs)} " +
                $"{nameof(syncAccountCatalogTask.IsCompletedSuccessfully)}: {syncAccountCatalogTask.IsCompletedSuccessfully}");
        }
    }
}
