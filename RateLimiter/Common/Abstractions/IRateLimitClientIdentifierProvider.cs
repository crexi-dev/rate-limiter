using Microsoft.AspNetCore.Http;
using RateLimiter.Common.Models;

namespace RateLimiter.Common.Abstractions;

/// <summary>
/// Provides client identifiers for rate limiting.
/// </summary>
public interface IRateLimitClientIdentifierProvider
{
    /// <summary>
    /// Gets a client identifier from the HTTP context.
    /// </summary>
    Task<ClientIdentifier> GetClientIdentifierAsync(HttpContext context);
}
