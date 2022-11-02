using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using PirateKing.Core;
using PirateKing.Cosmos;
using PirateKing.Diagnostic;
using PirateKing.Guards;
using PirateKing.Models;
using PirateKing.Plaid.V1;
using PirateKing.Constants;
using PirateKing.Tools;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using PirateKing.Email;
using PirateKing.Exceptions;

namespace PirateKing.Functions.Activities
{
    [DependencyInjectionConfig(typeof(AutofacConfig))]
    public class SyncTransactions
    {
        private const int plaidStartDayOffset = -5;
        private const int tenantStartDayOffset = plaidStartDayOffset - 10;

        private IAccessTokenClient accessTokenClient;
        private IAccountClient accountClient;
        private IPirateKingEmailClient emailClient;
        private IPlaidClientV1 plaidClient;
        private ITransactionClient transactionClient;
        private ITenantClient tenantClient;
        private ILogger log;

        [FunctionName(nameof(SyncTransactions))]
        public async Task SyncTransactionsAsync(
            [QueueTrigger(QueueNames.SyncTransactions, Connection = Settings.StorageConnectionString)]string queueMessage,
            [Inject] IAccessTokenClient accessTokenClient,
            [Inject] IAccountClient accountClient,
            [Inject] IPirateKingEmailClient emailClient,
            [Inject] IPlaidClientV1 plaidClient,
            [Inject] ITransactionClient transactionClient,
            [Inject] ITenantClient tenantClient,
            ILogger log)
        {
            Validate.NotNull(accessTokenClient, nameof(accessTokenClient));
            Validate.NotNull(accountClient, nameof(accountClient));
            Validate.NotNull(emailClient, nameof(emailClient));
            Validate.NotNull(plaidClient, nameof(plaidClient));
            Validate.NotNull(transactionClient, nameof(transactionClient));
            Validate.NotNull(tenantClient, nameof(tenantClient));
            Validate.NotNull(log, nameof(log));

            this.accessTokenClient = accessTokenClient;
            this.accountClient = accountClient;
            this.emailClient = emailClient;
            this.plaidClient = plaidClient;
            this.transactionClient = transactionClient;
            this.tenantClient = tenantClient;
            this.log = log;

            log.LogInformation($"C# Queue trigger function processed: {queueMessage}");

            try
            {
                await SyncTransactionsAsync(queueMessage);

                log.LogInformation($"{nameof(SyncTransactions)} successful");
            }
            catch (Exception e)
            {
                string errorDetail = e.ToString();

                log.LogError($"{nameof(SyncTransactionsAsync)} error - {e.Message}");
                log.LogError($"{errorDetail}");
            }
        }

        private async Task SyncTransactionsAsync(string queueMessage)
        {
            if (queueMessage.TryDeserialize(out SyncTransactionQueue queue) == false)
            {
                throw new ArgumentException($"invalid {nameof(queueMessage)}: {queueMessage}");
            }

            Validate.NotNull(queue, nameof(queue));
            Validate.NotNullOrEmpty(queue.ItemId, nameof(queue.ItemId));
            Validate.NotEmpty(queue.TenantId, nameof(queue.TenantId));

            Guid tenantId = queue.TenantId;
            string itemId = queue.ItemId;

            log.LogInformation($"Getting tenant and access token info...");

            Task<AccessToken> accessTokenTask = accessTokenClient.GetAsync(tenantId, itemId, AccessTokenState.Active);
            Task<Tenant> tenantTask = tenantClient.GetAsync(tenantId);

            await Task.WhenAll(accessTokenTask, tenantTask);

            AccessToken accessToken = await accessTokenTask;
            Tenant tenant = await tenantTask;

            if (accessToken == null)
            {
                log.LogInformation($"AccessToken for {nameof(itemId)} {itemId} does not exist.");
                return;
            }

            log.LogInformation($"Getting tenant and access token info success! " +
                $"{nameof(accessToken)}: {accessToken.AccessTokenId}");

            await SyncTransactionsAsync(tenant, accessToken);
        }

