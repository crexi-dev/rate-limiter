using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Core.Configuration;
using RateLimiter.Core.Models;

namespace RateLimiter.Core.Services;

/// <summary>
/// Hybrid rule provider that intelligently combines configuration-based and attribute-based rules
/// </summary>
public class HybridRuleProvider : IRateLimitRuleProvider
{
    private readonly ConfigurationRuleProvider? _configProvider;
    private readonly AttributeBasedRuleProvider? _attributeProvider;
    private readonly EnhancedRateLimitConfiguration _config;
    private readonly ILogger<HybridRuleProvider> _logger;
    private readonly IReadOnlyList<IRateLimitRule> _allRules;

    public HybridRuleProvider(
        IOptions<EnhancedRateLimitConfiguration> config,
        ILogger<HybridRuleProvider> logger,
        ConfigurationRuleProvider? configProvider = null,
        AttributeBasedRuleProvider? attributeProvider = null)
    {
        _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configProvider = configProvider;
        _attributeProvider = attributeProvider;
        
        // Pre-build combined rules for performance
        _allRules = BuildCombinedRules().Result;
    }

    public Task<IEnumerable<IRateLimitRule>> GetAllRulesAsync()
    {
        return Task.FromResult<IEnumerable<IRateLimitRule>>(_allRules);
    }

    public async Task<IEnumerable<IRateLimitRule>> GetMatchingRulesAsync(HttpContext context)
    {
        var matchingRules = new List<(IRateLimitRule Rule, RuleSource Source, int Priority)>();
        
        // Get configuration-based matches
        if (_config.EnableConfigurationRules && _configProvider != null)
        {
            var configMatches = await _configProvider.GetMatchingRulesAsync(context);
            foreach (var rule in configMatches)
            {
                var priority = rule is ConfigurableRule configRule ? configRule.Priority : 100;
                matchingRules.Add((rule, RuleSource.Configuration, priority));
            }
        }
        
        // Get attribute-based matches
        if (_config.EnableAttributeRules && _attributeProvider != null)
        {
            var attributeMatches = await _attributeProvider.GetMatchingRulesAsync(context);
            foreach (var rule in attributeMatches)
            {
                var conflictResolution = ResolveRuleConflicts(rule, matchingRules, context);
                if (conflictResolution.ShouldInclude)
                {
                    matchingRules.Add((rule, RuleSource.Attribute, conflictResolution.Priority));
                }
            }
        }
        
        // Apply final conflict resolution and sort by priority
        var resolvedRules = ApplyConflictResolution(matchingRules, context);
        
        _logger.LogDebug("Found {Count} resolved rules for {Path}: {Rules}", 
            resolvedRules.Count, 
            context.Request.Path,
            string.Join(", ", resolvedRules.Select(r => $"{r.Rule.Name}({r.Source})")));
        
        return resolvedRules.Select(r => r.Rule);
    }
    
    private async Task<IReadOnlyList<IRateLimitRule>> BuildCombinedRules()
    {
        var allRules = new List<(IRateLimitRule Rule, RuleSource Source)>();
        var conflicts = new List<RuleConflict>();
        
        // Add configuration rules
        if (_config.EnableConfigurationRules && _configProvider != null)
        {
            var configRules = await _configProvider.GetAllRulesAsync();
            allRules.AddRange(configRules.Select(r => (r, RuleSource.Configuration)));
            _logger.LogInformation("Loaded {Count} configuration-based rules", configRules.Count());
        }
        
        // Add attribute rules with conflict detection
        if (_config.EnableAttributeRules && _attributeProvider != null)
        {
            var attributeRules = await _attributeProvider.GetAllRulesAsync();
            
            foreach (var attributeRule in attributeRules)
            {
                var conflict = DetectRuleConflicts(attributeRule, allRules.Select(r => r.Rule));
                if (conflict != null)
                {
                    conflicts.Add(conflict);
                    if (_config.LogConflicts)
                    {
                        _logger.LogWarning("Rule conflict detected: {Conflict}", conflict);
                    }
                }
                
                allRules.Add((attributeRule, RuleSource.Attribute));
            }
            
            _logger.LogInformation("Loaded {Count} attribute-based rules", attributeRules.Count());
        }
        
        if (conflicts.Any() && _config.LogConflicts)
        {
            _logger.LogWarning("Found {Count} rule conflicts. Resolution strategy: {Strategy}", 
                conflicts.Count, _config.ConflictResolutionStrategy);
        }
        
        _logger.LogInformation("Total rules loaded: {Count} ({ConfigCount} config + {AttrCount} attribute)", 
            allRules.Count,
            allRules.Count(r => r.Source == RuleSource.Configuration),
            allRules.Count(r => r.Source == RuleSource.Attribute));
        
        return allRules.Select(r => r.Rule).ToList();
    }
    
