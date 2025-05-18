using RateLimiter.Common.Abstractions.Rules;

namespace RateLimiter.Core.Models;

/// <summary>
/// Represents the source of a rate limiting rule with precedence information
/// </summary>
public enum RuleSource
{
    /// <summary>
    /// Rules from code attributes - lowest precedence
    /// </summary>
    Attribute = 100,
    
    /// <summary>
    /// Rules from configuration (appsettings.json) - highest precedence
    /// </summary>
    Configuration = 10
}

/// <summary>
/// Extended rule information with source and precedence
/// </summary>
public class RuleInfo
{
    public required IRateLimitRule Rule { get; init; }
    public RuleSource Source { get; init; }
    public int Priority { get; init; }
    public string Path { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets effective precedence (lower = higher precedence)
    /// </summary>
    public int EffectivePrecedence => (int)Source + Priority;
}
