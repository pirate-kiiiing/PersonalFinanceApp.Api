using System.Net.Http;
using System.Threading.Tasks;
using PirateKing.Client;
using PirateKing.Configurations;
using PirateKing.Core;
using PirateKing.Exceptions;
using PirateKing.Guards;

namespace PirateKing.Plaid.V1
{
    /// <summary>
    /// Plaid Client
    /// </summary>
    public class PlaidClientV1 : HttpClientBase, IPlaidClientV1
    {
        private readonly PlaidConfiguration configuration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client">HttpClient</param>
        public PlaidClientV1(HttpClient client, PlaidConfiguration configuration) : base(client)
        {
            Validate.NotNull(configuration, nameof(configuration));

            this.configuration = configuration;
        }

        /// <summary>
        /// Get Plaid Account Balance
        /// </summary>
        /// <param name="contract"></param>
        /// <returns><see cref="GetAccountBalanceResponseContractV1"/></returns>
        public async Task<GetAccountBalanceResponseContractV1> GetAccountBalanceAsync(GetAccountBalanceRequestContractV1 contract)
        {
            Validate.NotNull(contract, nameof(contract));
            Validate.NotNull(contract.AccessToken, nameof(contract.AccessToken));

            string path = "/accounts/balance/get";

            var request = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = new ObjectContent<GetAccountBalanceRequestContractV1>(contract, mediaTypeFormatter),
            };

            HttpResponseMessage response = await client.SendAsync(request);

            return await GetPlaidResponse<GetAccountBalanceResponseContractV1>(response);
        }

        /// <summary>
        /// Get Plaid Transactions
        /// </summary>
        /// <param name="contract"></param>
        /// <returns><see cref="GetTransactionsResponseContractV1"/></returns>
        public async Task<GetTransactionsResponseContractV1> GetTransactionsAsync(GetTransactionsRequestContractV1 contract)
        {
            Validate.NotNull(contract, nameof(contract));
            Validate.NotNull(contract.AccessToken, nameof(contract.AccessToken));
            Validate.ValidRange(contract.StartDate, contract.EndDate);

            string path = "/transactions/get";

            var request = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = new ObjectContent<GetTransactionsRequestContractV1>(contract, mediaTypeFormatter),
            };

            HttpResponseMessage response = await client.SendAsync(request);

            return await GetPlaidResponse<GetTransactionsResponseContractV1>(response);
        }

        private async Task<T> GetPlaidResponse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode == false)
            {
                var errorResponse = await response.ReadContentAsAsync<PlaidErrorResponseContractV1>();

                throw new HttpException(response.StatusCode, errorResponse.ErrorMessage)
                {
                    Content = errorResponse,
                };
            }

            return await response.ReadContentAsAsync<T>();
        }
    }
}
