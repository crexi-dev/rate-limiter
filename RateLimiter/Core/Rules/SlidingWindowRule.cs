using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RateLimiter.Common.Abstractions.Counters;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Common.Models;
using RateLimiter.Core.Services.KeyBuilders;

namespace RateLimiter.Core.Rules;

/// <summary>
/// Implements a sliding window rate limit rule.
/// </summary>
public class SlidingWindowRule : IRateLimitRule
{
    private readonly ILogger<SlidingWindowRule> _logger;
    private readonly IKeyBuilder _keyBuilder;
    private readonly IRateLimitCounter _counter;
    private readonly RateLimit _rateLimit;
    private readonly Func<HttpContext, bool>? _matcher;

    public string Name { get; }

    public SlidingWindowRule(
        string name,
        RateLimit rateLimit,
        IKeyBuilder keyBuilder,
        IRateLimitCounter counter,
        ILogger<SlidingWindowRule> logger,
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
    /// Evaluates if the request is within rate limits according to the sliding window algorithm.
    /// </summary>
    public async Task<RateLimitResult> EvaluateAsync(HttpContext context, ClientIdentifier clientIdentifier)
    {
        try
        {
            string key = _keyBuilder.BuildKey(context, this, clientIdentifier);
            
            // Get the current window timestamp (rounded to window size)
            var now = DateTimeOffset.UtcNow;
            var currentWindowStart = now - TimeSpan.FromSeconds(now.Second % _rateLimit.TimeWindowInSeconds);
            var previousWindowStart = currentWindowStart.AddSeconds(-_rateLimit.TimeWindowInSeconds);
            
            // Calculate how far we are into the current window (0 to 1)
            double currentWindowElapsedRatio = (now - currentWindowStart).TotalSeconds / _rateLimit.TimeWindowInSeconds;
            
            // Generate keys for current and previous windows
            string currentWindowKey = $"{key}:{currentWindowStart.ToUnixTimeSeconds()}";
            string previousWindowKey = $"{key}:{previousWindowStart.ToUnixTimeSeconds()}";
            
            // Get the counts for both windows
            long currentWindowCount = await _counter.GetCountAsync(currentWindowKey);
            long previousWindowCount = await _counter.GetCountAsync(previousWindowKey);
            
            // Calculate the sliding window count
            // weight = (1 - currentWindowElapsedRatio) → previous window weight
            double slidingCount = previousWindowCount * (1 - currentWindowElapsedRatio) + currentWindowCount;
            
            if (Math.Ceiling(slidingCount) >= _rateLimit.MaxRequests)
            {
                _logger.LogInformation(
                    "Rate limit exceeded for rule {RuleName}. Current count: {CurrentCount}, Previous count: {PreviousCount}, Sliding count: {SlidingCount}, Limit: {Limit}",
                    Name, currentWindowCount, previousWindowCount, slidingCount, _rateLimit.MaxRequests);
                
                return new RateLimitResult
                {
                    IsAllowed = false,
                    Rule = Name,
                    Counter = (long)Math.Ceiling(slidingCount),
                    Limit = _rateLimit.MaxRequests,
                    TimeWindowInSeconds = _rateLimit.TimeWindowInSeconds,
                    ResetAfter = CalculateResetTime(currentWindowStart, now)
                };
            }
            
            // Increment the current window counter
            await _counter.IncrementAsync(
                currentWindowKey, 
                1, 
                TimeSpan.FromSeconds(_rateLimit.TimeWindowInSeconds * 2)); // Double the expiry to keep previous window
            
            return new RateLimitResult
            {
                IsAllowed = true,
                Rule = Name,
                Counter = (long)Math.Ceiling(slidingCount) + 1, // Add 1 for the current request
                Limit = _rateLimit.MaxRequests,
                TimeWindowInSeconds = _rateLimit.TimeWindowInSeconds,
                ResetAfter = CalculateResetTime(currentWindowStart, now)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating sliding window rate limit for rule {RuleName}", Name);
            
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
    /// Calculates the time remaining until the rate limit resets.
    /// </summary>
    private TimeSpan CalculateResetTime(DateTimeOffset windowStart, DateTimeOffset now)
    {
        // Calculate when the current window ends
        DateTimeOffset windowEnd = windowStart.AddSeconds(_rateLimit.TimeWindowInSeconds);
        
        // Return the time remaining in the current window
        return windowEnd - now;
    }
}
