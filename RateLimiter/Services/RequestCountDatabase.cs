using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using RateLimiter.Interfaces;
using RateLimiter.Models;

namespace RateLimiter.Services;

public class RequestCountDatabase {
    private readonly ConcurrentDictionary<string, IEnumerable<IClientRequest>> Store;

    public RequestCountDatabase() {
        Store = new ConcurrentDictionary<string, IEnumerable<IClientRequest>>();
    }

    public void Update(
        string userToken,
        IEnumerable<IClientRequest> newVale,
        IEnumerable<IClientRequest> comparisonValue) {
        Store.TryUpdate(userToken, newVale, comparisonValue);
    }

    public void Add(string userToken, IEnumerable<IClientRequest> value) {
        foreach (var valuee in value) {
            Console.WriteLine(valuee.ResourceAccessed + "before");
        }

        Store.TryAdd(userToken, value);
        foreach (var valuee in value) {
            Console.WriteLine(valuee.ResourceAccessed + "after");
        }
    }

    public IEnumerable<IClientRequest>? Get(string userToken) {
        Store.TryGetValue(userToken, out var value);

        return value;
    }
}