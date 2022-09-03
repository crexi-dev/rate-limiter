using System.Collections.Generic;

namespace RateLimiter.Stores
{
    public interface ICacheProvider
    {
        T Get<T>(string key) where T : class;
        void Set<T>(string key, T value) where T : class;
    }
}
