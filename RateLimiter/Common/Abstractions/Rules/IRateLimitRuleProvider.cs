using Microsoft.AspNetCore.Http;

namespace RateLimiter.Common.Abstractions.Rules;

/// <summary>
/// Provides access to rate limit rules.
/// </summary>
public interface IRateLimitRuleProvider
{
    /// <summary>
    /// Gets all available rate limit rules.
    /// </summary>
    Task<IEnumerable<IRateLimitRule>> GetAllRulesAsync();

    /// <summary>
    /// Gets all rules that apply to the given HTTP context.
    /// </summary>
    Task<IEnumerable<IRateLimitRule>> GetMatchingRulesAsync(HttpContext context);
}
