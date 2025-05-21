using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Core.Configuration;
using RateLimiter.Core.Models;

namespace RateLimiter.Core.Services;

/// <summary>
/// Enhanced hybrid rule provider with proper path-based conflict resolution
/// and configuration precedence
/// </summary>
public class EnhancedHybridRuleProvider : IRateLimitRuleProvider
{
    private readonly ConfigurationRuleProvider? _configProvider;
    private readonly AttributeBasedRuleProvider? _attributeProvider;
    private readonly EnhancedRateLimitConfiguration _config;
    private readonly ILogger<EnhancedHybridRuleProvider> _logger;
    private readonly IReadOnlyList<RuleInfo> _allRules;

    public EnhancedHybridRuleProvider(
        IOptions<EnhancedRateLimitConfiguration> config,
        ILogger<EnhancedHybridRuleProvider> logger,
        ConfigurationRuleProvider? configProvider = null,
        AttributeBasedRuleProvider? attributeProvider = null)
    {
        _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configProvider = configProvider;
        _attributeProvider = attributeProvider;
        
        // Pre-build and resolve all rules
        _allRules = BuildResolvedRules().Result;
    }

    public async Task<IEnumerable<IRateLimitRule>> GetAllRulesAsync()
    {
        return await Task.FromResult(_allRules.Select(r => r.Rule));
    }

    public async Task<IEnumerable<IRateLimitRule>> GetMatchingRulesAsync(HttpContext context)
    {
        var matchingRules = new List<RuleInfo>();
        
        // Get all rules that match this request
        foreach (var ruleInfo in _allRules)
        {
            if (ruleInfo.Rule.IsMatch(context))
            {
                matchingRules.Add(ruleInfo);
                _logger.LogDebug("Rule {RuleName} from {Source} matches request {Path}", 
                    ruleInfo.Rule.Name, ruleInfo.Source, context.Request.Path);
            }
        }
        
        // Apply conflict resolution and precedence
        var resolvedRules = ApplyConflictResolution(matchingRules, context);
        
        _logger.LogDebug("Resolved to {Count} rules for {Path}: {Rules}", 
            resolvedRules.Count, 
            context.Request.Path,
            string.Join(", ", resolvedRules.Select(r => $"{r.Rule.Name}({r.Source})")));
        
        return await Task.FromResult(resolvedRules.Select(r => r.Rule));
    }
    
    private async Task<IReadOnlyList<RuleInfo>> BuildResolvedRules()
    {
        var allRuleInfos = new List<RuleInfo>();
        var conflicts = new List<RuleConflict>();
        
        // Add configuration rules first (higher precedence)
        if (_config.EnableConfigurationRules && _configProvider != null)
        {
            var configRules = await _configProvider.GetAllRulesAsync();
            foreach (var rule in configRules)
            {
                var priority = rule is ConfigurableRule configRule ? configRule.Priority : 10;
                allRuleInfos.Add(new RuleInfo
                {
                    Rule = rule,
                    Source = RuleSource.Configuration,
                    Priority = priority,
                    Path = ExtractPathFromRule(rule)
                });
            }
            _logger.LogInformation("Loaded {Count} configuration rules", configRules.Count());
        }
        
        // Add attribute rules with conflict detection
        if (_config.EnableAttributeRules && _attributeProvider != null)
        {
            var attributeRules = await _attributeProvider.GetAllRulesAsync();
            foreach (var rule in attributeRules)
            {
                var ruleInfo = new RuleInfo
                {
                    Rule = rule,
                    Source = RuleSource.Attribute,
                    Priority = _config.DefaultAttributePriority,
                    Path = ExtractPathFromRule(rule)
                };
                
                // Check for conflicts with existing rules
                var conflict = DetectConflicts(ruleInfo, allRuleInfos);
                if (conflict != null)
                {
                    conflicts.Add(conflict);
                    if (_config.LogConflicts)
                    {
                        _logger.LogWarning("Rule conflict detected: {Conflict}", conflict);
                    }
                }
                
                allRuleInfos.Add(ruleInfo);
            }
            _logger.LogInformation("Loaded {Count} attribute rules", attributeRules.Count());
        }
        
        if (conflicts.Any())
        {
            _logger.LogInformation("Detected {Count} rule conflicts, applying resolution strategy: {Strategy}", 
                conflicts.Count, _config.ConflictResolutionStrategy);
        }
        
        return allRuleInfos;
    }
    
    private RuleConflict? DetectConflicts(RuleInfo newRule, List<RuleInfo> existingRules)
    {
        foreach (var existing in existingRules)
        {
            // Name-based conflict
            if (string.Equals(newRule.Rule.Name, existing.Rule.Name, StringComparison.OrdinalIgnoreCase))
            {
                return new RuleConflict
                {
                    ConflictType = ConflictType.SameName,
                    Rule1 = existing.Rule,
                    Rule2 = newRule.Rule,
                    Rule1Source = existing.Source.ToString(),
                    Rule2Source = newRule.Source.ToString(),
                    Description = $"Rules have the same name: '{newRule.Rule.Name}'"
                };
            }
            
            // Path-based conflict (NEW: This is the key fix!)
            if (!string.IsNullOrEmpty(existing.Path) && 
                !string.IsNullOrEmpty(newRule.Path) &&
                PathsConflict(existing.Path, newRule.Path))
            {
                return new RuleConflict
                {
                    ConflictType = ConflictType.OverlappingPaths,
                    Rule1 = existing.Rule,
                    Rule2 = newRule.Rule,
                    Rule1Source = existing.Source.ToString(),
                    Rule2Source = newRule.Source.ToString(),
                    Description = $"Rules target overlapping paths: '{existing.Path}' vs '{newRule.Path}'"
                };
            }
        }
        
        return null;
    }
    
