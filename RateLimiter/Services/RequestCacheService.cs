using System;
using Microsoft.Extensions.Caching.Memory;

public class RequestCacheService : IRequestCacheService
{
    private readonly IMemoryCache _memoryCache;

    public RequestCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public T GetData<T>(string key)
    {
        var item = (T)_memoryCache.Get(key);
        return item;
    }

    public void SetData<T>(string key, T value, TimeSpan expirationTime)
    {
        if (!_memoryCache.TryGetValue(key, out T item))
        {
            item = value;
            _memoryCache.Set(key, item, expirationTime);
        }
    }
}