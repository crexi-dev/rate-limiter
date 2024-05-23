using Microsoft.AspNetCore.Http;
using RateLimiter.Attributes;

namespace RateLimiter;

public static class HttpContextExtension
{
    public static string GetCustomerKey(this HttpContext context)
        => $"{context.Request.Path}_{context.Connection.RemoteIpAddress}";

    public static bool HasRateLimitAttribute(this HttpContext context, out RateLimitAttribute? rateLimitAttribute)
    {
        rateLimitAttribute = context.GetEndpoint()?.Metadata.GetMetadata<RateLimitAttribute>();
        return rateLimitAttribute is not null;
    }
}