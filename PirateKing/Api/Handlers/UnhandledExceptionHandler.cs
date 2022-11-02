using System;
using System.Net;
using System.Threading.Tasks;
using PirateKing.Core;
using PirateKing.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;

namespace PirateKing.Api.Handlers
{
    internal sealed class UnhandledExceptionHandler
    {
        private const string unknownErrorMessage = "An unknown error has occurred.";

        private readonly RequestDelegate next;

        public UnhandledExceptionHandler(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            string result = (exception.Message.IsNullOrEmpty() == true) ? unknownErrorMessage : exception.Message;
            
            switch (exception)
            {
                case HttpException e:
                    statusCode = e.StatusCode;
                    result = (e.Content == null) ? e.Message : e.Content.Serialize();
                    break;
            }

            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(result);
        }
    }
}
