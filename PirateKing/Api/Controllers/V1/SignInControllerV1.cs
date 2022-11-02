using System;
using System.Security.Principal;
using System.Threading.Tasks;
using PirateKing.Contracts.V1;
using PirateKing.Core;
using PirateKing.HttpUtils;
using PirateKing.Models;
using PirateKing.Tokens;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace PirateKing.Api.Controllers.V1
{
    /// <summary>
    /// Sign-in controller for Get/Create/Update/Delete operations
    /// </summary>
    [Route("v1.0")]
    [ApiController]
    [AllowAnonymous]
    public class SignInControllerV1 : BaseControllerV1
    {
        private const string cookieKey = "SID";

        public SignInControllerV1(IDependencyFactory dependencyFactory) : base(dependencyFactory) { }

        /// <summary>
        /// Signs in a user
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of <see cref="SignInResponseContractV1"/></returns>
        [HttpPost, Route("signin")]
        public async Task<IActionResult> SignInAsync()
        {
            // GoogleToken
            if (Request.HasBearerToken() == true)
            {
                string accessToken = Request.GetBearerToken();

                return await SignInGoogleTokenAsync(accessToken);
            }
            else if (Request.Cookies.ContainsKey(cookieKey) == true)
            {
                string cookieToken = Request.Cookies[cookieKey];

                if (JwtToken.IsValidToken(cookieToken, out IPrincipal principal) == false)
                {
                    return Unauthorized("Invalid cookie");
                }

                // is PirateKing token
                return await SignInUserAsync(principal);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Signs out a user
        /// </summary>
        /// <param name="tenantId"></param>
        [HttpPost, Route("signout")]
        public IActionResult SignOut()
        {
            Response.Cookies.Append(cookieKey, "", new CookieOptions
            {
                Path = "/",
                SameSite = SameSiteMode.None,
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(-1),
            });

            return NoContent();
        }

        private async Task<IActionResult> SignInUserAsync(IPrincipal principal)
        {
            if (principal.TryGetTenantId(out Guid tenantId) == false)
            {
                return Unauthorized("Invalid token");
            }
            if (principal.TryGetUserId(out Guid userId) == false)
            {
                return Unauthorized("Invalid token");
            }

            return await SignInAsync(tenantId, userId);
        }

        private async Task<IActionResult> SignInGoogleTokenAsync(string googleToken)
        {
            Payload payload;

            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(googleToken);
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }

            string googleUserId = payload.Subject;
            GoogleUser googleUser = await googleUserClient.GetAsync(googleUserId);

            if (googleUser == null)
            {
                return Unauthorized("Invalid user");
            }

            return await SignInAsync(googleUser.TenantId, googleUser.UserId);
        }

        private async Task<IActionResult> SignInAsync(
            Guid tenantId, 
            Guid userId)
        {
            User user = await userClient.GetAsync(tenantId, userId);

            if (user == null)
            {
                return Unauthorized("Invalid user");
            }

            string accessToken = JwtToken.GenerateJwtToken(new GenerateTokenRequest
            {
                Role = user.Role.ToString(),
                TenantId = user.TenantId.ToString(),
                UserId = user.UserId.ToString(),
                TimeSpan = TokenConstants.DefaultLifeSpan,
            });

            TimeSpan cookieTimeSpan = TimeSpan.FromDays(7);
            string cookieToken = JwtToken.GenerateCookieToken(tenantId, userId, cookieTimeSpan);

            Response.Cookies.Append(cookieKey, cookieToken, new CookieOptions
            {
                Path = "/",
                SameSite = SameSiteMode.None,
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.Add(cookieTimeSpan),
            });

            return Ok(new SignInResponseContractV1
            {
                TenantId = user.TenantId,
                UserId = user.UserId,
                UserProfileImageUrl = user.ProfileImageUrl,
                UserRole = user.Role,
                AccessToken = accessToken,
            });
        }
    }
}
