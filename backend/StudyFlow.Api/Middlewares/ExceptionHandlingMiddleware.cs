

using System.Text.Json;

namespace StudyFlow.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu uma exceção não tratada: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = exception switch
            {
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            var response = new ErrorResponse(
                StatusCode: statusCode,
                Message: statusCode == StatusCodes.Status500InternalServerError
                    ? "Ocorreu um erro interno no servidor."
                    : exception.Message,
                Details: _environment.IsDevelopment() ? exception.StackTrace : null,
                TraceId: context.TraceIdentifier);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json; charset=utf-8";

            return context.Response.WriteAsJsonAsync(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }

    public sealed record ErrorResponse(
        int StatusCode,
        string Message,
        string? Details,
        string TraceId);
}