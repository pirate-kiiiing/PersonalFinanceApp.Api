using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PirateKing.Contracts.V1;
using PirateKing.Core;
using PirateKing.Guards;
using PirateKing.HttpUtils;
using PirateKing.Models;
using PirateKing.Api.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace PirateKing.Api.Controllers.V1
{
    /// <summary>
    /// Account catalogs controller for Get/Create/Update/Delete operations
    /// </summary>
    [Route("v1.0/tenants/{tenantId}")]
    [ApiController]
    public class AccountCatalogControllerV1 : BaseControllerV1
    {
        public AccountCatalogControllerV1(IDependencyFactory dependencyFactory) : base(dependencyFactory) { }

        /// <summary>
        /// Get all account catalogs of a tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of <see cref="GetAccountCatalogResponseContractV1"/></returns>
        [HttpGet, Route("account-catalogs")]
        [AllowedRoles(UserRole.Admin, UserRole.User)]
        public async Task<IActionResult> GetAsync(Guid tenantId, Date startDate, Date endDate)
        {
            IReadOnlyList<AccountCatalog> catlaogs = await accountCatalogClient.GetAsync(tenantId, startDate, endDate);

            if (catlaogs.IsNullOrEmpty() == true)
            {
                return Ok(Enumerable.Empty<GetAccountCatalogResponseContractV1>());
            }

            IReadOnlyList<GetAccountCatalogResponseContractV1> response =
                catlaogs
                    .Select(c => new GetAccountCatalogResponseContractV1
                    {
                        AccountId = c.AccountCatalogId.Guid,
                        Date = c.AccountCatalogId.Date,
                        TenantId = c.TenantId,
                        Timestamp = c.Timestamp,
                        Value = c.Value,
                    })
                    .ToList();

            return Ok(response);
        }

        /// <summary>
        /// Update account catalog of a tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of <see cref="Account"/></returns>
        [HttpPut, Route("accounts/{accountId}/catalogs/{date}")]
        [AllowedRoles(UserRole.Admin)]
        public async Task<IActionResult> UpdateAsync(
            Guid tenantId,
            Guid accountId,
            Date date,
            PutAccountCatalogRequestContractV1 catalog)
        {
            HttpValidate.NotNull(catalog, nameof(catalog));
            HttpValidate.NotEmpty(catalog.AccountId, nameof(catalog.AccountId));
            HttpValidate.ValidDate(catalog.Date);
            HttpValidate.AreEqual(accountId, catalog.AccountId, $"{nameof(accountId)}");
            HttpValidate.AreEqual(date, catalog.Date);

            AccountCatalog accountCatalog;

            // request contains if-match header
            if (Request.HasIfMatch() == true)
            {
                Request.ValidateIfMatch(out string etag);

                accountCatalog = GetAccountCatalog(catalog, tenantId, etag);
                accountCatalog = await accountCatalogClient.UpdateAsync(accountCatalog);
            }
            else
            {
                accountCatalog = GetAccountCatalog(catalog, tenantId);
                accountCatalog = await accountCatalogClient.ForceUpdateAsync(accountCatalog);
            }

            var response = new PutAccountCatalogResponseContractV1
            {
                AccountId = accountCatalog.AccountCatalogId.Guid,
                Date = accountCatalog.AccountCatalogId.Date,
                Value = accountCatalog.Value,
            };

            Response.SetEtag(accountCatalog.Etag);
            Response.SetLastModified(accountCatalog.Timestamp);

            return Ok(response);
        }

        private AccountCatalog GetAccountCatalog(PutAccountCatalogRequestContractV1 catalog, Guid tenantId, string etag = null)
        {
            var accountCatalog = (string.IsNullOrEmpty(etag) == true) ? new AccountCatalog() : new AccountCatalog(etag);

            accountCatalog.AccountCatalogId = new GuidDate(catalog.AccountId, catalog.Date);
            accountCatalog.TenantId = tenantId;
            accountCatalog.Value = catalog.Value;

            return accountCatalog;
        }
    }
}