        private async Task SyncTransactionsAsync(Tenant tenant, AccessToken accessToken)
        {
            Guid tenantId = tenant.TenantId;
            // get plaid transactions and tenant transactions in parallel
            Task<GetTransactionsResponseContractV1> getPlaidTransactionsTask = GetPlaidTransactionsAsync(tenant, accessToken);
            Task<IEnumerable<Transaction>> getTenantTransactionsTask = GetTenantTransactionsAsync(tenant);

            await Task.WhenAll(getPlaidTransactionsTask, getTenantTransactionsTask);

            GetTransactionsResponseContractV1 plaidResponse = await getPlaidTransactionsTask;
            IEnumerable<Transaction> tenantTransactions = await getTenantTransactionsTask;
            IEnumerable<PlaidTransactionContractV1> plaidTransactions = plaidResponse.Transactions;

            if (plaidTransactions.IsNullOrEmpty() == true)
            {
                log.LogInformation($"No transaction updates from plaid for " +
                    $"{nameof(tenant)} {tenantId} with {nameof(accessToken)} {accessToken.AccessTokenId}");
                return;
            }
            if (tenantTransactions == null)
            {
                string errorMessage = $"Failed to fetch any transactions from Azure for tenant {tenantId}";
                log.LogError(errorMessage);

                throw new SystemException(errorMessage);
            }

            await SyncTransactionsAsync(tenant, accessToken, plaidTransactions, tenantTransactions);

            log.LogInformation($"{nameof(SyncTransactionsAsync)} for tenant {tenantId} " +
                $"with {nameof(accessToken)} {accessToken.AccessTokenId} successful");
        }

        private async Task<GetTransactionsResponseContractV1> GetPlaidTransactionsAsync(Tenant tenant, AccessToken accessToken)
        {
            Guid accessTokenId = accessToken.AccessTokenId;
            DateTime localDateTime = tenant.LocalNow;
            Date localDate = localDateTime.ToDate();
            Date startDate = localDate.AddDays(plaidStartDayOffset);
            Date endDate = localDate.AddDays(1);

            var contract = new GetTransactionsRequestContractV1
            {
                AccessToken = accessTokenId,
                StartDate = startDate,
                EndDate = endDate,
            };

            try
            {
                log.LogInformation($"Requesting {nameof(GetPlaidTransactionsAsync)}({accessTokenId}) " +
                    $"for {nameof(tenant)} {tenant.TenantId}...");

                GetTransactionsResponseContractV1 response
                    = await plaidClient.GetTransactionsAsync(contract);

                log.LogInformation($"{nameof(GetPlaidTransactionsAsync)} success!");
                log.LogInformation($"{response.Serialize()}");

                return response;
            }
            catch (HttpException e) when (e.Content is PlaidErrorResponseContractV1 error)
            {
                log.LogError($"Plaid {nameof(GetPlaidTransactionsAsync)} error: {contract.Serialize()}");
                log.LogError($"{e.Message}");

                if (error?.ErrorType != PlaidErrorTypeContractV1.INSTITUTION_ERROR.ToString() ||
                    error?.SuggestedAction.IsNullOrEmpty() == false)
                {
                    await FunctionNotifier.SendEmailAsync(
                        emailClient: emailClient,
                        log: log,
                        subject: $"Plaid {nameof(GetPlaidTransactionsAsync)} Error",
                        fromName: $"{nameof(SyncTransactions)} Function",
                        body: $"request: {contract.Serialize()} - error: {error.Serialize()}");
                }

                throw;
            }
            catch (Exception e)
            {
                log.LogError($"{nameof(GetPlaidTransactionsAsync)} failure for {nameof(accessToken)} {accessToken}");
                log.LogError($"{e.ToString()}");

                await FunctionNotifier.SendEmailAsync(
                    emailClient: emailClient,
                    log: log,
                    subject: $"Plaid {nameof(GetPlaidTransactionsAsync)} Unknown Error",
                    fromName: $"{nameof(SyncTransactions)} Function",
                    body: $"request: {contract.Serialize()} - error: {e.Message}");

                throw;
            }
        }

