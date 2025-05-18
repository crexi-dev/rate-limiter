using Microsoft.AspNetCore.Http;
using RateLimiter.Common.Models;

namespace RateLimiter.Common.Abstractions.Rules;

/// <summary>
/// Interface for rate limit rules.
/// </summary>
public interface IRateLimitRule
{
    /// <summary>
    /// Gets the name of the rule.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Determines if this rule applies to the given HTTP context.
    /// </summary>
    bool IsMatch(HttpContext context);

    /// <summary>
    /// Gets the applicable rate limit for this rule.
    /// </summary>
    RateLimit GetLimit(HttpContext context);

    /// <summary>
    /// Evaluates if the request is within rate limits.
    /// </summary>
    Task<RateLimitResult> EvaluateAsync(HttpContext context, ClientIdentifier clientIdentifier);
}
