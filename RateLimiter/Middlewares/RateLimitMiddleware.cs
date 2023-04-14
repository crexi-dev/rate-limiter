using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Attributes;
using RateLimiter.Attributes.Interfaces;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter.Middlewares;

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    
    public RateLimitMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IServiceProvider serviceProvider)
    {
        var resource = context.GetEndpoint();
        var rateLimitingConfigurations = resource?.Metadata.GetOrderedMetadata<IRateLimiterAttribute>();
        var canProcess = true;
        if (rateLimitingConfigurations != null)
        {
            foreach (var rateLimiter in rateLimitingConfigurations)
            {
                var type = typeof(IRule<>).MakeGenericType(rateLimiter.GetType());
                var ruleService = (IRule)serviceProvider.GetRequiredService(type);
                ruleService.SetParameters(rateLimiter);
                if (await ruleService.IsRestrict(context))
                {
                    canProcess = false;
                    break;
                }
            }
        }

        if (canProcess)
        {
            await _next(context);
            return;
        }
        
        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
    }
}