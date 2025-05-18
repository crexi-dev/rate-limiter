using Microsoft.Extensions.Logging;
using RateLimiter.Common.Abstractions.Counters;
using StackExchange.Redis;

namespace RateLimiter.Infrastructure.Counters;

/// <summary>
/// Redis implementation of rate limit counter for distributed scenarios.
/// </summary>
public class RedisRateLimitCounter : IRateLimitCounter
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisRateLimitCounter> _logger;
    private readonly string _keyPrefix;

    public RedisRateLimitCounter(
        IConnectionMultiplexer redis,
        ILogger<RedisRateLimitCounter> logger,
        string keyPrefix = "ratelimit:")
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _keyPrefix = keyPrefix;
    }

    /// <summary>
    /// Gets the current count for a key.
    /// </summary>
    public async Task<long> GetCountAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }
        
        string redisKey = GetRedisKey(key);
        var db = _redis.GetDatabase();
        
        RedisValue value = await db.StringGetAsync(redisKey);
        return value.IsNull ? 0 : (long)value;
    }

    /// <summary>
    /// Sets the count for a key.
    /// </summary>
    public async Task SetCountAsync(string key, long count, TimeSpan expiry)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }
        
        if (count < 0)
        {
            throw new ArgumentException("Count cannot be negative", nameof(count));
        }
        
        string redisKey = GetRedisKey(key);
        var db = _redis.GetDatabase();
        
        await db.StringSetAsync(redisKey, count, expiry);
    }

    /// <summary>
    /// Increments the count for a key.
    /// </summary>
    public async Task IncrementAsync(string key, long value, TimeSpan expiry)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }
        
        if (value <= 0)
        {
            throw new ArgumentException("Value must be positive", nameof(value));
        }
        
        string redisKey = GetRedisKey(key);
        var db = _redis.GetDatabase();
        
        // Use Lua script for atomic increment and expire
        string script = @"
            local current = redis.call('incrby', KEYS[1], ARGV[1])
            redis.call('expire', KEYS[1], ARGV[2])
            return current";
        
        await db.ScriptEvaluateAsync(
            script,
            new RedisKey[] { redisKey },
            new RedisValue[] { value, (int)expiry.TotalSeconds });
    }

    /// <summary>
    /// Decrements the count for a key.
    /// </summary>
    public async Task DecrementAsync(string key, long value)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }
        
        if (value <= 0)
        {
            throw new ArgumentException("Value must be positive", nameof(value));
        }
        
        string redisKey = GetRedisKey(key);
        var db = _redis.GetDatabase();
        
        // Use Lua script for atomic decrement with minimum value of 0
        string script = @"
            local current = redis.call('get', KEYS[1])
            if not current then return 0 end
            
            local new_value = math.max(0, tonumber(current) - tonumber(ARGV[1]))
            redis.call('set', KEYS[1], new_value)
            
            -- Keep the existing TTL
            local ttl = redis.call('ttl', KEYS[1])
            if ttl > 0 then
                redis.call('expire', KEYS[1], ttl)
            end
            
            return new_value";
        
        await db.ScriptEvaluateAsync(
            script,
            new RedisKey[] { redisKey },
            new RedisValue[] { value });
    }
    
    /// <summary>
    /// Resets counters for a specific client.
    /// </summary>
    public async Task ResetAsync(string clientId)
    {
        if (string.IsNullOrEmpty(clientId))
        {
            throw new ArgumentException("Client ID cannot be null or empty", nameof(clientId));
        }
        
        _logger.LogInformation("Resetting rate limits for client {ClientId}", clientId);
        
        var server = GetServer();
        if (server == null)
        {
            _logger.LogWarning("No Redis server available for key pattern search");
            return;
        }
        
        // Find all keys for this client
        string pattern = $"{_keyPrefix}*{clientId}*";
        var keys = server.Keys(pattern: pattern).ToArray();
        
        if (keys.Length > 0)
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(keys);
            
            _logger.LogInformation("Deleted {KeyCount} rate limit keys for client {ClientId}", keys.Length, clientId);
        }
    }
    
    /// <summary>
    /// Gets a Redis key with the prefix.
    /// </summary>
    private string GetRedisKey(string key)
    {
        return $"{_keyPrefix}{key}";
    }
    
    /// <summary>
    /// Gets a Redis server for key pattern operations.
    /// </summary>
    private IServer? GetServer()
    {
        var endpoints = _redis.GetEndPoints();
        return endpoints.Length > 0 ? _redis.GetServer(endpoints[0]) : null;
    }
}
