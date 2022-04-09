using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Interfaces
{
    public interface ICacheService
    {
        public T Get<T>(string key);

        public void Set<T>(string key, T Value);
    }
}
