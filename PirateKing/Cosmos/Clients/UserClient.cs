using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PirateKing.Guards;
using PirateKing.Models;

namespace PirateKing.Cosmos
{
    /// <summary>
    /// User Client
    /// </summary>
    public class UserClient : IUserClient
    {
        private readonly ICosmosClient cosmosClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cosmosClient"></param>
        public UserClient(ICosmosClient cosmosClient)
        {
            Validate.NotNull(cosmosClient, nameof(cosmosClient));

            this.cosmosClient = cosmosClient;
        }

        /// <summary>
        /// Get all users in a tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of <see cref="User"/></returns>
        public async Task<IReadOnlyList<User>> GetAsync(Guid tenantId)
        {
            Validate.NotEmpty(tenantId, nameof(tenantId));

            IEnumerable<User> users =
                await cosmosClient.ReadAllDocumentsAsync<User>(
                    CosmosConstants.DatabaseName,
                    CollectionNames.Users,
                    partitionKey: tenantId.ToString(),
                    filter: null);

            return users.ToList();
        }

        /// <summary>
        /// Get all admin users in a tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of <see cref="User"/></returns>
        public async Task<IReadOnlyList<User>> GetAdminsAsync(Guid tenantId)
        {
            Validate.NotEmpty(tenantId, nameof(tenantId));

            var filter = new FieldFilter("role", UserRole.Admin.ToString());

            IEnumerable<User> users =
                await cosmosClient.ReadAllDocumentsAsync<User>(
                    CosmosConstants.DatabaseName,
                    CollectionNames.Users,
                    partitionKey: tenantId.ToString(),
                    filter: filter);

            return users.ToList();
        }

        /// <summary>
        /// Get user
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <returns><see cref="User"/></returns>
        public async Task<User> GetAsync(Guid tenantId, Guid userId)
        {
            Validate.NotEmpty(tenantId, nameof(tenantId));
            Validate.NotEmpty(userId, nameof(userId));

            User user =
                await cosmosClient.ReadDocumentAsync<User>(
                    CosmosConstants.DatabaseName,
                    CollectionNames.Users,
                    partitionKey: tenantId.ToString(),
                    id: userId.ToString());

            return user;
        }
    }

    public interface IUserClient
    {
        /// <summary>
        /// Get all users in a tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of <see cref="User"/></returns>
        Task<IReadOnlyList<User>> GetAsync(Guid tenantId);

        /// <summary>
        /// Get all admin users in a tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of <see cref="User"/></returns>
        Task<IReadOnlyList<User>> GetAdminsAsync(Guid tenantId);

        /// <summary>
        /// Get user
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <returns><see cref="User"/></returns>
        Task<User> GetAsync(Guid tenantId, Guid userId);
    }
}
