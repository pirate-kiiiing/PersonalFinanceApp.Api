using System;
using PirateKing.Models;
using Newtonsoft.Json;

namespace PirateKing.Contracts.V1
{
    public class GetUserResponseContractV1
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "tenantId")]
        public Guid TenantId { get; set; }

        [JsonProperty(PropertyName = "profileImageUrl")]
        public string ProfileImageUrl { get; set; }

        [JsonProperty(PropertyName = "role")]
        public UserRole UserRole { get; set; }
    }
}
