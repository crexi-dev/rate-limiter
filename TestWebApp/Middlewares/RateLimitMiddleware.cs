using System.ComponentModel.DataAnnotations;
using System.Text;
using RateLimiter;
using RateLimiter.Abstractions;

namespace TestWebApp.Middlewares
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;

        public RateLimitMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IRateLimitValidator rateLimitValidator)
        {
            context.Request.Headers.TryGetValue("x-api-key", out var apiKey);

            try
            {
                await rateLimitValidator.ValidateAsync(apiKey.ToString());
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                var bytes = Encoding.UTF8.GetBytes(ex.Message);
                await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                return;
            }

            await _next(context);
        }
    }


    public static class RateLimitMiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimitMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitMiddleware>();
        }
    }
}
