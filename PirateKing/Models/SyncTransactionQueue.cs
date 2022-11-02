using System;
using Newtonsoft.Json;

namespace PirateKing.Models
{
    /// <summary>
    /// Queue schema for SyncTransaction Function
    /// </summary>
    public class SyncTransactionQueue
    {
        [JsonProperty(PropertyName = "tenantId")]
        public Guid TenantId { get; set; }

        [JsonProperty(PropertyName = "itemId")]
        public string ItemId { get; set; }
    }
}
