using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Models;
using RateLimiter.Repositories.Interfaces;

namespace RateLimiter.Repositories;

public class RateLimitRepository(IMemoryCache memoryCache)
    : IRateLimitRepository
{
    private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

    public async Task AddAsync(RateLimitRequestModel model, TimeSpan? expirationTime = null)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.AccessToken))
        {
            throw new ArgumentNullException("Incorrect input model");
        }

        var cacheKey = $"{model.AccessToken}_{model.Path}";
        var items = await GetAsync(cacheKey);
        items.Add(model.DateTime, model);
        await SetCacheAsync(cacheKey, items, expirationTime);
    }

    private Task SetCacheAsync(string cacheKey, Dictionary<DateTime, RateLimitRequestModel> items, TimeSpan? expirationTime = null)
    {
        var options = new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.NeverRemove
        };

        if (expirationTime.HasValue)
        {
            options.SetAbsoluteExpiration(expirationTime.Value);
        }

        _memoryCache.Set(cacheKey, items, options);

        return Task.CompletedTask;
    }

    public async Task<Dictionary<DateTime, RateLimitRequestModel>> GetAsync(string cacheKey)
    {
        return await Task.FromResult(
            _memoryCache.TryGetValue(cacheKey, out Dictionary<DateTime, RateLimitRequestModel> items)
                ? items
                : []);
    }
}