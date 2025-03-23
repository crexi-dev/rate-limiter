using RateLimiter.Rules;
using RateLimiter.Rules.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRateLimiterConfigRepository
{
    Task<List<IRateLimiterRule>?> GetRulesConfigByEndpointAsync(string endpoint);
}
