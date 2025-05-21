using RateLimiter.Common.Abstractions.Rules;

namespace RateLimiter.Core.Models;

/// <summary>
/// Represents a conflict between rules
/// </summary>
public class RuleConflict
{
    public ConflictType ConflictType { get; set; }
    public IRateLimitRule Rule1 { get; set; } = null!;
    public IRateLimitRule Rule2 { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public string Rule1Source { get; set; } = string.Empty;
    public string Rule2Source { get; set; } = string.Empty;
    
    public override string ToString() => 
        $"{ConflictType}: {Rule1.Name}({Rule1Source}) vs {Rule2.Name}({Rule2Source}) - {Description}";
}

/// <summary>
/// Types of rule conflicts
/// </summary>
public enum ConflictType
{
    SameName,
    OverlappingPaths,
    ConflictingLimits
}

/// <summary>
/// Result of conflict resolution
/// </summary>
public class ConflictResolution
{
    public bool ShouldInclude { get; set; }
    public int Priority { get; set; }
    public string? Reason { get; set; }
}
