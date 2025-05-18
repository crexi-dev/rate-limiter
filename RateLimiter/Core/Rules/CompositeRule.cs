using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Common.Models;

namespace RateLimiter.Core.Rules;

/// <summary>
/// Composite rule that combines multiple rate limit rules.
/// </summary>
public class CompositeRule : IRateLimitRule
{
    private readonly ILogger<CompositeRule> _logger;
    private readonly IEnumerable<IRateLimitRule> _rules;
    private readonly CompositeMode _mode;
    private readonly Func<HttpContext, bool>? _matcher;

    public string Name { get; }

    /// <summary>
    /// The mode in which multiple rules are evaluated.
    /// </summary>
    public enum CompositeMode
    {
        /// <summary>
        /// All rules must allow the request for it to be allowed.
        /// </summary>
        AllRules,
        
        /// <summary>
        /// At least one rule must allow the request for it to be allowed.
        /// </summary>
        AnyRule,
        
        /// <summary>
        /// The most restrictive rule's result is used.
        /// </summary>
        MostRestrictive
    }

    public CompositeRule(
        string name,
        IEnumerable<IRateLimitRule> rules,
        CompositeMode mode,
        ILogger<CompositeRule> logger,
        Func<HttpContext, bool>? matcher = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _rules = rules ?? throw new ArgumentNullException(nameof(rules));
        
        if (!_rules.Any())
        {
            throw new ArgumentException("At least one rule must be provided", nameof(rules));
        }
        
        _mode = mode;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _matcher = matcher;
    }

    /// <summary>
    /// Determines if this composite rule applies to the given HTTP context.
    /// A composite rule matches if any of its child rules match.
    /// </summary>
    public bool IsMatch(HttpContext context)
    {
        if (_matcher != null && !_matcher(context))
        {
            return false;
        }
        
        // If any of the child rules match, the composite rule matches
        return _rules.Any(rule => rule.IsMatch(context));
    }

    /// <summary>
    /// Gets the most restrictive rate limit from all matching child rules.
    /// </summary>
    public RateLimit GetLimit(HttpContext context)
    {
        // For composite rules, we'll take the most restrictive limit
        // by finding the rule with the lowest requests-per-second rate
        
        var matchingRules = _rules.Where(rule => rule.IsMatch(context)).ToList();
        
        if (!matchingRules.Any())
        {
            // If no rules match, return a default permissive limit
            return new RateLimit { MaxRequests = int.MaxValue, TimeWindowInSeconds = 1 };
        }
        
        // Calculate the most restrictive rate (lowest requests per second)
        var mostRestrictiveRule = matchingRules
            .Select(rule => new
            {
                Rule = rule,
                Limit = rule.GetLimit(context),
                RequestsPerSecond = (double)rule.GetLimit(context).MaxRequests / rule.GetLimit(context).TimeWindowInSeconds
            })
            .OrderBy(x => x.RequestsPerSecond)
            .First();
            
        return mostRestrictiveRule.Limit;
    }

    /// <summary>
    /// Evaluates all child rules according to the composite mode.
    /// </summary>
    public async Task<RateLimitResult> EvaluateAsync(HttpContext context, ClientIdentifier clientIdentifier)
    {
        try
        {
            // Get all matching rules
            var matchingRules = _rules.Where(rule => rule.IsMatch(context)).ToList();
            
            if (!matchingRules.Any())
            {
                _logger.LogDebug("No rules in composite rule {RuleName} match the request", Name);
                
                // If no rules match, allow the request
                return new RateLimitResult
                {
                    IsAllowed = true,
                    Rule = Name
                };
            }
            
            // Evaluate all matching rules
            var results = new List<RateLimitResult>();
            
            foreach (var rule in matchingRules)
            {
                var result = await rule.EvaluateAsync(context, clientIdentifier);
                results.Add(result);
                
                _logger.LogDebug(
                    "Rule {ChildRule} evaluated in composite rule {RuleName}: {IsAllowed}",
                    result.Rule, Name, result.IsAllowed);
            }
            
            // Determine the final result based on the composite mode
            bool isAllowed;
            RateLimitResult mostRestrictiveResult;
            
            switch (_mode)
            {
                case CompositeMode.AllRules:
                    // All rules must allow the request
                    isAllowed = results.All(r => r.IsAllowed);
                    // Use the first failing rule's details, or the most restrictive if all pass
                    mostRestrictiveResult = results.FirstOrDefault(r => !r.IsAllowed) ?? 
                                          GetMostRestrictiveResult(results);
                    break;
                    
                case CompositeMode.AnyRule:
                    // At least one rule must allow the request
                    isAllowed = results.Any(r => r.IsAllowed);
                    // Use the first passing rule's details, or the most restrictive if all fail
                    mostRestrictiveResult = isAllowed ? 
                                         results.First(r => r.IsAllowed) : 
                                         GetMostRestrictiveResult(results);
                    break;
                    
                case CompositeMode.MostRestrictive:
                    // Use the most restrictive rule's result
                    mostRestrictiveResult = GetMostRestrictiveResult(results);
                    isAllowed = mostRestrictiveResult.IsAllowed;
                    break;
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(_mode), "Unsupported composite mode");
            }
            
            _logger.LogInformation(
                "Composite rule {RuleName} evaluation result: {IsAllowed}. Mode: {Mode}",
                Name, isAllowed, _mode);
            
            // Return a new result that combines the evaluation
            return new RateLimitResult
            {
                IsAllowed = isAllowed,
                Rule = Name,
                Counter = mostRestrictiveResult.Counter,
                Limit = mostRestrictiveResult.Limit,
                TimeWindowInSeconds = mostRestrictiveResult.TimeWindowInSeconds,
                ResetAfter = mostRestrictiveResult.ResetAfter,
                // Capture information about which subrule determined the result
                Message = $"Determined by rule: {mostRestrictiveResult.Rule}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating composite rate limit rule {RuleName}", Name);
            
            // Fail open - allow the request if there's an error evaluating the limit
            return new RateLimitResult 
            { 
                IsAllowed = true,
                Rule = Name
            };
        }
    }
    
    /// <summary>
    /// Gets the most restrictive result from a list of rate limit results.
    /// </summary>
    private RateLimitResult GetMostRestrictiveResult(List<RateLimitResult> results)
    {
        // Define what "most restrictive" means:
        // 1. First, any result that blocks the request
        // 2. If all allow, then the one with the highest counter-to-limit ratio
        
        var blockingResults = results.Where(r => !r.IsAllowed).ToList();
        
        if (blockingResults.Any())
        {
            // Find the blocking result with the longest reset time
            return blockingResults
                .OrderByDescending(r => r.ResetAfter)
                .First();
        }
        
        // If all results allow the request, find the one closest to its limit
        return results
            .OrderByDescending(r => (double)r.Counter / r.Limit)
            .First();
    }
}
