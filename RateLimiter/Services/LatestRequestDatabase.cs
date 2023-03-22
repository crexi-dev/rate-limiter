using System;
using System.Collections.Concurrent;
using RateLimiter.Models;

namespace RateLimiter.Services;

public class LatestRequestDatabase {
    private readonly ConcurrentDictionary<ClientRequestKey, DateTime> Store;

    public LatestRequestDatabase() {
        Store = new ConcurrentDictionary<ClientRequestKey, DateTime>();
    }

    public void Update(
        ClientRequestKey key,
        DateTime newVale,
        DateTime oldValue) {
        Store.TryUpdate(key, newVale, oldValue);
    }

    public void Add(ClientRequestKey key, DateTime timeToAdd) {
        Store.TryAdd(key, timeToAdd);
    }

    public DateTime? Get(ClientRequestKey key) {
        if (Store.TryGetValue(key, out var value)) {
            return value;
        }

        return null;
    }
}