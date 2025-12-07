using System.Net;
using System.Text.Json;
using Tp3.Exceptions;

namespace Tp3.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "An unhandled exception occurred: {Message}", error.Message);
                await HandleExceptionAsync(context, error);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception error)
        {
            // If the request is for a view (HTML), we might want to redirect to an error page
            // Check if the request accepts HTML
            bool isHtmlRequest = context.Request.Headers["Accept"].ToString().Contains("text/html");

            if (isHtmlRequest)
            {
                // For specific exceptions, we can redirect to specific pages
                if (error is NotFoundException)
                {
                    // Assuming we will create a ResourceNotFound action in HomeController
                    context.Response.Redirect("/Home/ResourceNotFound");
                    return Task.CompletedTask;
                }

                // For other exceptions, redirect to the generic error page
                context.Response.Redirect("/Home/Error");
                return Task.CompletedTask;
            }

            // API / JSON Response
            context.Response.ContentType = "application/json";
            
            switch (error)
            {
                case AppException e:
                    // custom application error
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case NotFoundException e:
                    // not found error
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                default:
                    // unhandled error
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var result = JsonSerializer.Serialize(new { message = error?.Message });
            return context.Response.WriteAsync(result);
        }
    }
}
