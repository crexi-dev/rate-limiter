using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RateLimiter.Common.Abstractions.Counters;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Common.Models;
using RateLimiter.Core.Services.KeyBuilders;

namespace RateLimiter.Core.Rules;

/// <summary>
/// Implements a region-based rate limit rule.
/// </summary>
public class RegionBasedRule : IRateLimitRule
{
    private readonly ILogger<RegionBasedRule> _logger;
    private readonly IKeyBuilder _keyBuilder;
    private readonly IRateLimitCounter _counter;
    private readonly RateLimit _rateLimit;
    private readonly string _targetRegion;
    private readonly int _minTimeBetweenRequests;
    private readonly string _lastRequestTimeKeySuffix = ":lastReq";
    private readonly Func<HttpContext, bool>? _matcher;

    public string Name { get; }

    public RegionBasedRule(
        string name,
        string targetRegion,
        RateLimit rateLimit,
        IKeyBuilder keyBuilder,
        IRateLimitCounter counter,
        ILogger<RegionBasedRule> logger,
        int minTimeBetweenRequests = 0,
        Func<HttpContext, bool>? matcher = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _targetRegion = targetRegion ?? throw new ArgumentNullException(nameof(targetRegion));
        _rateLimit = rateLimit ?? throw new ArgumentNullException(nameof(rateLimit));
        _keyBuilder = keyBuilder ?? throw new ArgumentNullException(nameof(keyBuilder));
        _counter = counter ?? throw new ArgumentNullException(nameof(counter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _minTimeBetweenRequests = minTimeBetweenRequests;
        _matcher = matcher;
    }

    /// <summary>
    /// Determines if this rule applies to the given HTTP context.
    /// </summary>
    public bool IsMatch(HttpContext context)
    {
        if (_matcher != null && !_matcher(context))
        {
            return false;
        }
        
        return true;
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
            // Skip this rule if the region doesn't match
            if (!string.IsNullOrEmpty(clientIdentifier.Region) && 
                !clientIdentifier.Region.Equals(_targetRegion, StringComparison.OrdinalIgnoreCase))
            {
                return new RateLimitResult
                {
                    IsAllowed = true,
                    Rule = Name,
                    Message = $"Rule skipped: client region '{clientIdentifier.Region}' doesn't match target region '{_targetRegion}'"
                };
            }
            
            string key = _keyBuilder.BuildKey(context, this, clientIdentifier);
            
            // Check minimum time between requests if configured
            if (_minTimeBetweenRequests > 0)
            {
                string lastRequestTimeKey = $"{key}{_lastRequestTimeKeySuffix}";
                long lastRequestTime = await _counter.GetCountAsync(lastRequestTimeKey);
                
                if (lastRequestTime > 0)
                {
                    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    var elapsedMs = now - lastRequestTime;
                    
                    if (elapsedMs < _minTimeBetweenRequests)
                    {
                        _logger.LogInformation(
                            "Rate limit exceeded for rule {RuleName}. Minimum time between requests not met. Elapsed: {ElapsedMs}ms, Required: {RequiredMs}ms",
                            Name, elapsedMs, _minTimeBetweenRequests);
                        
                        var waitTimeMs = _minTimeBetweenRequests - elapsedMs;
                        return new RateLimitResult
                        {
                            IsAllowed = false,
                            Rule = Name,
                            Message = $"Minimum time between requests not met for region {_targetRegion}",
                            ResetAfter = TimeSpan.FromMilliseconds(waitTimeMs)
                        };
                    }
                }
                
                // Update last request time
                await _counter.SetCountAsync(
                    lastRequestTimeKey,
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    TimeSpan.FromHours(24));
            }
            
            // Continue with standard fixed window rate limiting
            long currentCount = await _counter.GetCountAsync(key);
            
            if (currentCount >= _rateLimit.MaxRequests)
            {
                _logger.LogInformation(
                    "Rate limit exceeded for rule {RuleName}. Current count: {CurrentCount}, Limit: {Limit}, Region: {Region}",
                    Name, currentCount, _rateLimit.MaxRequests, _targetRegion);
                
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
                    ResetAfter = resetAfter,
                    Message = $"Rate limit exceeded for region {_targetRegion}"
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
            _logger.LogError(ex, "Error evaluating region-based rate limit for rule {RuleName}", Name);
            
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