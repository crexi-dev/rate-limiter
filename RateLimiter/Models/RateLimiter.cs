using RateLimiter.Contracts;
using System;

namespace RateLimiter.Models
{
    public partial class RateLimiter : IRateLimiter
    {
        private readonly IRepository _repository;

        public RateLimiter(IRepository repository)
        {
            _repository = repository;
        }

        public bool Validate(Guid token, DateTime utcTime, ResourcesType resource)
        {
            (var client, var rules) = _repository.GetClientWithRules(token, resource);

            foreach (IRequestLimitRule rule in rules)
            {
                (int count, DateTime lastCallTime) = _repository.GetClientLimitInfo(token, utcTime, rule.Time, resource);

                if (!CheckRule(rule, client, count, utcTime, lastCallTime))
                {
                    return false;
                }
            }

            _repository.Incremet(token, utcTime, resource);

            return true;
        }

        private bool CheckRule(IRequestLimitRule rule, IClient client, int count, DateTime utcTime, DateTime lastCallTime)
        {
            if (rule.RegionId == null || rule.RegionId == client.RegionId)
            {
                if ((rule.RuleType & (byte)RequestLimitRuleType.RequestsPerTime) > 0)
                {
                    if (rule.CountLimit <= count)
                    {
                        return false;
                    }
                }

                if ((rule.RuleType & (byte)RequestLimitRuleType.TiemAfterLastCall) > 0)
                {
                    if (utcTime.Subtract(rule.Time) <= lastCallTime)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
