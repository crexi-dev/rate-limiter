using System;

namespace RateLimiter.DataCaching
{
    public interface ICacheService
    {
        T GetData<T>(string key);

        void SetData<T>(string key, T value, TimeSpan expirationTime);
    }
}