        private async Task<IEnumerable<Transaction>> GetTenantTransactionsAsync(Tenant tenant)
        {
            Guid tenantId = tenant.TenantId;
            DateTime localDateTime = tenant.LocalNow;
            Date localDate = localDateTime.ToDate();
            Date startDate = localDate.AddDays(tenantStartDayOffset);
            Date endDate = localDate.AddDays(1);

            try
            {
                log.LogInformation($"Sending {nameof(GetTenantTransactionsAsync)}({tenantId}) request...");

                IEnumerable<Transaction> transactions = await transactionClient.GetAsync(tenantId, startDate, endDate);

                log.LogInformation($"{nameof(GetTenantTransactionsAsync)}({tenantId}) success!");

                return transactions;
            }
            catch (Exception e)
            {
                log.LogError($"{nameof(GetTenantTransactionsAsync)} failure for tenant {tenantId}");
                log.LogError($"{e.ToString()}");

                return null;
            }
        }

        private async Task SyncTransactionsAsync(
            Tenant tenant,
            AccessToken accessToken,
            IEnumerable<PlaidTransactionContractV1> plaidTransactions,
            IEnumerable<Transaction> tenantTransactions)
        {
            Guid tenantId = tenant.TenantId;
            Guid accessTokenId = accessToken.AccessTokenId;

            IEnumerable<Account> accounts = await accountClient.GetAsync(tenantId, accessTokenId);

            if (accounts.IsNullOrEmpty() == true)
            {
                string errorMessage = $"Tenant {tenantId} does not have any valid accounts";
                log.LogError(errorMessage);

                throw new ArgumentException(errorMessage);
            }

            IDictionary<string, Account> accountAliasMap =
                accounts
                    .ToDictionary(x => x.AliasId, x => x);
            IDictionary<string, PlaidTransactionContractV1> plaidTransactionMap =
                plaidTransactions.ToDictionary(x => x.TransactionId, x => x);
            // transactions that have been settled
            IDictionary<string, Transaction> tenantTransactionMap =
                tenantTransactions
                    .Where(x => x.GetState() != TransactionState.Pending)
                    .ToDictionary(x => x.PlaidTransactionId, x => x);
            // transactions that are in pending states
            IDictionary<string, Transaction> pendingTransactionMap =
                tenantTransactions
                    .Where(x => x.GetState() == TransactionState.Pending)
                    .ToDictionary(x => x.PlaidPendingId, x => x);

            var transactions = new List<Transaction>();

            foreach (string transactionId in plaidTransactionMap.Keys)
            {
                // no further action needed for transactions already stored in DB
                if (tenantTransactionMap.ContainsKey(transactionId) == true || 
                    pendingTransactionMap.ContainsKey(transactionId) == true) continue;
                
                PlaidTransactionContractV1 plaidTransaction = plaidTransactionMap[transactionId];
                // Plaid account isn't tracked by this access token
                if (accountAliasMap.ContainsKey(plaidTransaction.AccountId) == false) continue;

                Guid accountId = accountAliasMap[plaidTransaction.AccountId].AccountId;
                bool isPending = plaidTransaction.IsPending;
                Transaction transaction;

                if (isPending == true)
                {
                    transaction = GetPendingTransaction(plaidTransaction, accountId, tenantId);
                }
                else // settle transactions 
                {
                    string pendingTransactionId = plaidTransaction.PendingTransactionId;
                    // mergeable pair not found - settle only
                    if (String.IsNullOrEmpty(pendingTransactionId) == true ||
                        pendingTransactionMap.ContainsKey(pendingTransactionId) == false)
                    {
                        transaction = GetSettledTransaction(plaidTransaction, accountId, tenantId);
                    }
                    else // merge transactions
                    {
                        transaction = GetMergedTransaction(pendingTransactionMap, plaidTransaction);
                    }
                }

                transactions.Add(transaction);
            }

            await UpsertTransactionsAsync(transactions);
        }

