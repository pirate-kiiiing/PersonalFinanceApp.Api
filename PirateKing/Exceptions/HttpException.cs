using System;
using System.Net;

namespace PirateKing.Exceptions
{
    public class HttpException : HttpExceptionBase
    {
        public HttpException(HttpStatusCode statusCode, string message, Exception innerException = null)
            : base(statusCode, message, innerException)
        {
        }
    }
}
