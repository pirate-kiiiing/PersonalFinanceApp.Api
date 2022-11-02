using System;
using System.Collections.Generic;
using PirateKing.CloudQueue;
using PirateKing.Cosmos;
using PirateKing.Guards;
using PirateKing.Plaid.V1;

namespace PirateKing.Api
{
    internal sealed class DependencyFactory : IDependencyFactory
    {
        private Dictionary<Type, object> dependencyMap;

        public DependencyFactory(
            IAccountClient accountClient,
            IAccountCatalogClient accountCatalogClient,
            ICloudQueueClient cloudQueueClient,
            IGoogleUserClient googleUserClient,
            IPlaidClientV1 plaidClient,
            ITenantClient tenantClient,
            ITransactionClient transactionClient,
            IUserClient userClient)
        {
            Validate.NotNull(accountClient, nameof(accountClient));
            Validate.NotNull(accountCatalogClient, nameof(accountCatalogClient));
            Validate.NotNull(cloudQueueClient, nameof(cloudQueueClient));
            Validate.NotNull(googleUserClient, nameof(googleUserClient));
            Validate.NotNull(plaidClient, nameof(plaidClient));
            Validate.NotNull(tenantClient, nameof(tenantClient));
            Validate.NotNull(transactionClient, nameof(transactionClient));
            Validate.NotNull(userClient, nameof(userClient));

            dependencyMap = new Dictionary<Type, object>();
            
            dependencyMap[typeof(IAccountClient)] = accountClient;
            dependencyMap[typeof(IAccountCatalogClient)] = accountCatalogClient;
            dependencyMap[typeof(ICloudQueueClient)] = cloudQueueClient;
            dependencyMap[typeof(IGoogleUserClient)] = googleUserClient;
            dependencyMap[typeof(IPlaidClientV1)] = plaidClient;
            dependencyMap[typeof(ITenantClient)] = tenantClient;
            dependencyMap[typeof(ITransactionClient)] = transactionClient;
            dependencyMap[typeof(IUserClient)] = userClient;
        }

        public T Get<T>()
        {
            Validate.ContainsKey(dependencyMap, typeof(T), nameof(dependencyMap), typeof(T).FullName);

            return (T) dependencyMap[typeof(T)];
        }
    }

    public interface IDependencyFactory
    {
        T Get<T>();
    }
}
