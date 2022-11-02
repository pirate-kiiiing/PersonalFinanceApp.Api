using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PirateKing.Contracts.V1;
using PirateKing.Core;
using PirateKing.Models;
using PirateKing.Api.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace PirateKing.Api.Controllers.V1
{
    /// <summary>
    /// Accounts controller for Get/Create/Update/Delete operations
    /// </summary>
    [Route("v1.0/tenants/{tenantId}/accounts")]
    [ApiController]
    public class AccountControllerV1 : BaseControllerV1
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dependencyFactory"></param>
        public AccountControllerV1(IDependencyFactory dependencyFactory) : base(dependencyFactory) { }

        /// <summary>
        /// Get all accounts of a tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of <see cref="GetAccountResponseContractV1"/></returns>
        [HttpGet, Route("")]
        [AllowedRoles(UserRole.Admin, UserRole.User)]
        public async Task<IActionResult> GetAsync(Guid tenantId, bool? isAsset, bool? isExpense)
        {
            IReadOnlyList<Account> accounts = await accountClient.GetAsync(tenantId);

            if (accounts.IsNullOrEmpty() == true)
            {
                return Ok(Enumerable.Empty<GetAccountResponseContractV1>());
            }

            IReadOnlyList<GetAccountResponseContractV1> response =
                accounts
                    .Select(a => new GetAccountResponseContractV1
                    {
                        AssetType = a.AssetType,
                        ExpenseType = a.ExpenseType,
                        Id = a.AccountId,
                        IsTracked = a.TrackingType.HasValue,
                        Name = a.Name,
                        State = a.State,
                        Symbol = a.Symbol,
                        TenantId = a.TenantId,
                        UserId = a.UserId,
                    })
                    .ToList();

            if (isAsset.HasValue && isAsset.Value == true)
            {
                response = response
                            .Where(x => x.AssetType != null)
                            .ToList();
            }
            if (isExpense.HasValue && isExpense.Value == true)
            {
                response = response
                            .Where(x => x.ExpenseType != null)
                            .ToList();
            }

            return Ok(response);
        }
    }
}
