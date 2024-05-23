using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RateLimiter.Exceptions;

namespace RateLimiter.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RateLimitFilter : Attribute, IAsyncActionFilter
{
    private readonly string _region;
    private readonly string _clientId;
    private readonly int _statusCode;
    private readonly string _message;

    public RateLimitFilter(string region, string clientId, int statusCode = 429, string message = "Rate limit exceeded")
    {
        _region = region;
        _clientId = clientId;
        _statusCode = statusCode;
        _message = message;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var rateLimiter = context.HttpContext.RequestServices.GetService<RateLimiter>();
        if (rateLimiter == null)
        {
            throw new RateLimiterConfigurationException("RateLimiter service is not registered.");
        }

        var logger = context.HttpContext.RequestServices.GetService<ILogger<RateLimitFilter>>();

        try
        {
            var allowed = await rateLimiter.IsRequestAllowedAsync(_clientId, _region);
            if (!allowed)
            {
                logger?.LogWarning("Rate limit exceeded for client {ClientId} in region {Region}", _clientId, _region);
                throw new RateLimitExceededException(_message);
            }
            await next();
        }
        catch (RateLimitExceededException ex)
        {
            context.Result = new ContentResult
            {
                StatusCode = _statusCode,
                Content = ex.Message
            };
        }

        await next();
    }
}