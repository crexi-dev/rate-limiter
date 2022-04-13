using System;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Interfaces
{
    /// <summary>
    /// This service is used to store information in a cache. In a production environment, a concrete implementation of 
    /// this cache should be: a) Thread-safe, b) Shared across multiple servers, c) configured to expire a key after it 
    /// has been unused for sufficiently long. In practice, we could probably create a service that works with Redis
    /// </summary>
    public interface ICachingService
    {
        /// <summary>
        /// Update the cache entry for a given key by calling the updateFunction and return the new value. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="updateFunction">A function that takes in the current value of the cache entry (or 
        /// defaultValue if it does not exist) and returns the new value. If the cache cannot be updated, 
        /// throw a <see cref="CacheNotUpdatedException"/></param>
        /// <param name="defaultValue"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The new value of the cached item</returns>
        Task<T> UpdateValue<T>(string key, Func<T, T> updateFunction, T defaultValue, CancellationToken cancellationToken);
    }
}
