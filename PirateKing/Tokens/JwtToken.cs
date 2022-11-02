using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using PirateKing.Constants;
using PirateKing.Guards;
using PirateKing.Models;
using Microsoft.IdentityModel.Tokens;

namespace PirateKing.Tokens
{
    public class JwtToken
    {
        private static SymmetricSecurityKey symmetricSecurityKey;
        private static string issuer;
        private static string audience;

        /// <summary>
        /// Generates a JWT token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GenerateJwtToken(GenerateTokenRequest request)
        {
            if (symmetricSecurityKey == null || string.IsNullOrEmpty(issuer) == true || string.IsNullOrEmpty(audience) == true)
            {
                throw new ArgumentException($"Must initialize the class with {nameof(JwtToken)}.{nameof(SetJwtSecrets)}() before making any calls");
            }
            ValidateRequest(request);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(KnownClaimTypes.TenantId, request.TenantId.ToString()),
                    new Claim(KnownClaimTypes.UserId, request.UserId.ToString()),
                    new Claim(ClaimTypes.Role, request.Role),
                }),

                Expires = DateTime.UtcNow.Add(request.TimeSpan),

                SigningCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature),
            };

            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            string token = tokenHandler.WriteToken(securityToken);

            return token;
        }

        /// <summary>
        /// Generates a cookie token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GenerateCookieToken(Guid tenantId, Guid userId, TimeSpan timeSpan)
        {
            if (symmetricSecurityKey == null || string.IsNullOrEmpty(issuer) == true || string.IsNullOrEmpty(audience) == true)
            {
                throw new ArgumentException($"Must initialize the class with {nameof(JwtToken)}.{nameof(SetJwtSecrets)}() before making any calls");
            }
            Validate.NotNullOrEmpty(tenantId.ToString(), nameof(tenantId));
            Validate.NotNullOrEmpty(userId.ToString(), nameof(userId));
            if (timeSpan == default)
            {
                throw new ArgumentException($"{nameof(timeSpan)} cannot be a default value");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(KnownClaimTypes.TenantId, tenantId.ToString()),
                    new Claim(KnownClaimTypes.UserId, userId.ToString()),
                }),

                Expires = DateTime.UtcNow.Add(timeSpan),

                SigningCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature),
            };

            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            string token = tokenHandler.WriteToken(securityToken);

            return token;
        }

        /// <summary>
        /// Sets JWT token generation secret key
        /// </summary>
        /// <param name="secrets"></param>
        public static void SetJwtSecrets(JwtSecrets secrets)
        {
            Validate.NotNull(secrets, nameof(secrets));

            byte[] securityKey = Convert.FromBase64String(secrets.SecurityKey);
            JwtToken.symmetricSecurityKey = new SymmetricSecurityKey(securityKey);
            JwtToken.issuer = secrets.Issuer;
            JwtToken.audience = secrets.Audience;
        }

        /// <summary>
        /// Gets token validation parameters
        /// </summary>
        /// <returns></returns>
        public static TokenValidationParameters GetTokenValidationParameters() =>
            new TokenValidationParameters()
            {
                // what to validate
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                RequireSignedTokens = true,
                // setup validate data
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = symmetricSecurityKey,
            };

        /// <summary>
        /// Validates whether a given token is valid or not and returns the claim principal on success
        /// </summary>
        /// <param name="token"></param>
        public static bool IsValidToken(string token, out IPrincipal principal)
        {
            Validate.NotNullOrEmpty(token, nameof(token));

            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters();
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception e)
            {
                principal = null;

                return false;
            }
        }

        private static void ValidateRequest(GenerateTokenRequest request)
        {
            Validate.NotNull(request, nameof(request));
            Validate.NotNullOrEmpty(request.TenantId, nameof(request.TenantId));
            Validate.NotNullOrEmpty(request.UserId, nameof(request.UserId));
            Validate.NotNullOrEmpty(request.Role, nameof(request.Role));

            if (request.TimeSpan == default)
            {
                throw new ArgumentException($"{nameof(request.TimeSpan)} cannot be a default value");
            }
        }
    }
}
