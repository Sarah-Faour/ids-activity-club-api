using System.Net;
using System.Text.Json;
using ActivityClub.Contracts.Errors;

namespace ActivityClub.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            int statusCode;
            string message;

            // Map exceptions → HTTP status
            switch (ex)
            {
                case ArgumentException:
                    statusCode = (int)HttpStatusCode.BadRequest; // 400
                    message = ex.Message;
                    break;

                case InvalidOperationException:
                    statusCode = (int)HttpStatusCode.Conflict; // 409 (often used for “state conflict”)
                    message = ex.Message;
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError; // 500
                    message = "An unexpected error occurred.";
                    break;
            }

            var response = new ApiErrorResponse
            {
                StatusCode = statusCode,
                Message = message,
                Details = _env.IsDevelopment() ? ex.ToString() : null
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
