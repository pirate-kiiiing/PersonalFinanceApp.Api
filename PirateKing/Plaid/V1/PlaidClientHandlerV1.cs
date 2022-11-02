using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PirateKing.Client;
using PirateKing.Configurations;
using PirateKing.Core;
using PirateKing.Guards;

namespace PirateKing.Plaid.V1
{
    public class PlaidClientHandlerV1 : HttpClientHandlerBase
    {
        private PlaidConfiguration configuration;

        public PlaidClientHandlerV1(PlaidConfiguration configuration)
        {
            Validate.NotNull(configuration, nameof(configuration));

            this.configuration = configuration;

            InnerHandler = new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var content = await request.Content.ReadAsAsync<IDictionary<string, string>>();
            content["client_id"] = configuration.ClientId;
            content["secret"] = configuration.Secret;
            content["access_token"] = $"{configuration.AccessTokenPrefix}{content["access_token"]}";

            string body = content.Serialize();

            request.Content = new StringContent(body, Encoding.UTF8, applicationJson);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
