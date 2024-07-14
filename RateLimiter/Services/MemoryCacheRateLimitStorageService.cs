using System;
using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Models;
using RateLimiter.Services.Interfaces;

namespace RateLimiter.Services;

internal sealed class MemoryCacheRateLimitStorageService : IRateLimitStorageService
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheRateLimitStorageService(IMemoryCache memoryCache) => _memoryCache = memoryCache;

    public void Set(string id, RateLimitCounter counter, TimeSpan expirationTime)
        => _memoryCache.Set(id, counter, new MemoryCacheEntryOptions().SetAbsoluteExpiration(expirationTime));

    public bool Exists(string id) => _memoryCache.TryGetValue(id, out RateLimitCounter counter);

    public RateLimitCounter? Get(string id) => _memoryCache.TryGetValue(id, out RateLimitCounter counter) ? counter : null;

    public void Remove(string id) => _memoryCache.Remove(id);
}
