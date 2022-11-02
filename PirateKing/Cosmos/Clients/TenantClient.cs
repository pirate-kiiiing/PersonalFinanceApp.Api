using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PirateKing.Guards;
using PirateKing.Models;

namespace PirateKing.Cosmos
{
    /// <summary>
    /// Tenants Client
    /// </summary>
    public class TenantClient : ITenantClient
    {
        private readonly ICosmosClient cosmosClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cosmosClient"></param>
        public TenantClient(ICosmosClient cosmosClient)
        {
            Validate.NotNull(cosmosClient, nameof(cosmosClient));

            this.cosmosClient = cosmosClient;
        }

        /// <summary>
        /// Get tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns><see cref="Tenant"/></returns>
        public async Task<Tenant> GetAsync(Guid tenantId)
        {
            Validate.NotEmpty(tenantId, nameof(tenantId));

            Tenant tenant =
                await cosmosClient.ReadDocumentAsync<Tenant>(
                    CosmosConstants.DatabaseName,
                    CollectionNames.Tenants,
                    tenantId.ToString(),
                    tenantId.ToString());

            return tenant;
        }

        /// <summary>
        /// Get all tenants
        /// </summary>
        /// <returns>List of <see cref="Tenant"/></returns>
        public async Task<IReadOnlyList<Tenant>> GetAsync()
        {
            IEnumerable<Tenant> tenants =
                await cosmosClient.ReadAllDocumentsAsync<Tenant>(
                    CosmosConstants.DatabaseName,
                    CollectionNames.Tenants,
                    partitionKey: null,
                    filter: null);

            return tenants.ToList();
        }
    }

    public interface ITenantClient
    {
        /// <summary>
        /// Get tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns><see cref="Tenant"/></returns>
        Task<Tenant> GetAsync(Guid tenantId);

        /// <summary>
        /// Get all tenants
        /// </summary>
        /// <returns>List of <see cref="Tenant"/></returns>
        Task<IReadOnlyList<Tenant>> GetAsync();
    }
}
