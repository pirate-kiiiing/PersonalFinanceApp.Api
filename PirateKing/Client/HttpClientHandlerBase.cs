using System.Net.Http;

namespace PirateKing.Client
{
    public class HttpClientHandlerBase : DelegatingHandler
    {
        protected const string applicationJson = "application/json";
    }
}
