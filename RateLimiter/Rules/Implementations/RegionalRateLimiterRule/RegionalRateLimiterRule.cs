using RateLimiter.Core;
using RateLimiter.Rules.Interfaces;
using System.Threading.Tasks;

namespace RateLimiter.Rules.Implementations.RegionalRateLimiterRule
{
    public class RegionalRateLimiterRule(IRateLimiterRule usRule, IRateLimiterRule euRule) : IRateLimiterRule
    {
        private readonly IRateLimiterRule _usRule = usRule;
        private readonly IRateLimiterRule _euRule = euRule;

        public async Task<bool> IsRequestAllowedAsync(ClientRequestContext context)
        {
            return await Task.FromResult(true);
        }
    }
}
