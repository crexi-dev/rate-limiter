using RateLimiter.Contracts;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Models
{
    public partial class RateLimiter: IRateLimiter
    {
        public async Task<bool> ValidateAsync(Guid token, DateTime utcTime, ResourcesType resource)
        {
            (var client, var rules) = await _repository.GetClientWithRulesAsync(token, resource);

            foreach (var rule in rules)
            {
                (int count, DateTime lastCalltime) = await _repository.GetClientLimitInfoAsync(token, utcTime, rule.Time, resource);

                if (!CheckRule(rule, client, count, utcTime, lastCalltime))
                {
                    return false;
                }
            }

            await _repository.IncremetAsync(token, utcTime, resource);

            return true;

        }
    }
}
