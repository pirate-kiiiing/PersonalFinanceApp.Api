using System;
using System.Linq;
using System.Net.Http.Headers;
using PirateKing.Core;
using PirateKing.Exceptions;
using PirateKing.Guards;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace PirateKing.HttpUtils
{
    public static class HttpRequestExtension
    {
        public static string GetEtag(this HttpRequest request)
        {
            request.Validate();

            return request.Headers.ContainsKey(HeaderNames.ETag)
                ? request.Headers[HeaderNames.ETag].FirstOrDefault()
                : null;
        }

        public static string GetIfMatch(this HttpRequest request)
        {
            return (request.HasIfMatch() == true)
                ? request.Headers[HeaderNames.IfMatch].FirstOrDefault()
                : null;
        }

        public static bool HasIfMatch(this HttpRequest request)
        {
            request.Validate();

            return request.Headers.ContainsKey(HeaderNames.IfMatch);
        }

        public static void ValidateEtag(this HttpRequest request)
        {
            HttpValidate.NotNullOrEmpty(GetEtag(request), HeaderNames.ETag);
        }

        public static void ValidateEtag(this HttpRequest request, out string etag)
        {
            ValidateEtag(request);

            etag = GetEtag(request);
        }

        public static void ValidateIfMatch(this HttpRequest request, out string value)
        {
            request.Validate();

            value = request.GetIfMatch();

            HttpValidate.NotNullOrEmpty(value, nameof(value));
        }

        public static string GetAuthorization(this HttpRequest request)
        {
            HttpValidate.NotNull(request, nameof(request));
            HttpValidate.NotNull(request.Headers, nameof(request.Headers));

            return request.Headers.ContainsKey(HeaderNames.Authorization)
                ? request.Headers[HeaderNames.Authorization].FirstOrDefault()
                : "";
        }

        public static string GetBearerToken(this HttpRequest request)
        {
            if (HasBearerToken(request) == false)
            {
                return null;
            }

            string authorization = GetAuthorization(request);
            if (AuthenticationHeaderValue.TryParse(authorization, out AuthenticationHeaderValue authenticationHeader) == false)
            {
                throw HttpExceptions.UnauthorizedException("Unable to read the token");
            }
            if (StringComparer.OrdinalIgnoreCase.Equals(JwtBearerDefaults.AuthenticationScheme, authenticationHeader.Scheme) == false)
            {
                throw HttpExceptions.UnauthorizedException($"Invalid scheme {authenticationHeader.Scheme}");
            }

            return authenticationHeader.Parameter;
        }

        public static bool HasBearerToken(this HttpRequest request)
        {
            if (request == null || request.Headers == null || request.Headers.Count <= 0)
            {
                return false;
            }

            string authorization = GetAuthorization(request);
            if (AuthenticationHeaderValue.TryParse(authorization, out AuthenticationHeaderValue authenticationHeader) == false)
            {
                return false;
            }
            if (StringComparer.OrdinalIgnoreCase.Equals(JwtBearerDefaults.AuthenticationScheme, authenticationHeader.Scheme) == false)
            {
                return false;
            }

            return authenticationHeader.Parameter.IsNullOrEmpty() == false;
        }

        private static void Validate(this HttpRequest request)
        {
            HttpValidate.NotNull(request, nameof(request));
            HttpValidate.NotNull(request.Headers, nameof(request.Headers));
        }
    }
}
