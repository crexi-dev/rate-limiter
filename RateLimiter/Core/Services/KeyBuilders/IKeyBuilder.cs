using Microsoft.AspNetCore.Http;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Common.Models;

namespace RateLimiter.Core.Services.KeyBuilders;

/// <summary>
/// Interface for building rate limiting keys.
/// </summary>
public interface IKeyBuilder
{
    /// <summary>
    /// Builds a key for rate limiting.
    /// </summary>
    string BuildKey(HttpContext context, IRateLimitRule rule, ClientIdentifier clientIdentifier);
}
