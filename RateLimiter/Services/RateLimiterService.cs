using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Rules;

namespace RateLimiter.Services;

/// <summary>
/// Service for rate limiting requests based on predefined rules.
/// </summary>
public class RateLimiterService : IRateLimiterService
{
    private readonly ConcurrentDictionary<string, List<DateTime>> _requestLogs = new();
    private readonly Dictionary<string, List<IRateLimitRule>> _rules = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimiterService"/> class.
    /// </summary>
    /// <param name="rules">A dictionary where the key is the endpoint, and the value is a list of rate limit rules that apply to that endpoint.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="rules"/> parameter is null.</exception>
    public RateLimiterService(Dictionary<string, List<IRateLimitRule>> rules)
    {
        _rules = rules ?? throw new ArgumentNullException(nameof(rules));
    }

    /// <summary>
    /// Determines whether a request is allowed based on the endpoint and token.
    /// </summary>
    /// <param name="endpoint">The endpoint being accessed.</param>
    /// <param name="token">The token representing the client making the request.</param>
    /// <returns>True if the request is allowed; otherwise, false.</returns>
    public bool IsRequestAllowed(string endpoint, string token)
    {
        List<IRateLimitRule> rules = new();
        if (!_rules.TryGetValue(endpoint, out rules))
        {
            return true;
        }

        if (rules.Count == 0)
        {
            return true;
        }

        string key = $"{endpoint}:{token}";
        DateTime now = DateTime.UtcNow;
        List<DateTime> logs = _requestLogs.GetOrAdd(key, _ => new List<DateTime>());

        bool isAllowed;
        lock (logs)
        {
            isAllowed = rules.All(rule => rule.IsRequestAllowed(logs, now));
            if (isAllowed)
            {
                logs.Add(now);
            }
        }

        return isAllowed;
    }
}