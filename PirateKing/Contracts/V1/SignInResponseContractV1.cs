using System;
using PirateKing.Models;
using Newtonsoft.Json;

namespace PirateKing.Contracts.V1
{
    public class SignInResponseContractV1
    {
        /// <summary>
        /// Tenant Id
        /// </summary>
        [JsonProperty(PropertyName = "tenantId", Required = Required.Always)]
        public Guid TenantId { get; set; }

        /// <summary>
        /// User Id
        /// </summary>
        [JsonProperty(PropertyName = "userId", Required = Required.Always)]
        public Guid UserId { get; set; }

        /// <summary>
        /// Profile Image Url
        /// </summary>
        [JsonProperty(PropertyName = "userProfileImageUrl", Required = Required.Always)]
        public string UserProfileImageUrl { get; set; }

        /// <summary>
        /// User Role
        /// </summary>
        [JsonProperty(PropertyName = "userRole", Required = Required.Always)]
        public UserRole UserRole { get; set; }

        /// <summary>
        /// Access Token
        /// </summary>
        [JsonProperty(PropertyName = "accessToken", Required = Required.Always)]
        public string AccessToken { get; set; }
    }
}
