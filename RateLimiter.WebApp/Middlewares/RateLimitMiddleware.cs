using RateLimiter.Machine;
using RateLimiter.Models;

namespace RateLimiter.WebApp.Middlewares;

public static class RateLimitMiddlewareExtensions
{
    public static IApplicationBuilder UseRateLimit(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RateLimitMiddleware>();
    }
}

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;

    public RateLimitMiddleware( RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IRuleStateMachine<RequestData> ruleStateMachine, ILogger<RateLimitMiddleware> logger)
    {
        if (!context.Request.Cookies.TryGetValue("token", out string? token) || String.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(120)))
        {
            bool result = false;
            try
            {
                result = await ruleStateMachine.RunAsync(new(token), source.Token);
            }
            catch (OperationCanceledException ex)
            {
                logger.LogInformation("Request with token {token} was cancelled: {ex}", token, ex);
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            }
            catch (Exception ex)
            {
                logger.LogError("Rule check failed: {ex}", ex);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
            if (result)
            {
                await _next(context);
            }
        }
    }
}