using System;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using RateLimiter.Models;
using RateLimiter.Services.Interfaces;

namespace RateLimiter.Services;

internal sealed class DistributedCacheRateLimitStorageService : IRateLimitStorageService
{
    private readonly IDistributedCache _memoryCache;

    public DistributedCacheRateLimitStorageService(IDistributedCache memoryCache) => _memoryCache = memoryCache;

    public void Set(string id, RateLimitCounter counter, TimeSpan expirationTime)
        => _memoryCache.SetString(id, JsonSerializer.Serialize(counter), new DistributedCacheEntryOptions().SetAbsoluteExpiration(expirationTime));

    public bool Exists(string id) => !string.IsNullOrEmpty(_memoryCache.GetString(id));

    public RateLimitCounter? Get(string id)
    {
        var stored = _memoryCache.GetString(id);
        return !string.IsNullOrEmpty(stored)
            ? JsonSerializer.Deserialize<RateLimitCounter>(stored)
            : null;
    }

    public void Remove(string id) => _memoryCache.Remove(id);
}
