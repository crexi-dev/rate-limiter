using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RateLimiter.Stores.Interfaces;
using RateLimiter.Stores.Models;

namespace RateLimiter.Stores;

public class InMemoryStore : IRateLimitStore
{
    private readonly ConcurrentDictionary<string, ConcurrentStack<RequestRateModel>> _store;

    public InMemoryStore()
    {
        _store = new ConcurrentDictionary<string, ConcurrentStack<RequestRateModel>>();
    }

    public Task Add(string resource)
    {
        var newRequest = new RequestRateModel { RequestTicks = DateTime.Now.Ticks };
        var count = _store.GetOrAdd(resource, _ =>
        {
            var queue = new ConcurrentStack<RequestRateModel>();
            queue.Push(newRequest);
            return queue;
        });
        count.Push(newRequest);

        return Task.CompletedTask;
    }

    public Task<List<RequestRateModel>> Get(string resource)
    {
        _store.TryGetValue(resource, out var data);
        return Task.FromResult(data?.ToList() ?? new List<RequestRateModel>());
    }

    public Task<RequestRateModel?> GetLast(string resource)
    {
        _store.TryGetValue(resource, out var data);
        RequestRateModel? last = null;
        data?.TryPeek(out last);
        return Task.FromResult(last);
    }
}