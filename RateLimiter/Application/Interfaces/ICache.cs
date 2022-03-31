using System;

namespace RateLimiter.Application.Interfaces
{
    public interface ICache
    {
        bool TryGet<T>(string key, out T? value);

        T Get<T>(string key);

        object? Get(string key);

        void SetWithAbsoluteExpiration(string key, object value, TimeSpan absoluteExpiration);

        void SetWithSlidingExpiration(string key, object value, TimeSpan slidingExpiration);

        void Clean();
    }
}
