using Microsoft.AspNetCore.Http;
using RateLimiter.Common.Models;

namespace RateLimiter.Common.Abstractions.Rules;

/// <summary>
/// Interface for rate limiter service.
/// </summary>
public interface IRateLimiterService
{
    /// <summary>
    /// Evaluates if a request should be allowed based on rate limits.
    /// </summary>
    Task<RateLimitResult> EvaluateRequestAsync(HttpContext context);

    /// <summary>
    /// Resets rate limits for a client.
    /// </summary>
    Task ResetLimitsAsync(string clientId);
}
