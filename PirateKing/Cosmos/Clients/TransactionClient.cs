using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PirateKing.Contracts.V1;
using PirateKing.Core;
using PirateKing.Guards;
using PirateKing.Models;

namespace PirateKing.Cosmos
{
    /// <summary>
    /// Transaction Client
    /// </summary>
    public class TransactionClient : ITransactionClient
    {
        private readonly ICosmosClient cosmosClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cosmosClient"></param>
        public TransactionClient(ICosmosClient cosmosClient)
        {
            Validate.NotNull(cosmosClient, nameof(cosmosClient));

            this.cosmosClient = cosmosClient;
        }

        /// <summary>
        /// Creates <see cref="Transaction"/>
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns><see cref="Transaction"/></returns>
        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            Validate.NotNull(transaction, nameof(transaction));
            Validate.NotEmpty(transaction.TenantId, nameof(transaction.TenantId));
            Validate.NotEmpty(transaction.AccountId, nameof(transaction.AccountId));
            Validate.IsNullOrEmpty(transaction.Etag, nameof(transaction.Etag));

            return await cosmosClient.CreateDocumentAsync(
                CosmosConstants.DatabaseName,
                CollectionNames.Transactions,
                transaction.TenantId.ToString(),
                transaction);
        }

        /// <summary>
        /// Gets <see cref="Transaction"/>
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="transactionId"></param>
        /// <returns><see cref="Transaction"/></returns>
        public async Task<Transaction> GetAsync(
            Guid tenantId,
            Guid transactionId)
        {
            Validate.NotEmpty(tenantId, nameof(tenantId));
            Validate.NotEmpty(transactionId, nameof(transactionId));

            Transaction transaction = 
                await cosmosClient.ReadDocumentAsync<Transaction>(
                    CosmosConstants.DatabaseName,
                    CollectionNames.Transactions,
                    tenantId.ToString(),
                    transactionId.ToString());

            return transaction;
        }

        /// <summary>
        /// Get all transactions of a tenant within a given date range
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>List of <see cref="Transaction"/></returns>
        public async Task<IEnumerable<Transaction>> GetAsync(
            Guid tenantId,
            Date startDate,
            Date endDate)
        {
            Validate.NotEmpty(tenantId, nameof(tenantId));
            Validate.ValidRange(startDate, endDate);

            var filter = new DateFilter(startDate, endDate);

            IEnumerable<Transaction> transactions = 
                await cosmosClient.ReadAllDocumentsAsync<Transaction>(
                    CosmosConstants.DatabaseName,
                    CollectionNames.Transactions,
                    tenantId.ToString(),
                    filter);

            return transactions;
        }

        /// <summary>
        /// Updates <see cref="Transaction"/>
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns><see cref="Transaction"/></returns>
        public async Task<Transaction> UpdateAsync(Transaction transaction)
        {
            Validate.NotNull(transaction, nameof(transaction));
            Validate.NotEmpty(transaction.TenantId, nameof(transaction.TenantId));
            Validate.NotNullOrEmpty(transaction.Etag, nameof(transaction.Etag));

            return await cosmosClient.UpdateDocumentAsync(
                CosmosConstants.DatabaseName,
                CollectionNames.Transactions,
                transaction.TransactionId.ToString(),
                transaction.TenantId.ToString(),
                transaction,
                transaction.Etag);
        }

        /// <summary>
        /// Upserts <see cref="Transaction"/>
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns><see cref="Transaction"/></returns>
        public async Task<Transaction> UpsertAsync(Transaction transaction)
        {
            Validate.NotNull(transaction, nameof(transaction));
            Validate.NotEmpty(transaction.TenantId, nameof(transaction.TenantId));

            Transaction existing = 
                await cosmosClient.ReadDocumentAsync<Transaction>(
                    CosmosConstants.DatabaseName,
                    CollectionNames.Transactions,
                    transaction.TenantId.ToString(),
                    transaction.TransactionId.ToString());

            string etag = (existing != null) ? existing.Etag : null;

            return await cosmosClient.UpsertDocumentAsync(
                CosmosConstants.DatabaseName,
                CollectionNames.Transactions,
                transaction.TenantId.ToString(),
                transaction,
                etag);
        }

