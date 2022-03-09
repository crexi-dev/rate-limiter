using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Cache
{
    /// <summary>
    ///  A wrapper class that extends methods for getting/setting data in IDistributedCache.
    /// </summary>
    public static class DistributedCache
    {
        /// <summary>
        ///  Gets an object from the cache with the specified key.
        /// </summary>
        public static async Task<T> Get<T>(this IDistributedCache distributedCache, string key,
            CancellationToken token = default) where T : class
        {
            var value = await distributedCache.GetStringAsync(key, token).ConfigureAwait(false);
            return string.IsNullOrEmpty(value) ? null : JsonSerializer.Deserialize<T>(value);
        }

        /// <summary>
        ///  Inserts an object in the cache with the specified key.
        /// </summary>
        public static async Task Insert<T>(this IDistributedCache distributedCache, string key, T value, TimeSpan expiration,
            CancellationToken token = default)
        {
            var json = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            await distributedCache.SetStringAsync(key, json, options, token);
        }
    }
}
