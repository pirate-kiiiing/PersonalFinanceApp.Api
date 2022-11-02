using Newtonsoft.Json;

namespace PirateKing.Models
{
    public class AzureDocumentBase
    {
        public AzureDocumentBase() { }

        public AzureDocumentBase(string etag)
        {
            Etag = etag;
        }

        [JsonConstructor]
        public AzureDocumentBase(
            [JsonProperty(PropertyName = "_etag")] string etag,
            [JsonProperty(PropertyName = "_ts")] long timestamp)
        {
            Etag = etag;
            Timestamp = timestamp;
        }

        [JsonProperty(PropertyName = "_etag")]
        public string Etag { get; }

        [JsonProperty(PropertyName = "_ts")]
        public long Timestamp { get; }
    }
}
