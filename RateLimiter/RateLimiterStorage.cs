using System.Collections.Concurrent;

namespace RateLimiter;

public class RateLimiterStorage : IRateLimiterStorage
{
    private readonly ConcurrentDictionary<string, ResourceEntry> _storage = new();
    
    public void AddOrUpdate(string key, Func<string, ResourceEntry> addValueFactory, Func<string, ResourceEntry, ResourceEntry> updateValueFactory)
    {
        _storage.AddOrUpdate(key, addValueFactory, updateValueFactory);
    }
}