using RateLimiter.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    /// <summary>
    /// A simple implementation of ICachingService that is suitable for testing purposes. It stores data in local memory 
    /// and is not thread-safe. 
    /// </summary>
    internal class TestCachingService : ICachingService
    {
        private Dictionary<string, object> _cache = new Dictionary<string, object>();

        public Task<T> UpdateValue<T>(string key, Func<T, T> updateFunction, T defaultValue, CancellationToken cancellationToken)
        {
            T currentValue = _cache.TryGetValue(key, out var value) ? (T)value : defaultValue;
            T newValue = updateFunction(currentValue);
            _cache[key] = newValue;
            return Task.FromResult(newValue);
        }
    }
}
