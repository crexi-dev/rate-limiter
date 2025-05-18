using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RateLimiter.Common.Abstractions.Counters;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Common.Models;
using RateLimiter.Core.Services.KeyBuilders;

namespace RateLimiter.Core.Rules;

/// <summary>
/// Implements a token bucket algorithm for rate limiting.
/// </summary>
public class TokenBucketRule : IRateLimitRule
{
    private readonly ILogger<TokenBucketRule> _logger;
    private readonly IKeyBuilder _keyBuilder;
    private readonly IRateLimitCounter _counter;
    private readonly string _ruleName;
    private readonly int _bucketCapacity;
    private readonly double _refillRate; // tokens per second
    private readonly Func<HttpContext, bool>? _matcher;
    private readonly string _lastRefillTimeKeySuffix = ":lastRefill";
    private readonly string _availableTokensKeySuffix = ":tokens";

    public string Name => _ruleName;

    public TokenBucketRule(
        string ruleName,
        int bucketCapacity,
        double refillRatePerSecond,
        IKeyBuilder keyBuilder,
        IRateLimitCounter counter,
        ILogger<TokenBucketRule> logger,
        Func<HttpContext, bool>? matcher = null)
    {
        _ruleName = ruleName ?? throw new ArgumentNullException(nameof(ruleName));
        _bucketCapacity = bucketCapacity > 0 ? bucketCapacity : throw new ArgumentException("Bucket capacity must be greater than zero", nameof(bucketCapacity));
        _refillRate = refillRatePerSecond > 0 ? refillRatePerSecond : throw new ArgumentException("Refill rate must be greater than zero", nameof(refillRatePerSecond));
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
        // For token bucket, we represent rate limit as:
        // - MaxRequests: bucket capacity (maximum burst)
        // - TimeWindowInSeconds: time to refill the entire bucket (capacity / refill rate)
        int timeWindow = (int)Math.Ceiling(_bucketCapacity / _refillRate);
        
        return new RateLimit
        {
            MaxRequests = _bucketCapacity,
            TimeWindowInSeconds = timeWindow
        };
    }

    /// <summary>
    /// Evaluates if the request is within rate limits according to the token bucket algorithm.
    /// </summary>
    public async Task<RateLimitResult> EvaluateAsync(HttpContext context, ClientIdentifier clientIdentifier)
    {
        try
        {
            string baseKey = _keyBuilder.BuildKey(context, this, clientIdentifier);
            string lastRefillKey = $"{baseKey}{_lastRefillTimeKeySuffix}";
            string tokensKey = $"{baseKey}{_availableTokensKeySuffix}";
            
            DateTimeOffset now = DateTimeOffset.UtcNow;
            long nowUnix = now.ToUnixTimeMilliseconds();
            
            // Get the last refill time and available tokens
            long lastRefillTime = await _counter.GetCountAsync(lastRefillKey);
            long availableTokens = await _counter.GetCountAsync(tokensKey);
            
            // Initialize if these values aren't in the store yet
            if (lastRefillTime == 0)
            {
                lastRefillTime = nowUnix;
                availableTokens = _bucketCapacity;
                
                // Store the initial values with a long expiry
                await _counter.SetCountAsync(
                    lastRefillKey, 
                    lastRefillTime, 
                    TimeSpan.FromHours(24));
                    
                await _counter.SetCountAsync(
                    tokensKey, 
                    availableTokens, 
                    TimeSpan.FromHours(24));
            }
            else
            {
                // Calculate how many tokens to add based on time elapsed
                double elapsedSeconds = (nowUnix - lastRefillTime) / 1000.0;
                long tokensToAdd = (long)Math.Floor(elapsedSeconds * _refillRate);
                
                if (tokensToAdd > 0)
                {
                    // Update last refill time
                    await _counter.SetCountAsync(
                        lastRefillKey, 
                        nowUnix, 
                        TimeSpan.FromHours(24));
                    
                    // Add new tokens, but don't exceed capacity
                    availableTokens = Math.Min(_bucketCapacity, availableTokens + tokensToAdd);
                    await _counter.SetCountAsync(
                        tokensKey, 
                        availableTokens, 
                        TimeSpan.FromHours(24));
                }
            }
            
            // Check if we have enough tokens for this request
            if (availableTokens < 1)
            {
                _logger.LogInformation(
                    "Rate limit exceeded for rule {RuleName}. No tokens available in the bucket.",
                    Name);
                
                // Calculate time until next token is available
                double secondsUntilNextToken = 1.0 / _refillRate;
                
                return new RateLimitResult
                {
                    IsAllowed = false,
                    Rule = Name,
                    Counter = _bucketCapacity - availableTokens,
                    Limit = _bucketCapacity,
                    TimeWindowInSeconds = GetLimit(context).TimeWindowInSeconds,
                    ResetAfter = TimeSpan.FromSeconds(secondsUntilNextToken)
                };
            }
            
            // Consume a token
            await _counter.DecrementAsync(tokensKey, 1);
            
            // Calculate time to refill the bucket completely
            double secondsToRefill = (_bucketCapacity - (availableTokens - 1)) / _refillRate;
            
            return new RateLimitResult
            {
                IsAllowed = true,
                Rule = Name,
                Counter = _bucketCapacity - (availableTokens - 1),
                Limit = _bucketCapacity,
                TimeWindowInSeconds = GetLimit(context).TimeWindowInSeconds,
                ResetAfter = TimeSpan.FromSeconds(secondsToRefill)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating token bucket rate limit for rule {RuleName}", Name);
            
            // Fail open - allow the request if there's an error evaluating the limit
            return new RateLimitResult 
            { 
                IsAllowed = true,
                Rule = Name,
                Limit = _bucketCapacity,
                TimeWindowInSeconds = GetLimit(context).TimeWindowInSeconds
            };
        }
    }
}
