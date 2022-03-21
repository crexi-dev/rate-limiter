using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Core.Models.RateLimit;

namespace RateLimiter.API.Store;

public interface IRateLimitCounterStore
{
    public Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);

    public Task<RateLimitCounter?> GetAsync(string id, CancellationToken cancellationToken = default);
    
    public Task RemoveAsync(string id, CancellationToken cancellationToken = default);

    public Task SetAsync(string id, RateLimitCounter? entry, TimeSpan? expirationTime = null,
        CancellationToken cancellationToken = default);
}