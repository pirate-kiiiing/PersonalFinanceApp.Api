using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace PirateKing.Functions
{
    /// <summary>
    /// Defines OrchestrationClients that triggers orchestrators
    /// </summary>
    public class OrchestrationClients
    {
        private const string SyncAssets = nameof(SyncAssets);

        [FunctionName(SyncAssets)]
        public async Task SyncAssetsAsync(
            [OrchestrationClient] DurableOrchestrationClient starter,
            [TimerTrigger(
                "%" + Settings.SyncAssetsSchedule + "%"
#if DEBUG
                ,RunOnStartup = true
#endif
            )]TimerInfo myTimer,
            ILogger logger)
        {
            logger.LogInformation($"Starting Orchestration for {nameof(SyncAssetsAsync)}");

            await starter.StartNewAsync(nameof(Orchestrators.SyncAssetsOrchestrator), null);
        }
    }
}
