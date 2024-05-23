using System;

namespace RateLimiter.Rules.Interfaces;

/// <summary>
/// Interface for rate limit rules.
/// </summary>
public interface IRateLimitRule
{
    /// <summary>
    /// Checks if the request is allowed.
    /// </summary>
    /// <param name="clientToken">Client token.</param>
    /// <param name="resource">Request resource.</param>
    /// <param name="requestTime">Request time.</param>
    /// <returns>true if the request is allowed; otherwise, false.</returns>
    bool IsRequestAllowed(string clientToken, string resource, DateTime requestTime);
}