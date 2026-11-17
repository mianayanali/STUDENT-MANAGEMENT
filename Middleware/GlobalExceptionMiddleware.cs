using System.Net;
using System.Text.Json;

namespace STUDENT_MANAGEMENT.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Request ko agle middleware / controller tak jane dena
                await _next(context);
            }
            catch (Exception ex)
            {
                // Terminal me asal error log karna taake developer dekh sake
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

                // Client ko clean aur safe response bhejna
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "An internal server error occurred. Please contact administrator.",
                Detailed = exception.Message // Development ke waqt useful hai
            };

            var json = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(json);
        }
    }
}
