using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PirateKing.Core;
using PirateKing.Exceptions;
using PirateKing.Guards;
using PirateKing.Models;

namespace PirateKing.Cosmos
{
    /// <summary>
    /// Account Catalog Client
    /// </summary>
    public class AccountCatalogClient : IAccountCatalogClient
    {
        private readonly ICosmosClient cosmosClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cosmosClient"></param>
        public AccountCatalogClient(ICosmosClient cosmosClient)
        {
            Validate.NotNull(cosmosClient, nameof(cosmosClient));

            this.cosmosClient = cosmosClient;
        }

        /// <summary>
        /// Creates <see cref="AccountCatalog"/>
        /// </summary>
        /// <param name="accountCatalog"></param>
        /// <returns><see cref="AccountCatalog"/></returns>
        public async Task<AccountCatalog> CreateAsync(AccountCatalog accountCatalog)
        {
            Validate.NotNull(accountCatalog, nameof(accountCatalog));
            Validate.NotEmpty(accountCatalog.TenantId, nameof(accountCatalog.TenantId));
            Validate.IsNullOrEmpty(accountCatalog.Etag, nameof(accountCatalog.Etag));

            return await cosmosClient.CreateDocumentAsync(
                CosmosConstants.DatabaseName,
                CollectionNames.AccountCatalogs,
                accountCatalog.TenantId.ToString(),
                accountCatalog);
        }

        /// <summary>
        /// Get account catalog with the specified accountCatalogId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="accountCatalogId"></param>
        /// <returns><see cref="AccountCatalog"/></returns>
        public async Task<AccountCatalog> GetAsync(
            Guid tenantId,
            GuidDate accountCatalogId)
        {
            Validate.NotEmpty(tenantId, nameof(tenantId));
            Validate.NotNull(accountCatalogId, nameof(accountCatalogId));

            AccountCatalog accountCatalog =
                await cosmosClient.ReadDocumentAsync<AccountCatalog>(
                    CosmosConstants.DatabaseName,
                    CollectionNames.AccountCatalogs,
                    tenantId.ToString(),
                    accountCatalogId.ToString());

            return accountCatalog;
        }

        /// <summary>
        /// Get all account catalogs of a tenant within the given date range
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>List of <see cref="AccountCatalog"/></returns>
        public async Task<IReadOnlyList<AccountCatalog>> GetAsync(
            Guid tenantId,
            Date startDate,
            Date endDate)
        {
            Validate.NotEmpty(tenantId, nameof(tenantId));
            Validate.ValidRange(startDate, endDate);

            var filter = new GuidDateFilter(startDate, endDate);

            IEnumerable<AccountCatalog> accountCatalogs = 
                await cosmosClient.ReadAllDocumentsAsync<AccountCatalog>(
                    CosmosConstants.DatabaseName,
                    CollectionNames.AccountCatalogs,
                    tenantId.ToString(),
                    filter);

            return accountCatalogs.ToList();
        }

        /// <summary>
        /// Get all account catalogs of a tenant within the given date range
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="accountId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>List of <see cref="AccountCatalog"/></returns>
        public async Task<IReadOnlyList<AccountCatalog>> GetAsync(
            Guid tenantId, 
            Guid accountId, 
            Date startDate,
            Date endDate)
        {
            Validate.NotEmpty(tenantId, nameof(tenantId));
            Validate.NotEmpty(accountId, nameof(accountId));
            Validate.ValidRange(startDate, endDate);

            var filter = new GuidDateFilter(accountId, startDate, endDate);

            IEnumerable<AccountCatalog> accountCatalogs = 
                await cosmosClient.ReadAllDocumentsAsync<AccountCatalog>(
                    CosmosConstants.DatabaseName,
                    CollectionNames.AccountCatalogs,
                    tenantId.ToString(),
                    filter);

            return accountCatalogs.ToList();
        }

        /// <summary>
        /// Updates <see cref="AccountCatalog"/>
        /// </summary>
        /// <param name="accountCatalog"></param>
        /// <returns><see cref="AccountCatalog"/></returns>
        public async Task<AccountCatalog> UpdateAsync(AccountCatalog accountCatalog)
        {
            Validate.NotNull(accountCatalog, nameof(accountCatalog));
            Validate.NotEmpty(accountCatalog.TenantId, nameof(accountCatalog.TenantId));
            Validate.NotNullOrEmpty(accountCatalog.Etag, nameof(accountCatalog.Etag));

            return await cosmosClient.UpdateDocumentAsync(
                CosmosConstants.DatabaseName,
                CollectionNames.AccountCatalogs,
                accountCatalog.AccountCatalogId.Id,
                accountCatalog.TenantId.ToString(),
                accountCatalog,
                accountCatalog.Etag);
        }

        /// <summary>
        /// Force Updates <see cref="AccountCatalog"/> by retrieving the latest etag
        /// </summary>
        /// <param name="accountCatalog"></param>
        /// <returns><see cref="AccountCatalog"/></returns>
        public async Task<AccountCatalog> ForceUpdateAsync(AccountCatalog accountCatalog)
        {
            Validate.NotNull(accountCatalog, nameof(accountCatalog));
            Validate.NotEmpty(accountCatalog.TenantId, nameof(accountCatalog.TenantId));

            AccountCatalog current = await GetAsync(accountCatalog.TenantId, new GuidDate(accountCatalog.AccountCatalogId.Id));

            return await cosmosClient.UpdateDocumentAsync(
                CosmosConstants.DatabaseName,
                CollectionNames.AccountCatalogs,
                accountCatalog.AccountCatalogId.Id,
                accountCatalog.TenantId.ToString(),
                accountCatalog,
                current.Etag);
        }

