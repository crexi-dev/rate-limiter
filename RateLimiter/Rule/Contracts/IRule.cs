using RateLimiter.Models;

namespace RateLimiter.Rule.Contracts
{
    public interface IRule
    {
        public bool ValidateRuleByStatistic(ClientStatistic statistic);
    }
}