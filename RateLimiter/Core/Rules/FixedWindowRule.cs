using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RateLimiter.Common.Abstractions.Counters;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Common.Models;
using RateLimiter.Core.Services.KeyBuilders;

namespace RateLimiter.Core.Rules;

/// <summary>
/// Implements a fixed window rate limit rule.
/// </summary>
public class FixedWindowRule : IRateLimitRule
{
    private readonly ILogger<FixedWindowRule> _logger;
    private readonly IKeyBuilder _keyBuilder;
    private readonly IRateLimitCounter _counter;
    private readonly RateLimit _rateLimit;
    private readonly Func<HttpContext, bool>? _matcher;

    public string Name { get; }

    public FixedWindowRule(
        string name,
        RateLimit rateLimit,
        IKeyBuilder keyBuilder,
        IRateLimitCounter counter,
        ILogger<FixedWindowRule> logger,
        Func<HttpContext, bool>? matcher = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _rateLimit = rateLimit ?? throw new ArgumentNullException(nameof(rateLimit));
        _keyBuilder = keyBuilder ?? throw new ArgumentNullException(nameof(keyBuilder));
        _counter = counter ?? throw new ArgumentNullException(nameof(counter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _matcher = matcher;
    }

    /// <summary>
    /// Determines if this rule applies to the given HTTP context.
    /// </summary>
    public bool IsMatch(HttpContext context)
    {
        return _matcher?.Invoke(context) ?? true;
    }

    /// <summary>
    /// Gets the applicable rate limit for this rule.
    /// </summary>
    public RateLimit GetLimit(HttpContext context)
    {
        return _rateLimit;
    }

    /// <summary>
    /// Evaluates if the request is within rate limits.
    /// </summary>
    public async Task<RateLimitResult> EvaluateAsync(HttpContext context, ClientIdentifier clientIdentifier)
    {
        try
        {
            string key = _keyBuilder.BuildKey(context, this, clientIdentifier);
            
            // Get the current count
            long currentCount = await _counter.GetCountAsync(key);
            
            if (currentCount >= _rateLimit.MaxRequests)
            {
                _logger.LogInformation(
                    "Rate limit exceeded for rule {RuleName}. Current count: {CurrentCount}, Limit: {Limit}",
                    Name, currentCount, _rateLimit.MaxRequests);
                
                // Calculate reset time
                var windowEnd = GetWindowEnd();
                var resetAfter = windowEnd - DateTimeOffset.UtcNow;
                
                return new RateLimitResult
                {
                    IsAllowed = false,
                    Rule = Name,
                    Counter = currentCount,
                    Limit = _rateLimit.MaxRequests,
                    TimeWindowInSeconds = _rateLimit.TimeWindowInSeconds,
                    ResetAfter = resetAfter
                };
            }
            
            // Increment the counter
            await _counter.IncrementAsync(
                key, 
                1, 
                TimeSpan.FromSeconds(_rateLimit.TimeWindowInSeconds));
            
            return new RateLimitResult
            {
                IsAllowed = true,
                Rule = Name,
                Counter = currentCount + 1,
                Limit = _rateLimit.MaxRequests,
                TimeWindowInSeconds = _rateLimit.TimeWindowInSeconds,
                ResetAfter = GetWindowEnd() - DateTimeOffset.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating fixed window rate limit for rule {RuleName}", Name);
            
            // Fail open - allow the request if there's an error evaluating the limit
            return new RateLimitResult 
            { 
                IsAllowed = true,
                Rule = Name,
                Limit = _rateLimit.MaxRequests,
                TimeWindowInSeconds = _rateLimit.TimeWindowInSeconds
            };
        }
    }
    
    /// <summary>
    /// Gets the end time of the current window.
    /// </summary>
    private DateTimeOffset GetWindowEnd()
    {
        var now = DateTimeOffset.UtcNow;
        var windowSizeSeconds = _rateLimit.TimeWindowInSeconds;
        
        // Calculate the window start by truncating to window size
        long unixTime = now.ToUnixTimeSeconds();
        long windowStart = unixTime - (unixTime % windowSizeSeconds);
        
        // Window end is window start + window size
        return DateTimeOffset.FromUnixTimeSeconds(windowStart + windowSizeSeconds);
    }
}
