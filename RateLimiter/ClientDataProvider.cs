using Core.Models;
using Microsoft.Extensions.Caching.Memory;

namespace RateLimiter
{
    public class ClientDataProvider
    {
        private readonly IMemoryCache cache_;

        public ClientDataProvider(IMemoryCache cache)
        {
            cache_ = cache;
        }

        public ClientData GetClientDataByKey(string key)
        {
            cache_.TryGetValue(key, out ClientData clientData);
            return clientData;
        }
    }
}
