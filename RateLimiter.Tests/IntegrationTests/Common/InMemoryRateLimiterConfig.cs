using RateLimiter.Rules.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

public class InMemoryConfigurationStorage : IConfigurationStorageProvider<IRateLimiterRule>
{
    private readonly Dictionary<string, List<IRateLimiterRule>> _rulesConfig;

    public InMemoryConfigurationStorage()
    {
        _rulesConfig = new Dictionary<string, List<IRateLimiterRule>>();
    }

    public async Task<List<IRateLimiterRule>?> LoadAsync(string endpoint)
    {
        _rulesConfig.TryGetValue(endpoint, out var rules);

        return await Task.FromResult(rules ?? new List<IRateLimiterRule>());
    }

    public async Task<bool> SaveAsync(string key, List<IRateLimiterRule> rules)
    {
        _rulesConfig[key] = rules;

        return await Task.FromResult(true);
    }
}
