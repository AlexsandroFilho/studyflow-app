using System.Security.Claims;
using StudyFlow.Api.Data;

namespace StudyFlow.Api.Middlewares
{
    public class CurrentUsuarioMiddleware
    {
        private readonly RequestDelegate _next;

        public CurrentUsuarioMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            var userIdValue = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userIdValue, out var userId))
                dbContext.CurrentUsuarioId = userId;

            await _next(context);
        }
    }
}