using System.Net.Http;
using System.Net.Http.Formatting;
using PirateKing.Guards;

namespace PirateKing.Client
{
    public class HttpClientBase
    {
        protected readonly HttpClient client;
        protected readonly MediaTypeFormatter mediaTypeFormatter;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client">HttpClient</param>
        public HttpClientBase(HttpClient client)
        {
            Validate.NotNull(client, nameof(client));

            this.client = client;
            this.mediaTypeFormatter = new JsonMediaTypeFormatter();
        }
    }
}
