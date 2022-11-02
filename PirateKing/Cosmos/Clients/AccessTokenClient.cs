using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PirateKing.Guards;
using PirateKing.Models;

namespace PirateKing.Cosmos
{
    /// <summary>
    /// Access Token Client
    /// </summary>
    public class AccessTokenClient : IAccessTokenClient
    {
        private readonly ICosmosClient cosmosClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cosmosClient"></param>
        public AccessTokenClient(ICosmosClient cosmosClient)
        {
            Validate.NotNull(cosmosClient, nameof(cosmosClient));

            this.cosmosClient = cosmosClient;
        }

        /// <summary>
        /// Get plaid access token of a tenant with the specified itemId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="itemId"></param>
        /// <param name="state"></param>
        /// <returns><see cref="AccessToken"/></returns>
        public async Task<AccessToken> GetAsync(Guid tenantId, string itemId, AccessTokenState state = AccessTokenState.None)
        {
            Validate.NotEmpty(tenantId, nameof(tenantId));
            Validate.NotNullOrEmpty(itemId, nameof(itemId));

            var filter = new FieldFilter(nameof(itemId), itemId);

            if (state != AccessTokenState.None)
            {
                filter.Add(nameof(state), state.ToString());
            }

            IEnumerable<AccessToken> accessTokens =
                await cosmosClient.ReadAllDocumentsAsync<AccessToken>(
                    CosmosConstants.DatabaseName,
                    CollectionNames.AccessTokens,
                    tenantId.ToString(),
                    filter);

            return accessTokens.Single();
        }

        /// <summary>
        /// Get all plaid access tokens of a tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of <see cref="AccessToken"/></returns>
        public async Task<IReadOnlyList<AccessToken>> GetAsync(Guid tenantId)
        {
            Validate.NotEmpty(tenantId, nameof(tenantId));

            IEnumerable<AccessToken> accessTokens = 
                await cosmosClient.ReadAllDocumentsAsync<AccessToken>(
                    CosmosConstants.DatabaseName,
                    CollectionNames.AccessTokens,
                    tenantId.ToString(),
                    filter: null);

            return accessTokens.ToList();
        }
    }

    public interface IAccessTokenClient
    {
        /// <summary>
        /// Get plaid access token of a tenant with the specified itemId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="itemId"></param>
        /// <param name="state"></param>
        /// <returns><see cref="AccessToken"/></returns>
        Task<AccessToken> GetAsync(Guid tenantId, string itemId, AccessTokenState state = AccessTokenState.None);

        /// <summary>
        /// Get all plaid access tokens of a tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of <see cref="AccessToken"/></returns>
        Task<IReadOnlyList<AccessToken>> GetAsync(Guid tenantId);
    }
}