        /// <summary>
        /// Upserts <see cref="Transaction"/>
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns><see cref="Transaction"/></returns>
        public async Task<Transaction> UpsertAsync(PutTransactionRequestContractV1 transaction)
        {
            Validate.NotNull(transaction, nameof(transaction));
            Validate.NotEmpty(transaction.TenantId, nameof(transaction.TenantId));

            Transaction existing =
                await cosmosClient.ReadDocumentAsync<Transaction>(
                    CosmosConstants.DatabaseName,
                    CollectionNames.Transactions,
                    transaction.TenantId.ToString(),
                    transaction.TransactionId.ToString());

            var newTransaction = new Transaction
            {
                AccountId = transaction.AccountId,
                Amount = transaction.Amount,
                CategoryId = existing.CategoryId,
                CurrencyCode = existing.CurrencyCode,
                Date = transaction.Date,
                ExpenseCategory = transaction.ExpenseCategory,
                MergedDate = existing.MergedDate,
                Name = transaction.Name,
                Note = transaction.Note,
                PlaidPendingId = existing.PlaidPendingId,
                PlaidTransactionId = existing.PlaidTransactionId,
                TenantId = transaction.TenantId,
                TransactionId = transaction.TransactionId,
                VerifiedDate = transaction.VerifiedDate,
            };

            return await cosmosClient.UpsertDocumentAsync(
                    CosmosConstants.DatabaseName,
                    CollectionNames.Transactions,
                    transaction.TenantId.ToString(),
                    newTransaction);
        }

        /// <summary>
        /// Deletes <see cref="Transaction"/>
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="transactionId"></param>
        /// <returns><see cref="Transaction"/></returns>
        public async Task<Transaction> DeleteAsync(Guid tenantId, Guid transactionId)
        {
            Validate.NotEmpty(tenantId, nameof(tenantId));
            Validate.NotEmpty(transactionId, nameof(transactionId));

            Transaction transaction = 
                await cosmosClient.DeleteDocumentAsync<Transaction>(
                    CosmosConstants.DatabaseName,
                    CollectionNames.Transactions,
                    tenantId.ToString(),
                    transactionId.ToString());

            return transaction;
        }

        /// <summary>
        /// Deletes a List of <see cref="Transaction"/>
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="transactionIds"></param>
        /// <returns>A list of <see cref="Transaction"/></returns>
        public async Task DeleteAsync(
            Guid tenantId,
            IReadOnlyList<Guid> transactionIds)
        {
            Validate.NotNullOrEmpty(transactionIds, nameof(transactionIds));

            List<Task<Transaction>> deleteTasks =
                transactionIds
                    .Select(id => DeleteAsync(tenantId, id))
                    .ToList();

            await Task.WhenAll(deleteTasks);
        }
    }

    public interface ITransactionClient
    {
        /// <summary>
        /// Creates <see cref="Transaction"/>
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns><see cref="Transaction"/></returns>
        Task<Transaction> CreateAsync(Transaction transaction);

        /// <summary>
        /// Gets <see cref="Transaction"/>
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="transactionId"></param>
        /// <returns><see cref="Transaction"/></returns>
        Task<Transaction> GetAsync(
            Guid tenantId,
            Guid transactionId);

        /// <summary>
        /// Get all transactions of a tenant within a given date range
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>List of <see cref="Transaction"/></returns>
        Task<IEnumerable<Transaction>> GetAsync(
            Guid tenantId,
            Date startDate,
            Date endDate);

        /// <summary>
        /// Updates <see cref="Transaction"/>
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns><see cref="Transaction"/></returns>
        Task<Transaction> UpdateAsync(Transaction transaction);

        /// <summary>
        /// Upserts <see cref="Transaction"/>
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns><see cref="Transaction"/></returns>
        Task<Transaction> UpsertAsync(Transaction transaction);

        /// <summary>
        /// Upserts <see cref="Transaction"/>
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns><see cref="Transaction"/></returns>
        Task<Transaction> UpsertAsync(PutTransactionRequestContractV1 transaction);

        /// <summary>
        /// Deletes <see cref="Transaction"/>
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="transactionId"></param>
        /// <returns><see cref="Transaction"/></returns>
        Task<Transaction> DeleteAsync(
            Guid tenantId, 
            Guid transactionId);

        /// <summary>
        /// Deletes a List of <see cref="Transaction"/>
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="transactionIds"></param>
        /// <returns>A list of <see cref="Transaction"/></returns>
        Task DeleteAsync(
            Guid tenantId,
            IReadOnlyList<Guid> transactionIds);
    }
}