        private async Task UpsertTransactionsAsync(IEnumerable<Transaction> transactions)
        {
            if (transactions.IsNullOrEmpty() == true)
            {
                log.LogInformation($"Transactions are up to date");
                return;
            }

            IList<Task<bool>> upsertTransactionTasks =
                transactions.Select(transaction => UpsertTransactionAsync(transaction)).ToList();

            IList<bool> results = await Task.WhenAll(upsertTransactionTasks);

            log.LogCritical(Diagnostics.GetMetricMessage(
                $"{nameof(UpsertTransactionAsync)} success rate: " +
                $"{results.Where(x => x == true).Count()} / {results.Count}"));
        }

        private async Task<bool> UpsertTransactionAsync(Transaction transaction)
        {
            log.LogInformation($"Updating transaction for {nameof(transaction)} {transaction.TransactionId}");

            try
            {
                await transactionClient.UpsertAsync(transaction);

                log.LogInformation($"{nameof(UpsertTransactionAsync)} success!");

                return true;
            }
            catch (Exception e)
            {
                log.LogError($"{nameof(UpsertTransactionAsync)} failure");
                log.LogError($"{e.ToString()}");

                return false;
            }
        }

        private Transaction GetPendingTransaction(
            PlaidTransactionContractV1 plaidTransaction,
            Guid accountId,
            Guid tenantId)
        {
            Transaction pendingTransaction = GetNewTransaction(plaidTransaction, accountId, tenantId);

            pendingTransaction.PlaidTransactionId = null;
            pendingTransaction.PlaidPendingId = plaidTransaction.TransactionId;

            return pendingTransaction;
        }

        private Transaction GetMergedTransaction(
            IDictionary<string, Transaction> pendingTransactionMap,
            PlaidTransactionContractV1 plaidTransaction)
        {
            string pendingTransactionId = plaidTransaction.PendingTransactionId;
            Transaction transaction = pendingTransactionMap[pendingTransactionId];

            transaction.Name = plaidTransaction.Name;
            transaction.CategoryId = plaidTransaction.CategoryId;
            transaction.PlaidTransactionId = plaidTransaction.TransactionId;
            transaction.Amount = plaidTransaction.Amount;
            transaction.MergedDate = DateTime.UtcNow;

            return transaction;
        }

        private Transaction GetSettledTransaction(
            PlaidTransactionContractV1 plaidTransaction,
            Guid accountId,
            Guid tenantId)
        {
            Transaction transaction = GetNewTransaction(plaidTransaction, accountId, tenantId);

            transaction.PlaidTransactionId = plaidTransaction.TransactionId;
            transaction.PlaidPendingId = null;

            return transaction;
        }

        private Transaction GetNewTransaction(
            PlaidTransactionContractV1 plaidTransaction,
            Guid accountId,
            Guid tenantId)
        {
            string name = plaidTransaction.Name;
            string categoryId = plaidTransaction.CategoryId;
            IList<string> categories = plaidTransaction.Categories;
            ExpenseCategory expenseCategory = ExpenseTools.GetExpenseCategory(name, categoryId, categories);
            string note = ExpenseTools.GetNote(expenseCategory, name, categoryId, categories);

            var transaction = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                Date = plaidTransaction.Date,
                TenantId = tenantId,
                AccountId = accountId,
                Name = name,
                Amount = plaidTransaction.Amount,
                CurrencyCode = plaidTransaction.IsoCurrencyCode,
                CategoryId = categoryId,
                ExpenseCategory = expenseCategory,
                Note = note,
                MergedDate = null,
                VerifiedDate = null,
            };

            return transaction;
        }
    }
}
