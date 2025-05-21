namespace RateLimiter.Core.Configuration;

/// <summary>
/// Strategies for resolving conflicts between configuration and attribute rules
/// </summary>
public enum ConflictResolutionStrategy
{
    /// <summary>
    /// Configuration rules take precedence over attribute rules (recommended for production)
    /// </summary>
    ConfigurationWins,
    
    /// <summary>
    /// Attribute rules take precedence over configuration rules (useful for development)
    /// </summary>
    AttributeWins,
    
    /// <summary>
    /// Choose the most restrictive rule (lowest rate limit)
    /// </summary>
    MostRestrictive,
    
    /// <summary>
    /// Combine both rules (both will be evaluated)
    /// </summary>
    Combine,
    
    /// <summary>
    /// Use priority-based resolution (lowest priority number wins)
    /// </summary>
    PriorityBased
}