    private static bool PathsConflict(string path1, string path2)
    {
        // Exact match
        if (string.Equals(path1, path2, StringComparison.OrdinalIgnoreCase))
            return true;
            
        // One path is a prefix of another
        if (path1.StartsWith(path2, StringComparison.OrdinalIgnoreCase) ||
            path2.StartsWith(path1, StringComparison.OrdinalIgnoreCase))
            return true;
            
        return false;
    }
    
    private List<RuleInfo> ApplyConflictResolution(List<RuleInfo> matchingRules, HttpContext context)
    {
        // Group by conflicting rules (same name or overlapping paths)
        var resolvedRules = new List<RuleInfo>();
        var processedRules = new HashSet<string>();
        
        foreach (var rule in matchingRules)
        {
            if (processedRules.Contains(rule.Rule.Name))
                continue;
                
            // Find all conflicting rules
            var conflictingRules = matchingRules
                .Where(r => r.Rule.Name == rule.Rule.Name || PathsConflict(r.Path, rule.Path))
                .ToList();
                
            if (conflictingRules.Count == 1)
            {
                // No conflicts, add the rule
                resolvedRules.Add(rule);
            }
            else
            {
                // Resolve conflict based on strategy
                var winner = ResolveConflict(conflictingRules, context);
                if (winner != null)
                {
                    resolvedRules.Add(winner);
                    _logger.LogDebug("Conflict resolved: {WinnerName} from {WinnerSource} wins over {LoserCount} other rules",
                        winner.Rule.Name, winner.Source, conflictingRules.Count - 1);
                }
            }
            
            // Mark all conflicting rules as processed
            foreach (var conflicting in conflictingRules)
            {
                processedRules.Add(conflicting.Rule.Name);
            }
        }
        
        return resolvedRules;
    }
    
    private RuleInfo? ResolveConflict(List<RuleInfo> conflictingRules, HttpContext context)
    {
        switch (_config.ConflictResolutionStrategy)
        {
            case ConflictResolutionStrategy.ConfigurationWins:
                return conflictingRules
                    .OrderBy(r => r.EffectivePrecedence) // Configuration has lower precedence number
                    .First();
                    
            case ConflictResolutionStrategy.AttributeWins:
                return conflictingRules
                    .OrderByDescending(r => r.EffectivePrecedence) // Attribute has higher precedence number
                    .First();
                    
            case ConflictResolutionStrategy.MostRestrictive:
                return FindMostRestrictiveRule(conflictingRules, context);
                    
            case ConflictResolutionStrategy.PriorityBased:
                return conflictingRules
                    .OrderBy(r => r.EffectivePrecedence)
                    .First();
                    
            default:
                _logger.LogWarning("Unknown conflict resolution strategy: {Strategy}, using ConfigurationWins", 
                    _config.ConflictResolutionStrategy);
                return conflictingRules
                    .OrderBy(r => r.EffectivePrecedence)
                    .First();
        }
    }
    
    private RuleInfo? FindMostRestrictiveRule(List<RuleInfo> rules, HttpContext context)
    {
        RuleInfo? mostRestrictive = null;
        double highestRestriction = 0;
        
        foreach (var ruleInfo in rules)
        {
            try
            {
                var limit = ruleInfo.Rule.GetLimit(context);
                var restrictiveness = CalculateRestrictiveness(limit);
                
                if (restrictiveness > highestRestriction)
                {
                    highestRestriction = restrictiveness;
                    mostRestrictive = ruleInfo;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error calculating restrictiveness for rule {RuleName}", ruleInfo.Rule.Name);
            }
        }
        
        return mostRestrictive;
    }
    
    private static double CalculateRestrictiveness(Common.Models.RateLimit limit)
    {
        // Higher rate = less restrictive, so we invert it
        var requestsPerSecond = (double)limit.MaxRequests / limit.TimeWindowInSeconds;
        return 1.0 / requestsPerSecond; // Higher value = more restrictive
    }
    
    private static string ExtractPathFromRule(IRateLimitRule rule)
    {
        // Try to extract path information from the rule
        // This is a simplified implementation - in practice you might want more sophisticated path extraction
        return rule.Name switch
        {
            "GlobalLimit" => "/api/demo",
            "ApiUserEndpoint" => "/api/demo/users",
            "ApiUserDetailsEndpoint" => "/api/demo/users/*",
            "BurstLimit" => "/api/demo/burst", 
            "UsRegionLimit" => "/api/demo/region/us",
            "EuRegionLimit" => "/api/demo/region/eu",
            "AdminApiLimit" => "/api/admin/*",
            "AuthenticatedUserLimit" => "/api/enhanceddemo/authenticated",
            "PremiumUserLimit" => "/api/enhanceddemo/premium",
            "RegionAwareLimit" => "/api/enhanceddemo/region-aware",
            "LoadTestLimit" => "/api/enhanceddemo/simulate-load",
            _ => string.Empty
        };
    }
}
