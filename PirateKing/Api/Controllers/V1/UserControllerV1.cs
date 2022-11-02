using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PirateKing.Contracts.V1;
using PirateKing.Models;
using PirateKing.Api.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace PirateKing.Api.Controllers.V1
{
    /// <summary>
    /// Users controller for Get/Create/Update/Delete operations
    /// </summary>
    [Route("v1.0/tenants/{tenantId}/users")]
    [ApiController]
    public class UserControllerV1 : BaseControllerV1
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dependencyFactory"></param>
        public UserControllerV1(IDependencyFactory dependencyFactory) : base(dependencyFactory) { }

        /// <summary>
        /// Get users in a given tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of <see cref="GetUserResponseContractV1"/></returns>
        [HttpGet, Route("")]
        [AllowedRoles(UserRole.Admin, UserRole.User)]
        public async Task<IActionResult> GetAsync(Guid tenantId)
        {
            IReadOnlyList<User> users = await userClient.GetAsync(tenantId);

            IReadOnlyList<GetUserResponseContractV1> response =
                users.Select(user => new GetUserResponseContractV1
                {
                    Id = user.UserId,
                    TenantId = user.TenantId,
                    ProfileImageUrl = user.ProfileImageUrl,
                    UserRole = user.Role,
                }).ToList();

            return Ok(users);
        }
    }
}
