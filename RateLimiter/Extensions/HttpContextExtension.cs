using RateLimiter.Rules.Base;

namespace RateLimiter.Extensions;

using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

public static class HttpContextExtension
{
    public static bool HasRateLimitAttribute(this HttpContext context, out IReadOnlyList<RateLimitAttribute>? rateLimitAttribute)
    {
        rateLimitAttribute = context.GetEndpoint()?.Metadata.GetOrderedMetadata<RateLimitAttribute>();
        return rateLimitAttribute is not null;
    }

    public static string GetCustomerKey(this HttpContext context) => $"{context.Request.Path}_{context.Connection.RemoteIpAddress}";
}