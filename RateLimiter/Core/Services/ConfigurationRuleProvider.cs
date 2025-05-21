using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateLimiter.Common.Abstractions.Counters;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Common.Models;
using RateLimiter.Core.Configuration;
using RateLimiter.Core.Rules;
using RateLimiter.Core.Services.KeyBuilders;

namespace RateLimiter.Core.Services;

/// <summary>
/// Provides rate limit rules from configuration (appsettings.json)
/// </summary>
public class ConfigurationRuleProvider : IRateLimitRuleProvider
{
    private readonly ILogger<ConfigurationRuleProvider> _logger;
    private readonly IKeyBuilder _keyBuilder;
    private readonly IRateLimitCounter _counter;
    private readonly ILoggerFactory _loggerFactory;
    private readonly EnhancedRateLimitConfiguration _config;
    private readonly IReadOnlyList<IRateLimitRule> _rules;

    public ConfigurationRuleProvider(
        IKeyBuilder keyBuilder,
        IRateLimitCounter counter,
        ILoggerFactory loggerFactory,
        IOptions<EnhancedRateLimitConfiguration> configuration,
        ILogger<ConfigurationRuleProvider> logger)
    {
        _keyBuilder = keyBuilder ?? throw new ArgumentNullException(nameof(keyBuilder));
        _counter = counter ?? throw new ArgumentNullException(nameof(counter));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _config = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
        
        // Build rules from configuration
        _rules = BuildRulesFromConfiguration();
    }

    public Task<IEnumerable<IRateLimitRule>> GetAllRulesAsync()
    {
        return Task.FromResult<IEnumerable<IRateLimitRule>>(_rules);
    }

    public async Task<IEnumerable<IRateLimitRule>> GetMatchingRulesAsync(HttpContext context)
    {
        var allRules = await GetAllRulesAsync();
        var matchingRules = new List<IRateLimitRule>();

        foreach (var rule in allRules)
        {
            if (rule.IsMatch(context))
            {
                matchingRules.Add(rule);
                _logger.LogDebug("Configuration rule {RuleName} matches request for {Path}", 
                    rule.Name, context.Request.Path);
            }
        }

        // Sort by priority if using ConfigurableRule
        if (matchingRules.Any(r => r is ConfigurableRule))
        {
            matchingRules = matchingRules
                .Cast<ConfigurableRule>()
                .OrderBy(r => r.Priority)
                .Cast<IRateLimitRule>()
                .ToList();
        }

        return matchingRules;
    }
    
    private List<IRateLimitRule> BuildRulesFromConfiguration()
    {
        var rules = new List<IRateLimitRule>();
        
        if (!_config.EnableConfigurationRules)
        {
            _logger.LogInformation("Configuration-based rules are disabled");
            return rules;
        }
        
        foreach (var ruleConfig in _config.Rules.Where(r => r.Enabled))
        {
            try
            {
                var rule = CreateRuleFromConfiguration(ruleConfig);
                if (rule != null)
                {
                    rules.Add(rule);
                    _logger.LogInformation(
                        "Created configuration rule '{RuleName}' of type '{RuleType}' for path '{PathPattern}'",
                        ruleConfig.Name, ruleConfig.Type, ruleConfig.PathPattern);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create rule '{RuleName}' from configuration", ruleConfig.Name);
            }
        }
        
        _logger.LogInformation("Built {RuleCount} rules from configuration", rules.Count);
        return rules;
    }
    
    private IRateLimitRule? CreateRuleFromConfiguration(RateLimitRuleConfiguration config)
    {
        var matcher = CreateMatcherFunction(config);
        
        switch (config.Type.ToLowerInvariant())
        {
            case "fixedwindow":
                return new ConfigurableRule(
                    config.Name,
                    config.Priority,
                    new FixedWindowRule(
                        config.Name,
                        new RateLimit { MaxRequests = config.MaxRequests, TimeWindowInSeconds = config.TimeWindowSeconds },
                        _keyBuilder,
                        _counter,
                        _loggerFactory.CreateLogger<FixedWindowRule>(),
                        matcher),
                    matcher);
                    
            case "slidingwindow":
                return new ConfigurableRule(
                    config.Name,
                    config.Priority,
                    new SlidingWindowRule(
                        config.Name,
                        new RateLimit { MaxRequests = config.MaxRequests, TimeWindowInSeconds = config.TimeWindowSeconds },
                        _keyBuilder,
                        _counter,
                        _loggerFactory.CreateLogger<SlidingWindowRule>(),
                        matcher),
                    matcher);
                    
            case "tokenbucket":
                return new ConfigurableRule(
                    config.Name,
                    config.Priority,
                    new TokenBucketRule(
                        config.Name,
                        config.BucketCapacity > 0 ? config.BucketCapacity : config.MaxRequests,
                        config.RefillRatePerSecond,
                        _keyBuilder,
                        _counter,
                        _loggerFactory.CreateLogger<TokenBucketRule>(),
                        matcher),
                    matcher);
                    
            case "regionbased":
                if (string.IsNullOrEmpty(config.TargetRegion))
                {
                    _logger.LogWarning("RegionBased rule '{RuleName}' has no TargetRegion specified", config.Name);
                    return null;
                }
                
                return new ConfigurableRule(
                    config.Name,
                    config.Priority,
                    new RegionBasedRule(
                        config.Name,
                        config.TargetRegion,
                        new RateLimit { MaxRequests = config.MaxRequests, TimeWindowInSeconds = config.TimeWindowSeconds },
                        _keyBuilder,
                        _counter,
                        _loggerFactory.CreateLogger<RegionBasedRule>(),
                        config.MinTimeBetweenRequestsMs,
                        matcher),
                    matcher);
                    
            default:
                _logger.LogWarning("Unknown rule type '{RuleType}' for rule '{RuleName}'", config.Type, config.Name);
                return null;
        }
    }
    
    private Func<HttpContext, bool> CreateMatcherFunction(RateLimitRuleConfiguration config)
    {
        var allowedMethods = config.HttpMethods
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(m => m.Trim().ToUpperInvariant())
            .ToHashSet();
        
        var pathMatcher = CreatePathMatcher(config.PathPattern);
        
        return context =>
        {
            if (allowedMethods.Any() && !allowedMethods.Contains(context.Request.Method.ToUpperInvariant()))
            {
                return false;
            }
            
            return pathMatcher(context.Request.Path.Value ?? string.Empty);
        };
    }
    
    private Func<string, bool> CreatePathMatcher(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            return _ => true;
        }
        
        if (pattern.Contains('*'))
        {
            var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            return path => regex.IsMatch(path);
        }
        
        return path => string.Equals(path, pattern, StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Wrapper for configuration-based rules with priority support
/// </summary>
public class ConfigurableRule : IRateLimitRule
{
    private readonly IRateLimitRule _innerRule;
    private readonly Func<HttpContext, bool> _matcher;
    
    public string Name { get; }  // Fixed: get-only property
    public int Priority { get; }
    
    public ConfigurableRule(string name, int priority, IRateLimitRule innerRule, Func<HttpContext, bool> matcher)
    {
        Name = name;  // Now this works
        Priority = priority;
        _innerRule = innerRule;
        _matcher = matcher;
    }
    
    public bool IsMatch(HttpContext context) => _matcher(context);
    
    public RateLimit GetLimit(HttpContext context) => _innerRule.GetLimit(context);
    
    public Task<RateLimitResult> EvaluateAsync(HttpContext context, ClientIdentifier clientIdentifier) =>
        _innerRule.EvaluateAsync(context, clientIdentifier);
}
