using System.Net;
using PirateKing.CloudQueue;
using PirateKing.Cosmos;
using PirateKing.Guards;
using PirateKing.Plaid.V1;
using Microsoft.AspNetCore.Mvc;

namespace PirateKing.Api.Controllers.V1
{
    /// <summary>
    /// Base Controller
    /// </summary>
    public class BaseControllerV1 : ControllerBase
    {
        protected readonly IAccountClient accountClient;
        protected readonly IAccountCatalogClient accountCatalogClient;
        protected readonly ICloudQueueClient cloudQueueClient;
        protected readonly IGoogleUserClient googleUserClient;
        protected readonly IPlaidClientV1 plaidClient;
        protected readonly ITransactionClient transactionClient;
        protected readonly IUserClient userClient;

        public BaseControllerV1(IDependencyFactory factory)
        {
            Validate.NotNull(factory, nameof(factory));

            this.accountClient = factory.Get<IAccountClient>();
            this.accountCatalogClient = factory.Get<IAccountCatalogClient>();
            this.cloudQueueClient = factory.Get<ICloudQueueClient>();
            this.googleUserClient = factory.Get<IGoogleUserClient>();
            this.plaidClient = factory.Get<IPlaidClientV1>();
            this.transactionClient = factory.Get<ITransactionClient>();
            this.userClient = factory.Get<IUserClient>();
        }

        protected ObjectResult MultiStatus(object value)
        {
            return StatusCode(207, value);
        }

        protected ObjectResult NotModified(object value = null)
        {
            return StatusCode((int) HttpStatusCode.NotModified, value);
        }

        protected ObjectResult Unauthorized(object value)
        {
            return StatusCode((int) HttpStatusCode.Unauthorized, value);
        }

        protected ObjectResult InternalServerError(object value)
        {
            return StatusCode((int) HttpStatusCode.InternalServerError, value);
        }
    }
}