        /// <summary>
        /// Updates a list of <see cref="AccountCatalog"/>
        /// </summary>
        /// <param name="accountCatalogs"></param>
        /// <returns>Update results</returns>
        public async Task<IReadOnlyList<(AccountCatalog, HttpStatusCode, Exception)>> 
            UpdateAsync(IReadOnlyList<AccountCatalog> accountCatalogs)
        {
            Validate.NotNullOrEmpty(accountCatalogs, nameof(accountCatalogs));

            IReadOnlyList<Task<(AccountCatalog, HttpStatusCode, Exception)>> tasks = 
                accountCatalogs
                    .Select(catalog => UpdateAsyncWrapper(catalog))
                    .ToList();

            return await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Upserts <see cref="AccountCatalog"/>
        /// </summary>
        /// <param name="accountCatalog"></param>
        /// <returns><see cref="AccountCatalog"/></returns>
        public async Task<AccountCatalog> UpsertAsync(AccountCatalog accountCatalog)
        {
            Validate.NotNull(accountCatalog, nameof(accountCatalog));
            Validate.NotEmpty(accountCatalog.TenantId, nameof(accountCatalog.TenantId));

            AccountCatalog existing = 
                await cosmosClient.ReadDocumentAsync<AccountCatalog>(
                    CosmosConstants.DatabaseName,
                    CollectionNames.AccountCatalogs,
                    accountCatalog.TenantId.ToString(),
                    accountCatalog.AccountCatalogId.Id);

            string etag = (existing != null) ? existing.Etag : null;

            return await cosmosClient.UpsertDocumentAsync(
                CosmosConstants.DatabaseName,
                CollectionNames.AccountCatalogs,
                accountCatalog.TenantId.ToString(),
                accountCatalog,
                etag);
        }

        private async Task<(AccountCatalog, HttpStatusCode, Exception)> UpdateAsyncWrapper(AccountCatalog accountCatalog)
        {
            try
            {
                return (await UpdateAsync(accountCatalog), HttpStatusCode.OK, null);
            }
            catch (HttpException e)
            {
                return (accountCatalog, e.StatusCode, e);
            }
            catch (Exception e)
            {
                return (accountCatalog, HttpStatusCode.InternalServerError, e);
            }
        }
    }

    public interface IAccountCatalogClient
    {
        /// <summary>
        /// Creates <see cref="AccountCatalog"/>
        /// </summary>
        /// <param name="accountCatalog"></param>
        /// <returns><see cref="AccountCatalog"/></returns>
        Task<AccountCatalog> CreateAsync(AccountCatalog accountCatalog);

        /// <summary>
        /// Get account catalog with the specified accountCatalogId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="accountCatalogId"></param>
        /// <returns><see cref="AccountCatalog"/></returns>
        Task<AccountCatalog> GetAsync(
            Guid tenantId,
            GuidDate accountCatalogId);

        /// <summary>
        /// Get all account catalogs of a tenant within the given date range
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>List of <see cref="AccountCatalog"/></returns>
        Task<IReadOnlyList<AccountCatalog>> GetAsync(
            Guid tenantId,
            Date startDate,
            Date endDate);

        /// <summary>
        /// Get all account catalogs of a tenant within the given date range
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="accountId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>List of <see cref="AccountCatalog"/></returns>
        Task<IReadOnlyList<AccountCatalog>> GetAsync(
            Guid tenantId, 
            Guid accountId, 
            Date startDate, 
            Date endDate);

        /// <summary>
        /// Updates <see cref="AccountCatalog"/>
        /// </summary>
        /// <param name="accountCatalog"></param>
        /// <returns><see cref="AccountCatalog"/></returns>
        Task<AccountCatalog> UpdateAsync(AccountCatalog accountCatalog);

        /// <summary>
        /// Force Updates <see cref="AccountCatalog"/> by retrieving the latest etag
        /// </summary>
        /// <param name="accountCatalog"></param>
        /// <returns><see cref="AccountCatalog"/></returns>
        Task<AccountCatalog> ForceUpdateAsync(AccountCatalog accountCatalog);

        /// <summary>
        /// Updates a list of <see cref="AccountCatalog"/>
        /// </summary>
        /// <param name="accountCatalogs"></param>
        /// <returns>Update results</returns>
        Task<IReadOnlyList<(AccountCatalog, HttpStatusCode, Exception)>>
            UpdateAsync(IReadOnlyList<AccountCatalog> accountCatalogs);

        /// <summary>
        /// Upserts <see cref="AccountCatalog"/>
        /// </summary>
        /// <param name="accountCatalog"></param>
        /// <returns><see cref="AccountCatalog"/></returns>
        Task<AccountCatalog> UpsertAsync(AccountCatalog accountCatalog);
    }
}
