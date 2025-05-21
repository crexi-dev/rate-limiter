using Microsoft.AspNetCore.Http;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Common.Models;

namespace RateLimiter.Core.Services.KeyBuilders;

/// <summary>
/// Default implementation of key builder for rate limiting.
/// </summary>
public class DefaultKeyBuilder : IKeyBuilder
{
    /// <summary>
    /// Builds a key for rate limiting.
    /// </summary>
    public string BuildKey(HttpContext context, IRateLimitRule rule, ClientIdentifier clientIdentifier)
    {
        return $"{rule.Name}:{clientIdentifier.Id}";
    }
}
