using System.Collections.Generic;
using RateLimiter.Rules;

namespace RateLimiter.Models;

/// <summary>
/// Represents the configuration options for the rate limiter.
/// </summary>
public class RateLimiterOptions
{
    /// <summary>
    /// Gets or sets the rate limit rules for each endpoint.
    /// </summary>
    /// <remarks>
    /// The dictionary key represents the endpoint, and the value is a list of rate limit rules that apply to that endpoint.
    /// </remarks>
    public Dictionary<string, List<IRateLimitRule>> Rules { get; set; } = new();
}