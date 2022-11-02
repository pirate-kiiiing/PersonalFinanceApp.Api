using System;
using System.Threading.Tasks;
using PirateKing.Guards;
using PirateKing.Models;

namespace PirateKing.Cosmos
{
    /// <summary>
    /// Google User Client
    /// </summary>
    public class GoogleUserClient : IGoogleUserClient
    {
        private readonly ICosmosClient cosmosClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cosmosClient"></param>
        public GoogleUserClient(ICosmosClient cosmosClient)
        {
            Validate.NotNull(cosmosClient, nameof(cosmosClient));

            this.cosmosClient = cosmosClient;
        }

        /// <summary>
        /// Get Google User with the specified googleUserId
        /// </summary>
        /// <param name="googleUserId"></param>
        /// <returns><see cref="GoogleUser"/></returns>
        public async Task<GoogleUser> GetAsync(string googleUserId)
        {
            Validate.NotNullOrEmpty(googleUserId, nameof(googleUserId));

            GoogleUser googleUser =
                await cosmosClient.ReadDocumentAsync<GoogleUser>(
                    CosmosConstants.DatabaseName,
                    CollectionNames.GoogleUsers,
                    googleUserId,
                    googleUserId);

            return googleUser;
        }
    }

    public interface IGoogleUserClient
    {
        /// <summary>
        /// Get Google User with the specified googleUserId
        /// </summary>
        /// <param name="googleUserId"></param>
        /// <returns><see cref="GoogleUser"/></returns>
        Task<GoogleUser> GetAsync(string googleUserId);
    }
}
