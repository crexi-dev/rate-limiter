using RateLimiter.Rules.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.ConfigurationStorageProvider
{
    public class ConfigurationProvider(IConfigurationStorageProvider<IRateLimiterRule> storageProvider)
    {
        private readonly IConfigurationStorageProvider<IRateLimiterRule> _storageProvider = storageProvider;

        public async Task<bool> SaveConfigAsync(string key, List<IRateLimiterRule> values)
        {
            return await _storageProvider.SaveAsync(key, values);
        }

        public async Task<List<IRateLimiterRule>?> GetConfigAsync(string key)
        {
            return await _storageProvider.LoadAsync(key);
        }
    }
}
