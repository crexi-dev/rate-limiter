using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RateLimiter.Common.Abstractions.Counters;

namespace RateLimiter.Infrastructure.Counters;

/// <summary>
/// In-memory implementation of rate limit counter.
/// </summary>
public class MemoryRateLimitCounter : IRateLimitCounter
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MemoryRateLimitCounter> _logger;
    private readonly ConcurrentDictionary<string, object> _locks = new();
    private readonly ConcurrentDictionary<string, TimeSpan> _expiryTimes = new();
    private readonly ConcurrentDictionary<string, HashSet<string>> _clientKeys = new();

    public MemoryRateLimitCounter(
        IMemoryCache cache,
        ILogger<MemoryRateLimitCounter> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the current count for a key.
    /// </summary>
    public Task<long> GetCountAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }
        
        long count = _cache.TryGetValue<long>(key, out var value) ? value : 0;
        return Task.FromResult(count);
    }

    /// <summary>
    /// Sets the count for a key.
    /// </summary>
    public Task SetCountAsync(string key, long count, TimeSpan expiry)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }
        
        if (count < 0)
        {
            throw new ArgumentException("Count cannot be negative", nameof(count));
        }
        
        _cache.Set(key, count, expiry);
        
        // Store expiry time for later use
        _expiryTimes[key] = expiry;
        
        // Also track in locks dictionary for ResetAsync to work
        _locks.TryAdd(key, new object());
        
        // Track key by client ID for reset operations
        TrackKeyByClientId(key);
        
        _logger.LogDebug("Set count for key {Key} to {Count} with expiry {Expiry}s", key, count, expiry.TotalSeconds);
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Increments the count for a key.
    /// </summary>
    public Task IncrementAsync(string key, long value, TimeSpan expiry)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }
        
        if (value <= 0)
        {
            throw new ArgumentException("Value must be positive", nameof(value));
        }
        
        // Get or create lock for this key
        var lockObject = _locks.GetOrAdd(key, _ => new object());
        
        lock (lockObject)
        {
            long currentValue = _cache.TryGetValue<long>(key, out var cachedValue) ? cachedValue : 0;
            long newValue = currentValue + value;
            
            _logger.LogDebug("Incrementing key {Key} from {OldValue} to {NewValue}", key, currentValue, newValue);
            
            _cache.Set(key, newValue, expiry);
            
            // Store expiry time for later use
            _expiryTimes[key] = expiry;
            
            // Track key by client ID for reset operations
            TrackKeyByClientId(key);
        }
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Decrements the count for a key.
    /// </summary>
    public Task DecrementAsync(string key, long value)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }
        
        if (value <= 0)
        {
            throw new ArgumentException("Value must be positive", nameof(value));
        }
        
        // Get or create lock for this key
        var lockObject = _locks.GetOrAdd(key, _ => new object());
        
        lock (lockObject)
        {
            if (_cache.TryGetValue<long>(key, out var currentValue))
            {
                long newValue = Math.Max(0, currentValue - value);
                
                _logger.LogDebug("Decrementing key {Key} from {OldValue} to {NewValue}", key, currentValue, newValue);
                
                // Get the existing expiry or use a default
                TimeSpan timeSpan = _expiryTimes.TryGetValue(key, out var expiry) 
                    ? expiry 
                    : TimeSpan.FromHours(1); // Default expiry
                
                _cache.Set(key, newValue, timeSpan);
            }
        }
        
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Resets counters for a specific client.
    /// </summary>
    public Task ResetAsync(string clientId)
    {
        if (string.IsNullOrEmpty(clientId))
        {
            throw new ArgumentException("Client ID cannot be null or empty", nameof(clientId));
        }
        
        _logger.LogInformation("Resetting rate limits for client {ClientId}", clientId);
        
        // Try to get keys from our tracking dictionary first
        if (_clientKeys.TryGetValue(clientId, out var clientKeySet))
        {
            foreach (var key in clientKeySet.ToList())
            {
                _logger.LogDebug("Removing tracked key: {Key}", key);
                _cache.Remove(key);
                _locks.TryRemove(key, out _);
                _expiryTimes.TryRemove(key, out _);
            }
            
            // Clear the set of keys for this client
            clientKeySet.Clear();
        }
        
        // Also look for any keys that might not be in our tracking dictionary
        var keysFromMatching = _locks.Keys
            .Where(k => k.Contains(clientId))
            .ToList();
            
        foreach (var key in keysFromMatching)
        {
            if (!clientKeySet?.Contains(key) ?? true)
            {
                _logger.LogDebug("Removing additional key: {Key}", key);
                _cache.Remove(key);
                _locks.TryRemove(key, out _);
                _expiryTimes.TryRemove(key, out _);
            }
        }
        
        // Also check our expiry times dictionary for any remaining keys
        var keysFromExpiry = _expiryTimes.Keys
            .Where(k => k.Contains(clientId) && !keysFromMatching.Contains(k))
            .ToList();
            
        foreach (var key in keysFromExpiry)
        {
            _logger.LogDebug("Removing expiry-tracked key: {Key}", key);
            _cache.Remove(key);
            _locks.TryRemove(key, out _);
            _expiryTimes.TryRemove(key, out _);
        }
        
        _logger.LogInformation("Reset complete for client {ClientId}", clientId);
        
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Tracks a key by the client ID contained in it.
    /// </summary>
    private void TrackKeyByClientId(string key)
    {
        // Extract client ID from key (assumes format like "ruleName:clientId:something")
        var parts = key.Split(':');
        if (parts.Length < 2)
        {
            return;
        }
        
        string clientId = parts[1];
        if (string.IsNullOrEmpty(clientId))
        {
            return;
        }
        
        // Add to tracking dictionary
        var clientKeys = _clientKeys.GetOrAdd(clientId, _ => new HashSet<string>());
        clientKeys.Add(key);
    }
}
