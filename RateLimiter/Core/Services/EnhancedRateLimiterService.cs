using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RateLimiter.Common.Abstractions;
using RateLimiter.Common.Abstractions.Counters;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Common.Models;

namespace RateLimiter.Core.Services;

/// <summary>
/// Enhanced rate limiter service that respects configuration precedence
/// </summary>
public class EnhancedRateLimiterService : IRateLimiterService
{
    private readonly IRateLimitRuleProvider _ruleProvider;
    private readonly IRateLimitClientIdentifierProvider _clientIdentifierProvider;
    private readonly IRateLimitCounter _counter;
    private readonly ILogger<EnhancedRateLimiterService> _logger;

    public EnhancedRateLimiterService(
        IRateLimitRuleProvider ruleProvider,
        IRateLimitClientIdentifierProvider clientIdentifierProvider,
        IRateLimitCounter counter,
        ILogger<EnhancedRateLimiterService> logger)
    {
        _ruleProvider = ruleProvider ?? throw new ArgumentNullException(nameof(ruleProvider));
        _clientIdentifierProvider = clientIdentifierProvider ?? throw new ArgumentNullException(nameof(clientIdentifierProvider));
        _counter = counter ?? throw new ArgumentNullException(nameof(counter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Evaluates if a request should be allowed with enhanced rule precedence logic
    /// </summary>
    public async Task<RateLimitResult> EvaluateRequestAsync(HttpContext context)
    {
        try
        {
            // Get all rules that match the current context (already resolved by HybridRuleProvider)
            var matchingRules = await _ruleProvider.GetMatchingRulesAsync(context);
            
            if (!matchingRules.Any())
            {
                _logger.LogDebug("No rate limit rules match the request. Allowing request for {Path}", context.Request.Path);
                
                return new RateLimitResult
                {
                    IsAllowed = true,
                    Rule = "NoMatchingRules"
                };
            }
            
            // Get client identifier
            var clientIdentifier = await _clientIdentifierProvider.GetClientIdentifierAsync(context);
            
            // Evaluate rules in the order provided by HybridRuleProvider (already precedence-sorted)
            var results = new List<RateLimitResult>();
            
            foreach (var rule in matchingRules)
            {
                var result = await rule.EvaluateAsync(context, clientIdentifier);
                results.Add(result);
                
                _logger.LogDebug(
                    "Rule {Rule} evaluated for {Path}: {IsAllowed}. Counter: {Counter}, Limit: {Limit}",
                    result.Rule, context.Request.Path, result.IsAllowed, result.Counter, result.Limit);
                
                // If any rule denies the request, return that result immediately
                if (!result.IsAllowed)
                {
                    _logger.LogInformation(
                        "Request blocked by rate limit rule {Rule} for {Path}. Counter: {Counter}, Limit: {Limit}",
                        result.Rule, context.Request.Path, result.Counter, result.Limit);
                        
                    return result;
                }
            }
            
            // All rules allowed the request, return the first rule's result (highest precedence)
            // This ensures configuration rules are preferred in headers/reporting
            var primaryResult = results.First();
                
            _logger.LogDebug(
                "Request allowed for {Path}. Primary rule: {Rule}, Counter: {Counter}, Limit: {Limit}",
                context.Request.Path, primaryResult.Rule, primaryResult.Counter, primaryResult.Limit);
                
            return primaryResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating rate limits for {Path}", context.Request.Path);
            
            // Fail open - allow the request if there's an error
            return new RateLimitResult
            {
                IsAllowed = true,
                Rule = "ErrorEvaluating",
                Message = "An error occurred while evaluating rate limits"
            };
        }
    }

    /// <summary>
    /// Resets rate limits for a client.
    /// </summary>
    public async Task ResetLimitsAsync(string clientId)
    {
        _logger.LogInformation("Resetting rate limits for client {ClientId}", clientId);
        
        // Reset all counters for this client
        await _counter.ResetAsync(clientId);
    }
}
