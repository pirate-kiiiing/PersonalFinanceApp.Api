using System;
using System.Collections.Generic;
using PirateKing.Core;
using PirateKing.Models;
using Newtonsoft.Json;

namespace PirateKing.Contracts.V1
{
    /// <summary>
    /// Response contract for get assets
    /// </summary>
    public class GetAssetsResponseContractV1
    {
        [JsonProperty(PropertyName = "assets")]
        public IReadOnlyList<GetAssetResponseContractV1> Assets { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public Guid TenantId { get; set; }
    }

    public class GetAssetResponseContractV1
    {
        [JsonProperty(PropertyName = "accounts")]
        public IReadOnlyList<GetAssetAccountResponseContractV1> Accounts { get; set; }

        [JsonProperty(PropertyName = "assetType")]
        public AssetType AssetType { get; set; }
    }

    public class GetAssetAccountResponseContractV1
    {
        [JsonProperty(PropertyName = "accountCatalogs")]
        public IReadOnlyList<GetAssetAccountCatalogResponseContractV1> AccountCatalogs { get; set; }

        [JsonProperty(PropertyName = "accountId")]
        public Guid AccountId { get; set; }

        [JsonProperty(PropertyName = "accountName")]
        public string AccountName { get; set; }

        [JsonProperty(PropertyName = "accountSymbol")]
        public string AccountSymbol { get; set; }

        [JsonProperty(PropertyName = "isTracked")]
        public bool IsTracked { get; set; }
    }

    public class GetAssetAccountCatalogResponseContractV1
    {
        [JsonProperty(PropertyName = "date")]
        public Date Date { get; set; }

        [JsonProperty(PropertyName = "etag")]
        public string Etag { get; set; }

        [JsonProperty(PropertyName = "lastModified")]
        public long LastModified { get; set; }

        [JsonProperty(PropertyName = "value")]
        public decimal Value { get; set; }
    }
}
