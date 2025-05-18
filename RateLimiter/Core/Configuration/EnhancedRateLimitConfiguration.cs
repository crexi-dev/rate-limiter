namespace RateLimiter.Core.Configuration;

/// <summary>
/// Enhanced configuration for hybrid rate limiting
/// </summary>
public class EnhancedRateLimitConfiguration
{
    public const string SectionName = "RateLimiting";
    
    /// <summary>
    /// Gets or sets the collection of rate limit rules
    /// </summary>
    public List<RateLimitRuleConfiguration> Rules { get; set; } = new();
    
    /// <summary>
    /// Gets or sets whether to enable configuration-based rules
    /// </summary>
    public bool EnableConfigurationRules { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether to enable attribute-based rules
    /// </summary>
    public bool EnableAttributeRules { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the conflict resolution strategy when rules overlap
    /// </summary>
    public ConflictResolutionStrategy ConflictResolutionStrategy { get; set; } = ConflictResolutionStrategy.ConfigurationWins;
    
    /// <summary>
    /// Gets or sets whether to log rule conflicts
    /// </summary>
    public bool LogConflicts { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the default priority for attribute-based rules
    /// </summary>
    public int DefaultAttributePriority { get; set; } = 200;
    
    /// <summary>
    /// Gets or sets whether to validate rule configurations on startup
    /// </summary>
    public bool ValidateOnStartup { get; set; } = true;
    
    /// <summary>
    /// Gets or sets performance optimization settings
    /// </summary>
    public PerformanceSettings Performance { get; set; } = new();
}

/// <summary>
/// Performance optimization settings
/// </summary>
public class PerformanceSettings
{
    /// <summary>
    /// Gets or sets whether to cache compiled rules
    /// </summary>
    public bool CacheCompiledRules { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the rule cache expiration time in minutes
    /// </summary>
    public int RuleCacheExpirationMinutes { get; set; } = 60;
    
    /// <summary>
    /// Gets or sets whether to pre-compile path patterns
    /// </summary>
    public bool PreCompilePatterns { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the maximum number of rules to evaluate per request
    /// </summary>
    public int MaxRulesPerRequest { get; set; } = 10;
}

/// <summary>
/// Extended rule configuration with hybrid support
/// </summary>
public class RateLimitRuleConfiguration
{
    /// <summary>
    /// Gets or sets the unique name of the rule
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the rule type
    /// </summary>
    public string Type { get; set; } = "FixedWindow";
    
    /// <summary>
    /// Gets or sets the maximum number of requests
    /// </summary>
    public int MaxRequests { get; set; }
    
    /// <summary>
    /// Gets or sets the time window in seconds
    /// </summary>
    public int TimeWindowSeconds { get; set; }
    
    /// <summary>
    /// Gets or sets the path pattern to match
    /// </summary>
    public string PathPattern { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the HTTP methods this rule applies to
    /// </summary>
    public string HttpMethods { get; set; } = "GET,POST,PUT,DELETE";
    
    /// <summary>
    /// Gets or sets whether the rule is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the priority (lower = higher priority)
    /// </summary>
    public int Priority { get; set; } = 100;
    
    /// <summary>
    /// Gets or sets the rule source (for tracking)
    /// </summary>
    public string Source { get; set; } = "Configuration";
    
    /// <summary>
    /// Gets or sets whether this rule can be overridden by attribute rules
    /// </summary>
    public bool AllowOverride { get; set; } = false;
    
    /// <summary>
    /// Gets or sets whether this rule should override conflicting attribute rules
    /// </summary>
    public bool OverrideAttributes { get; set; } = true;
    
    // Region-based properties
    public string? TargetRegion { get; set; }
    public int MinTimeBetweenRequestsMs { get; set; } = 0;
    
    // Token bucket properties
    public int BucketCapacity { get; set; }
    public double RefillRatePerSecond { get; set; } = 1.0;
    
    /// <summary>
    /// Gets or sets additional metadata for the rule
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
}
