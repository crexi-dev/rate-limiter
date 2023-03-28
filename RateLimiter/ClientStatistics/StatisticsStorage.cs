using System;
using System.Collections.Generic;
using RateLimiter.Interfaces;

namespace RateLimiter.ClientStatistics;

public class StatisticsStorage : IStatisticStorageProvider
{
    private readonly Dictionary<Type, IClientStatistics> _storage;

    public StatisticsStorage()
    {
        _storage = new Dictionary<Type, IClientStatistics>();
    }
    
    public void AddStorage<T>(Type type,T storage) where T : class, IClientStatistics
    {
        _storage[type] = storage;
    }

    public void AddStorage<T>(T storage) where T : class, IClientStatistics
    {
        AddStorage(typeof(T), storage);
    }

    public T GetStorage<T>() where T : class, IClientStatistics, new() 
    {
        var type = typeof(T);
        if (!_storage.TryGetValue(type, out var storage))
        {
            _storage[type] = storage = new T();
        }

        return storage as T ?? throw new NotSupportedException();
    }
}