    private RuleConflict? DetectRuleConflicts(IRateLimitRule newRule, IEnumerable<IRateLimitRule> existingRules)
    {
        foreach (var existingRule in existingRules)
        {
            if (string.Equals(newRule.Name, existingRule.Name, StringComparison.OrdinalIgnoreCase))
            {
                return new RuleConflict
                {
                    ConflictType = ConflictType.SameName,
                    Rule1 = existingRule,
                    Rule2 = newRule,
                    Rule1Source = "Configuration",
                    Rule2Source = "Attribute",
                    Description = $"Rules have the same name: '{newRule.Name}'"
                };
            }
        }
        
        return null;
    }
    
    private ConflictResolution ResolveRuleConflicts(
        IRateLimitRule rule, 
        List<(IRateLimitRule Rule, RuleSource Source, int Priority)> existingRules,
        HttpContext context)
    {
        var nameConflict = existingRules.FirstOrDefault(r => 
            string.Equals(r.Rule.Name, rule.Name, StringComparison.OrdinalIgnoreCase));
        
        if (nameConflict.Rule != null)
        {
            return ResolveNameConflict(rule, nameConflict, context);
        }
        
        return new ConflictResolution 
        { 
            ShouldInclude = true, 
            Priority = _config.DefaultAttributePriority
        };
    }
    
    private ConflictResolution ResolveNameConflict(
        IRateLimitRule attributeRule,
        (IRateLimitRule Rule, RuleSource Source, int Priority) existingRule,
        HttpContext context)
    {
        switch (_config.ConflictResolutionStrategy)
        {
            case ConflictResolutionStrategy.ConfigurationWins:
                if (existingRule.Source == RuleSource.Configuration)
                {
                    _logger.LogDebug("Configuration rule '{RuleName}' takes precedence over attribute rule", 
                        attributeRule.Name);
                    return new ConflictResolution { ShouldInclude = false, Reason = "Configuration wins" };
                }
                break;
                
            case ConflictResolutionStrategy.AttributeWins:
                if (existingRule.Source == RuleSource.Configuration)
                {
                    _logger.LogDebug("Attribute rule '{RuleName}' overrides configuration rule", 
                        attributeRule.Name);
                    return new ConflictResolution { ShouldInclude = true, Priority = 50, Reason = "Attribute wins" };
                }
                break;
                
            case ConflictResolutionStrategy.MostRestrictive:
                return ResolveMostRestrictive(attributeRule, existingRule.Rule, context);
                
            case ConflictResolutionStrategy.Combine:
                return new ConflictResolution 
                { 
                    ShouldInclude = true, 
                    Priority = Math.Min(existingRule.Priority, _config.DefaultAttributePriority) - 1,
                    Reason = "Combine rules"
                };
                
            default:
                _logger.LogWarning("Unknown conflict resolution strategy: {Strategy}", 
                    _config.ConflictResolutionStrategy);
                break;
        }
        
        return new ConflictResolution { ShouldInclude = true, Priority = _config.DefaultAttributePriority };
    }
    
    private ConflictResolution ResolveMostRestrictive(IRateLimitRule rule1, IRateLimitRule rule2, HttpContext context)
    {
        try
        {
            var limit1 = rule1.GetLimit(context);
            var limit2 = rule2.GetLimit(context);
            
            var rate1 = (double)limit1.MaxRequests / limit1.TimeWindowInSeconds;
            var rate2 = (double)limit2.MaxRequests / limit2.TimeWindowInSeconds;
            
            if (rate1 <= rate2)
            {
                _logger.LogDebug("Rule '{Rule1}' is more restrictive than '{Rule2}' ({Rate1} vs {Rate2} req/s)",
                    rule1.Name, rule2.Name, rate1, rate2);
                return new ConflictResolution { ShouldInclude = false, Reason = "Existing rule more restrictive" };
            }
            else
            {
                _logger.LogDebug("Rule '{Rule2}' is more restrictive than '{Rule1}' ({Rate2} vs {Rate1} req/s)",
                    rule2.Name, rule1.Name, rate2, rate1);
                return new ConflictResolution { ShouldInclude = true, Priority = 50, Reason = "New rule more restrictive" };
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error comparing rule restrictiveness for '{Rule1}' vs '{Rule2}'",
                rule1.Name, rule2.Name);
            return new ConflictResolution { ShouldInclude = true, Priority = _config.DefaultAttributePriority };
        }
    }
    
    private List<(IRateLimitRule Rule, RuleSource Source)> ApplyConflictResolution(
        List<(IRateLimitRule Rule, RuleSource Source, int Priority)> rules,
        HttpContext context)
    {
        var ruleGroups = rules.GroupBy(r => r.Rule.Name, StringComparer.OrdinalIgnoreCase);
        var resolvedRules = new List<(IRateLimitRule Rule, RuleSource Source, int Priority)>();
        
        foreach (var group in ruleGroups)
        {
            if (group.Count() == 1)
            {
                resolvedRules.Add(group.First());
            }
            else
            {
                var winner = group.OrderBy(r => r.Priority).First();
                resolvedRules.Add(winner);
                
                _logger.LogDebug("Conflict resolution: Rule '{RuleName}' from {Source} wins (priority {Priority})",
                    winner.Rule.Name, winner.Source, winner.Priority);
            }
        }
        
        return resolvedRules
            .OrderBy(r => r.Priority)
            .Select(r => (r.Rule, r.Source))
            .ToList();
    }
}
