using RateLimiter.Rules;
using RateLimiter.Rules.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.ConfigurationStorageProvider.Implementations
{
    internal class RedisConfigurationStorage : IConfigurationStorageProvider<IRateLimiterRule>
    {
        public Task<List<IRateLimiterRule>?> LoadAsync(string endpoint)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> SaveAsync(string key, List<IRateLimiterRule> values)
        {
            throw new System.NotImplementedException();
        }
    }
}
