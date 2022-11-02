using PirateKing.Guards;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace PirateKing.HttpUtils
{
    public static class HttpResponseExtension
    {
        public static void SetAuthorization(this HttpResponse response, string token)
        {
            Validate.NotNull(response, nameof(response));
            Validate.NotNullOrEmpty(token, nameof(token));

            response.Headers.Add(HeaderNames.Authorization, token);
        }

        public static void SetEtag(this HttpResponse response, string etag)
        {
            Validate.NotNull(response, nameof(response));
            Validate.NotNullOrEmpty(etag, nameof(etag));

            response.Headers.Add(HeaderNames.ETag, etag);
        }

        public static void SetLastModified(this HttpResponse response, long lastModified)
        {
            Validate.NotNull(response, nameof(response));

            response.Headers.Add(HeaderNames.LastModified, $"{lastModified}");
        }
    }
}
