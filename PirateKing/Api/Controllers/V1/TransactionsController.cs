using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PirateKing.Constants;
using PirateKing.Contracts.V1;
using PirateKing.Core;
using PirateKing.Guards;
using PirateKing.Models;
using PirateKing.Plaid.V1;
using PirateKing.Api.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace PirateKing.Api.Controllers.V1
{
    /// <summary>
    /// Transactions controller for Get/Create/Update/Delete operations
    /// </summary>
    [Route("v1.0/tenants/{tenantId}/transactions")]
    [ApiController]
    public class TransactionsControllerV1 : BaseControllerV1
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dependencyFactory"></param>
        public TransactionsControllerV1(IDependencyFactory dependencyFactory) : base(dependencyFactory) { }

        /// <summary>
        /// Get transactions in between startDate and endDate
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>List of <see cref=""/></returns>
        [HttpGet, Route("")]
        [AllowedRoles(UserRole.Admin, UserRole.User)]
        public async Task<IActionResult> GetAsync(
            Guid tenantId,
            [FromQuery] Date startDate,
            [FromQuery] Date endDate)
        {
            HttpValidate.NotEmpty(tenantId, nameof(tenantId));
            HttpValidate.ValidRange(startDate, endDate);

            IEnumerable<Transaction> transactions = await transactionClient.GetAsync(tenantId, startDate, endDate);
            IReadOnlyList<GetTransactionResponseContractV1> response
                = transactions
                    .Select(t => new GetTransactionResponseContractV1
                    {
                        AccountId = t.AccountId,
                        Amount = t.Amount,
                        Date = t.Date,
                        ExpenseCategory = t.ExpenseCategory,
                        IsPending = (t.VerifiedDate.HasValue == false) && (t.PlaidTransactionId.IsNullOrEmpty() == true),
                        MergedDate = t.MergedDate,
                        Name = t.Name,
                        Note = t.Note,
                        TenantId = t.TenantId,
                        TransactionId = t.TransactionId,
                        VerifiedDate = t.VerifiedDate,
                    })
                    .OrderByDescending(x => x.Date)
                    .ToList();

            return Ok(response);
        }

        /// <summary>
        /// Update transactions via Plaid Webhook
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="contract"><see cref="TransactionWebhookRequestContractV1"/></param>
        /// <returns><see cref="HttpStatusCode"/></returns>
        [HttpPost, Route("")]
        public async Task<IActionResult> PostAsync(
            Guid tenantId,
            [FromBody] TransactionWebhookRequestContractV1 contract)
        {
            HttpValidate.NotEmpty(tenantId, nameof(tenantId));
            HttpValidate.NotNull(contract, nameof(contract));

            if (contract.WebhookCode == TransactionWebhookCode.DEFAULT_UPDATE)
            {
                var queueMessage = new SyncTransactionQueue
                {
                    TenantId = tenantId,
                    ItemId = contract.ItemId,
                };

                await cloudQueueClient.EnqueueMessage(QueueNames.SyncTransactions, queueMessage);
            }

            return Accepted();
        }

        /// <summary>
        /// Put transactions
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="transactions"></param>
        /// <returns><see cref="HttpStatusCode"/></returns>
        [HttpPut, Route("")]
        [AllowedRoles(UserRole.Admin, UserRole.User)]
        public async Task<IActionResult> PutAsync(
            Guid tenantId,
            [FromBody] IReadOnlyList<PutTransactionRequestContractV1> transactions)
        {
            HttpValidate.NotEmpty(tenantId, nameof(tenantId));
            HttpValidate.NotNullOrEmpty(transactions, nameof(transactions));

            IEnumerable<Task> tasks = transactions.Select(t => transactionClient.UpsertAsync(t));

            await Task.WhenAll(tasks);

            return NoContent();
        }

        /// <summary>
        /// Put transaction
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="transactionId"></param>
        /// <param name="transaction"></param>
        /// <returns><see cref="HttpStatusCode"/></returns>
        [HttpPut, Route("{transactionId}")]
        [AllowedRoles(UserRole.Admin, UserRole.User)]
        public async Task<IActionResult> PutAsync(
            Guid tenantId,
            Guid transactionId,
            [FromBody] PutTransactionRequestContractV1 transaction)
        {
            HttpValidate.NotEmpty(tenantId, nameof(tenantId));
            HttpValidate.NotEmpty(transactionId, nameof(transactionId));
            HttpValidate.NotNull(transaction, nameof(transaction));
            HttpValidate.AreEqual(transactionId, transaction.TransactionId, $"{nameof(transactionId)}");
            HttpValidate.AreEqual(tenantId, transaction.TenantId, $"{nameof(tenantId)}");

            await transactionClient.UpsertAsync(transaction);

            return NoContent();
        }

        /// <summary>
        /// Merge settled transaction with the specified pending transaction
        /// and deletes the latter
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="settledTransactionId">Settled Transaction Identifier</param>
        /// <param name="contract"><see cref="MergeTransactionsRequestContractV1"/></param>
        /// <returns>Settled <see cref="Transaction"/></returns>
        [HttpPut, Route("{settledTransactionId}/merge")]
        [AllowedRoles(UserRole.Admin, UserRole.User)]
        public async Task<IActionResult> MergeAsync(
            Guid tenantId,
            Guid settledTransactionId,
            [FromBody] MergeTransactionsRequestContractV1 contract)
        {
            HttpValidate.NotEmpty(tenantId, nameof(tenantId));
            HttpValidate.NotNull(contract, nameof(contract));
            HttpValidate.NotEmpty(contract.PendingTransactionId, nameof(contract.PendingTransactionId));

            Guid pendingTransactionId = contract.PendingTransactionId;
            Task<Transaction> settledTransactionTask = transactionClient.GetAsync(tenantId, settledTransactionId);
            Task<Transaction> pendingTransactionTask = transactionClient.GetAsync(tenantId, pendingTransactionId);

            await Task.WhenAll(settledTransactionTask, pendingTransactionTask);

            Transaction settledTransaction = await settledTransactionTask;
            Transaction pendingTransaction = await pendingTransactionTask;

            if (settledTransaction == null) return BadRequest($"Invalid {nameof(settledTransactionId)} {settledTransactionId}");
            if (pendingTransaction == null) return BadRequest($"Invalid {nameof(contract.PendingTransactionId)} {contract.PendingTransactionId}");
            // merge transactions
            settledTransaction.Date = pendingTransaction.Date;
            settledTransaction.ExpenseCategory = pendingTransaction.ExpenseCategory;
            settledTransaction.Note = pendingTransaction.Note;
            settledTransaction.PlaidPendingId = pendingTransaction.PlaidPendingId;
            settledTransaction.MergedDate = DateTime.UtcNow;
            settledTransaction.VerifiedDate = contract.VerifiedDate;

            Task<Transaction> updateTransactionTask = transactionClient.UpsertAsync(settledTransaction);
            Task<Transaction> deleteTransactionTask = transactionClient.DeleteAsync(tenantId, pendingTransactionId);

            await Task.WhenAll(updateTransactionTask, deleteTransactionTask);

            Transaction mergedTransaction = await updateTransactionTask;
            var response = new MergeTransactionsResponseContractV1
            {
                AccountId = mergedTransaction.AccountId,
                Amount = mergedTransaction.Amount,
                Date = mergedTransaction.Date,
                ExpenseCategory = mergedTransaction.ExpenseCategory,
                IsPending = false,
                MergedDate = mergedTransaction.MergedDate,
                Name = mergedTransaction.Name,
                Note = mergedTransaction.Note,
                TenantId = mergedTransaction.TenantId,
                TransactionId = mergedTransaction.TransactionId,
                VerifiedDate = mergedTransaction.VerifiedDate,
            };

            return Ok(response);
        }

        /// <summary>
        /// Delete specified transactions
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="ids"></param>
        /// <returns><see cref="HttpStatusCode"/></returns>
        [HttpDelete, Route("")]
        [AllowedRoles(UserRole.Admin, UserRole.User)]
        public async Task<IActionResult> DeleteAsync(
            Guid tenantId,
            [FromQuery] string ids)
        {
            HttpValidate.NotEmpty(tenantId, nameof(tenantId));
            HttpValidate.NotNullOrEmpty(ids, nameof(ids));

            IEnumerable<Guid> transactionIds;

            try
            {
                IEnumerable<string> idList = ids.Split(",");
                transactionIds = idList.Select(id => new Guid(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            await transactionClient.DeleteAsync(tenantId, transactionIds.ToList());

            return NoContent();
        }
    }
}
