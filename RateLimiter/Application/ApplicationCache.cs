using System;
using System.Runtime.Caching;
using RateLimiter.Application.Interfaces;

namespace RateLimiter.Application
{
    public class ApplicationCache: ICache, IDisposable
    {
        private readonly string _name;
        private MemoryCache _memoryCache;
        private bool _disposed;

        public ApplicationCache(string name)
        {
            _name = name;
            _memoryCache = new MemoryCache(name);
        }

        public bool TryGet<T>(string key, out T? value)
        {
            var objValue = Get(key);

            if (objValue == null)
            {
                value = default;
                return false;
            }

            value = (T)objValue;
            return true;
        }

        public object? Get(string key)
        {
            return _memoryCache.Get(key);
        }

        public T Get<T>(string key)
        {
            var value = Get(key);

            if (value == null)
            {
                throw new Exception($"Value wasn't found in cache by key: {key}");
            }

            return (T)value;
        }

        public void SetWithAbsoluteExpiration(string key, object value, TimeSpan absoluteExpiration)
        {
            ThrowIfDisposed();

            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(absoluteExpiration.TotalSeconds)
            };

            _memoryCache.Set(key, value, policy);
        }

        public void SetWithSlidingExpiration(string key, object value, TimeSpan slidingExpiration)
        {
            ThrowIfDisposed();

            var policy = new CacheItemPolicy
            {
                SlidingExpiration = slidingExpiration
            };

            _memoryCache.Set(key, value, policy);
        }

        public void Clean()
        {
            _memoryCache.Dispose();
            _memoryCache = new MemoryCache(_name);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ApplicationCache));
            }
        }

        public void Dispose()
        {
            _memoryCache.Dispose();
            _disposed = true;
        }
    }
}
