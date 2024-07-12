using System;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Interfaces;

public interface IStore
{
    Task SetAsync<T>(string key, T? data, CancellationToken ct);
    Task<T?> GetAsync<T>(string key, CancellationToken ct);
    Task<IAsyncDisposable> LockAsync(string key, CancellationToken ct);
}