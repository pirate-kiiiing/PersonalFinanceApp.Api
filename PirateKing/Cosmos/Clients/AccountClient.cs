using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PirateKing.Guards;
using PirateKing.Models;

namespace PirateKing.Cosmos
{
    /// <summary>
    /// Account Client
    /// </summary>
    public class AccountClient : IAccountClient
    {
        private readonly ICosmosClient cosmosClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cosmosClient"></param>
        public AccountClient(ICosmosClient cosmosClient)
        {
            Validate.NotNull(cosmosClient, nameof(cosmosClient));

            this.cosmosClient = cosmosClient;
        }

        /// <summary>
        /// Get all accounts of a tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of <see cref="Account"/></returns>
        public async Task<IReadOnlyList<Account>> GetAsync(Guid tenantId)
        {
            Validate.NotEmpty(tenantId, nameof(tenantId));

            IEnumerable<Account> accounts = 
                await cosmosClient.ReadAllDocumentsAsync<Account>(
                    CosmosConstants.DatabaseName, 
                    CollectionNames.Accounts, 
                    tenantId.ToString(), 
                    filter: null);

            return accounts.ToList();
        }

        /// <summary>
        /// Get all accounts of a tenant with the specified access token
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of <see cref="Account"/></returns>
        public async Task<IReadOnlyList<Account>> GetAsync(
            Guid tenantId,
            Guid accessToken)
        {
            Validate.NotEmpty(tenantId, nameof(tenantId));

            var filter = new FieldFilter(nameof(accessToken), accessToken.ToString());

            IEnumerable<Account> accounts = 
                await cosmosClient.ReadAllDocumentsAsync<Account>(
                    CosmosConstants.DatabaseName,
                    CollectionNames.Accounts,
                    tenantId.ToString(),
                    filter);

            return accounts.ToList();
        }

        /// <summary>
        /// Get all asset accounts
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of <see cref="Account"/></returns>
        public async Task<IReadOnlyList<Account>> GetAssetAccountsAsync(Guid tenantId)
        {
            Validate.NotEmpty(tenantId, nameof(tenantId));

            var filter = new FieldFilter("assetType", null, operation: "!=");

            IEnumerable<Account> accounts =
                await cosmosClient.ReadAllDocumentsAsync<Account>(
                    CosmosConstants.DatabaseName,
                    CollectionNames.Accounts,
                    tenantId.ToString(),
                    filter);

            return accounts.ToList();
        }
    }

    public interface IAccountClient
    {
        /// <summary>
        /// Get all accounts of a tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of <see cref="Account"/></returns>
        Task<IReadOnlyList<Account>> GetAsync(Guid tenantId);

        /// <summary>
        /// Get all accounts of a tenant with the specified access token
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of <see cref="Account"/></returns>
        Task<IReadOnlyList<Account>> GetAsync(
            Guid tenantId,
            Guid accessToken);

        /// <summary>
        /// Get all asset accounts
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of <see cref="Account"/></returns>
        Task<IReadOnlyList<Account>> GetAssetAccountsAsync(Guid tenantId);
    }
}
