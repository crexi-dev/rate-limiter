using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using RateLimiter.Interfaces;

namespace RateLimiter.Implementations;

public class InMemoryStore : IStore
{
    private readonly ConcurrentDictionary<string, object?> _store = new();
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public async Task SetAsync<T>(string key, T? data, CancellationToken ct)
    {
        _store[key] = data;
        await Task.CompletedTask;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct)
    {
        if (_store.TryGetValue(key, out var value) && value is T typedValue)
        {
            return await Task.FromResult(typedValue);
        }

        return await Task.FromResult(default(T));
    }

    public async Task<IAsyncDisposable> LockAsync(string key, CancellationToken ct)
    {
        var semaphore = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(ct);
        return new Releaser(_locks, key, semaphore);
    }

    private class Releaser(ConcurrentDictionary<string, SemaphoreSlim> locks, string key, SemaphoreSlim semaphore)
        : IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            semaphore.Release();
            if (semaphore.CurrentCount == 1)
            {
                locks.TryRemove(key, out _);
            }
            await Task.CompletedTask;
        }
    }
}