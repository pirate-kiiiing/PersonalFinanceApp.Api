using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using PirateKing.Core;
using PirateKing.Cosmos;
using PirateKing.Diagnostic;
using PirateKing.Email;
using PirateKing.Exceptions;
using PirateKing.Guards;
using PirateKing.Models;
using PirateKing.Plaid.V1;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace PirateKing.Functions.Activities
{
    [DependencyInjectionConfig(typeof(AutofacConfig))]
    public class SyncAccountCatalogs
    {
        private IAccountCatalogClient accountCatalogClient;
        private IAccountClient accountClient;
        private IPlaidClientV1 plaidClient;
        private ITenantClient tenantClient;
        private IPirateKingEmailClient emailClient;
        private ILogger log;

        [FunctionName(nameof(SyncAccountCatalogs))]
        public async Task SyncAccountCatalogsAsync(
            [ActivityTrigger] string input,
            [Inject] IAccountCatalogClient accountCatalogClient,
            [Inject] IAccountClient accountClient,
            [Inject] IPlaidClientV1 plaidClient,
            [Inject] ITenantClient tenantClient,
            [Inject] IPirateKingEmailClient emailClient,
            ILogger log)
        {
            Validate.NotNull(accountClient, nameof(accountClient));
            Validate.NotNull(accountCatalogClient, nameof(accountCatalogClient));
            Validate.NotNull(plaidClient, nameof(plaidClient));
            Validate.NotNull(tenantClient, nameof(tenantClient));
            Validate.NotNull(emailClient, nameof(emailClient));
            Validate.NotNull(log, nameof(log));
            // setup
            this.accountCatalogClient = accountCatalogClient;
            this.accountClient = accountClient;
            this.plaidClient = plaidClient;
            this.tenantClient = tenantClient;
            this.emailClient = emailClient;
            this.log = log;
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            log.LogInformation($"{nameof(SyncAccountCatalogs)} executed at: {DateTime.Now}");

            await SyncAccountCatalogsAsync();

            stopwatch.Stop();
            log.LogCritical(Diagnostics.GetMetricMessage($"Execution time: {stopwatch.ElapsedMilliseconds} ms"));
            log.LogInformation($"{nameof(SyncAccountCatalogs)} exiting at: {DateTime.Now}");
        }

        private async Task SyncAccountCatalogsAsync()
        {
            IReadOnlyList<Tenant> tenants = await tenantClient.GetAsync();
            tenants = tenants.Where(tenant => tenant.State == TenantState.Active).ToList();

            if (tenants.IsNullOrEmpty() == true)
            {
                log.LogInformation($"No active tenants found");
                return;
            }

            List<Task<bool>> syncTenantAccountTasks = tenants.Select(tenant => SyncTenantAccountsAsync(tenant)).ToList();

            IList<bool> results = await Task.WhenAll(syncTenantAccountTasks);

            log.LogCritical(Diagnostics.GetMetricMessage(
                $"{nameof(SyncTenantAccountsAsync)} success rate: " +
                $"{results.Where(x => x == true).Count()} / {results.Count}"));
        }

        private async Task<bool> SyncTenantAccountsAsync(Tenant tenant)
        {
            Guid tenantId = tenant.TenantId;

            IEnumerable<Account> accounts = await accountClient.GetAsync(tenantId);

            if (accounts.IsNullOrEmpty() == true)
            {
                log.LogInformation($"Tenant {tenantId} does not have any accounts");
                return true;
            }

            IEnumerable<Account> syncAccounts = accounts.Where(
                                                    account =>
                                                    account.State == AccountState.Active &&
                                                    account.AssetType != null);

            if (syncAccounts.IsNullOrEmpty() == true)
            {
                log.LogInformation($"Tenant {tenantId} does not have any active asset accounts");
                return true;
            }

            try
            {
                List<AccountCatalog> accountCatalogs = await GetAccountCatalogsAsync(syncAccounts, tenant);

                await UpsertAccountCatalogsAsync(accountCatalogs);

                log.LogInformation($"{nameof(SyncTenantAccountsAsync)} success");

                return true;
            }
            catch (Exception e)
            {
                log.LogError($"An error occurred while syncing tenant accounts.");
                log.LogError($"{e.ToString()}");

                return false;
            }
        }

        private async Task<List<AccountCatalog>> GetAccountCatalogsAsync(IEnumerable<Account> accounts, Tenant tenant)
        {
            IEnumerable<Account> plaidAccounts = accounts.Where(account => account.TrackingType == TrackingType.Plaid);
            IEnumerable<Account> jobAccounts = accounts.Where(account => account.TrackingType == TrackingType.Job);

            var accountCatalogs = new List<AccountCatalog>();

            var getAccountCatalogTasks = new List<Task<List<AccountCatalog>>>()
            {
                GetPlaidAccountCatalogsAsync(plaidAccounts, tenant),
                GetJobAccountCatalogsAsync(jobAccounts, tenant),
            };

            var _accountCatalogs = await Task.WhenAll(getAccountCatalogTasks);

            accountCatalogs.AddRange(_accountCatalogs.SelectMany(x => x));

            return accountCatalogs;
        }

        private async Task<List<AccountCatalog>> GetPlaidAccountCatalogsAsync(IEnumerable<Account> accounts, Tenant tenant)
        {
            var accountCatalogs = new List<AccountCatalog>();

            if (accounts.IsNullOrEmpty() == true)
            {
                log.LogInformation($"Tenant {tenant.TenantId} does not have any plaid accounts");

                return accountCatalogs;
            }

            Dictionary<string, Guid> aliasAccountIdMap = accounts.ToDictionary(x => x.AliasId, x => x.AccountId);
            IEnumerable<string> accessTokens = 
                accounts
                    .Select(account => account.AccessToken)
                    .Distinct();

            IEnumerable<GetAccountBalanceRequestContractV1> requests =
                accessTokens.Select(
                    accessToken =>
                    new GetAccountBalanceRequestContractV1
                    {
                        AccessToken = accessToken
                    });

            // make sequential calls as Plaid blocks too many requests attempted in a short interval
            var responses = new List<GetAccountBalanceResponseContractV1>();
            foreach (GetAccountBalanceRequestContractV1 request in requests)
            {
                GetAccountBalanceResponseContractV1 response = await GetAccountBalanceAsync(request);
                
                if (response == null) continue;

                responses.Add(response);
            }

            foreach (GetAccountBalanceResponseContractV1 response in responses)
            {
                if (response == null || response.Accounts.IsNullOrEmpty() == true) continue;

                DateTime date = tenant.LocalNow;
                Dictionary<string, decimal> accountValueMap =
                    // Not all institutions provide available balance. Available balance includes pending amounts. 
                    response.Accounts.ToDictionary(x => x.AccountId, x => x.Balance.Available.HasValue ? x.Balance.Available.Value : x.Balance.Current);
                IEnumerable<AccountCatalog> _accountCatalogs =
                    accountValueMap.Keys
                        .Where(aliasId => aliasAccountIdMap.ContainsKey(aliasId) == true)
                        .Select(aliasId =>
                        {
                            Guid accountId = aliasAccountIdMap[aliasId];

                            return new AccountCatalog
                            {
                                AccountCatalogId = new GuidDate(accountId, date),
                                TenantId = tenant.TenantId,
                                Value = accountValueMap[aliasId],
                            };
                        });

                accountCatalogs.AddRange(_accountCatalogs);
            }

            log.LogCritical(Diagnostics.GetMetricMessage(
                $"{nameof(GetAccountBalanceAsync)} success rate: " +
                $"{responses.Where(x => x != null).Count()} / {responses.Count}"));

            return accountCatalogs;
        }

        private async Task<GetAccountBalanceResponseContractV1> GetAccountBalanceAsync(GetAccountBalanceRequestContractV1 request)
        {
            log.LogInformation($"Making {nameof(GetAccountBalanceAsync)} request to Plaid...");

            try
            {
                GetAccountBalanceResponseContractV1 response = await plaidClient.GetAccountBalanceAsync(request);

                log.LogInformation($"{nameof(GetAccountBalanceAsync)} for accessToken {request.AccessToken} success!");

                return response;
            }
            catch (HttpException e) when (e.Content is PlaidErrorResponseContractV1 error)
            {
                log.LogError($"Plaid {nameof(GetAccountBalanceAsync)} error: {request.Serialize()}");
                log.LogError($"{e.Message}");

                if (error?.ErrorType != PlaidErrorTypeContractV1.INSTITUTION_ERROR.ToString() ||
                    error?.SuggestedAction.IsNullOrEmpty() == false)
                {
                    await FunctionNotifier.SendEmailAsync(
                    emailClient: emailClient,
                    log: log,
                    subject: $"Plaid {nameof(GetAccountBalanceAsync)} Error",
                    fromName: $"{nameof(SyncAccountCatalogs)} Function",
                    body: $"request: {request.Serialize()} - error: {error.Serialize()}");
                }

                return null;
            }
            catch (Exception e)
            {
                log.LogError($"Unknown {nameof(GetAccountBalanceAsync)} error: {request.Serialize()}");
                log.LogError($"{e.Message}");

                await FunctionNotifier.SendEmailAsync(
                    emailClient: emailClient,
                    log: log,
                    subject: $"Plaid {nameof(GetAccountBalanceAsync)} Unknown Error",
                    fromName: $"{nameof(SyncAccountCatalogs)} Function",
                    body: $"request: {request.Serialize()} - error: {e.Message}");

                return null;
            }
        }

        private async Task<List<AccountCatalog>> GetJobAccountCatalogsAsync(IEnumerable<Account> accounts, Tenant tenant)
        {
            var accountCatalogs = new List<AccountCatalog>();

            if (accounts.IsNullOrEmpty() == true)
            {
                log.LogInformation($"Tenant {tenant.TenantId} does not have any accounts tracked by jobs");

                return accountCatalogs;
            }

            DateTime today = tenant.LocalNow;
            // only add new catalogs if they don't exist on the given date
            // for the existing ones, leave them as-is
            Dictionary<Guid, Task<AccountCatalog>> getAccountCatalogTaskMap =
                accounts
                    .Select(account => new KeyValuePair<Guid, Task<AccountCatalog>>(
                        account.AccountId, 
                        accountCatalogClient.GetAsync(tenant.TenantId, new GuidDate(account.AccountId, today))))
                    .ToDictionary(x => x.Key, x => x.Value);

            await Task.WhenAll(getAccountCatalogTaskMap.Values);

            DateTime previousDay = today.AddDays(-1);
            // get yesterday's catalog value if today's catalogs do not exist
            Dictionary<Guid, Task<AccountCatalog>> getPreviousAccountCatalogTaskMap =
                getAccountCatalogTaskMap
                    .Keys
                    .Where(accountId => getAccountCatalogTaskMap[accountId].Result == null)
                    .Select(accountId => new KeyValuePair<Guid, Task<AccountCatalog>>(
                        accountId,
                        accountCatalogClient.GetAsync(tenant.TenantId, new GuidDate(accountId, previousDay))))
                    .ToDictionary(x => x.Key, x => x.Value);

            foreach (Guid accountId in getAccountCatalogTaskMap.Keys)
            {
                AccountCatalog accountCatalog = await getAccountCatalogTaskMap[accountId];
                
                if (accountCatalog == null) continue;

                accountCatalogs.Add(new AccountCatalog
                {
                    AccountCatalogId = new GuidDate(accountId, today),
                    TenantId = tenant.TenantId,
                    Value = accountCatalog.Value,
                });
            }

            if (getPreviousAccountCatalogTaskMap.Values.Count <= 0)
            {
                return accountCatalogs;
            }

            await Task.WhenAll(getPreviousAccountCatalogTaskMap.Values);

            foreach (Guid accountId in getPreviousAccountCatalogTaskMap.Keys)
            {
                AccountCatalog previousAccountCatalog = await getPreviousAccountCatalogTaskMap[accountId];

                var accountCatalog = new AccountCatalog
                {
                    AccountCatalogId = new GuidDate(accountId, today),
                    TenantId = tenant.TenantId,
                    Value = previousAccountCatalog.Value,
                };

                accountCatalogs.Add(accountCatalog);
            }

            return accountCatalogs;
        }

        private async Task UpsertAccountCatalogsAsync(IEnumerable<AccountCatalog> accountCatalogs)
        {
            if (accountCatalogs.IsNullOrEmpty() == true) return;

            log.LogInformation($"Storing account catalogs: {accountCatalogs.Serialize()}");

            List<Task<bool>> upsertAccountCatalogTasks =
                accountCatalogs
                    .Select(
                        accountCatalog =>
                        UpsertAccountCatalogAsync(accountCatalog))
                    .ToList();

            IList<bool> results = await Task.WhenAll(upsertAccountCatalogTasks);

            log.LogCritical(Diagnostics.GetMetricMessage(
                $"{nameof(UpsertAccountCatalogAsync)} success rate: " +
                $"{results.Where(x => x == true).Count()} / {results.Count}"));
        }

        private async Task<bool> UpsertAccountCatalogAsync(AccountCatalog accountCatalog)
        {
            log.LogInformation($"Storing account catalog: {accountCatalog.Serialize()}");

            try
            {
                await accountCatalogClient.UpsertAsync(accountCatalog);

                log.LogInformation($"{nameof(UpsertAccountCatalogAsync)} success");

                return true;
            }
            catch (Exception e)
            {
                log.LogError($"{nameof(UpsertAccountCatalogAsync)} failure");
                log.LogError($"{e.ToString()}");

                return false;
            }
        }
    }
}   
