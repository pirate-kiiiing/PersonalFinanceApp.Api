using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PirateKing.Configurations;
using PirateKing.Core;
using PirateKing.Guards;
using PirateKing.Models;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Rest.Azure;
using static Microsoft.Azure.KeyVault.KeyVaultClient;

namespace PirateKing.KeyVault
{
    public class KeyVaultClient
    {
        private const int maxResults = 25;

        private static ClientConfiguration configuration;
        private static Microsoft.Azure.KeyVault.KeyVaultClient keyVaultClient;

        /// <summary>
        /// Initializes the static KeyVaultClient class. 
        /// Must be called before any other interfaces are called.
        /// </summary>
        /// <param name="configuration"></param>
        public static void Init(ClientConfiguration configuration)
        { 
            Validate.NotNull(configuration, nameof(configuration));

            AzureServiceTokenProvider tokenProvider = new AzureServiceTokenProvider();
            AuthenticationCallback authenticationCallback = new AuthenticationCallback(tokenProvider.KeyVaultTokenCallback);

            KeyVaultClient.configuration = configuration;
            KeyVaultClient.keyVaultClient = new Microsoft.Azure.KeyVault.KeyVaultClient(authenticationCallback);
        }

        public static async Task<string> GetSecretAsync(string secretName)
        {
            ValidateConfig();
            Validate.NotNullOrEmpty(secretName, nameof(secretName));

            SecretBundle secret = await keyVaultClient.GetSecretAsync(configuration.BaseAddress, secretName);

            return secret.Value;
        }

        public static async Task<EmailSecrets> GetEmailSecretsAsync()
        {
            ValidateConfig();

            SecretBundle secret = await keyVaultClient.GetSecretAsync(configuration.BaseAddress, SecretNames.EmailSecrets);
            string secretValue = secret.Value;

            return secretValue.Deserialize<EmailSecrets>();
        }

        private static async Task<IEnumerable<SecretItem>> GetAllSecretItemsAsync()
        {
            ValidateConfig();

            IPage<SecretItem> pagedResult = await keyVaultClient.GetSecretsAsync(configuration.BaseAddress, maxResults);

            var secretItems = new List<SecretItem>(pagedResult.Select(x => x));

            while (String.IsNullOrEmpty(pagedResult.NextPageLink) == false)
            {
                pagedResult = await keyVaultClient.GetSecretsNextAsync(pagedResult.NextPageLink);
                secretItems.AddRange(pagedResult.Select(x => x));
            }

            return secretItems;
        }

        private static void ValidateConfig()
        {
            if (KeyVaultClient.configuration == null)
            {
                throw new ArgumentNullException($"Must first initialize the class by calling {nameof(KeyVaultClient)}.{nameof(Init)}()");
            }
        }
    }
}
