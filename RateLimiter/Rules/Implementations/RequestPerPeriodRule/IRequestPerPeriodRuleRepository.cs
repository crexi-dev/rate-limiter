using System;

namespace RateLimiter.Rules.Implementations.RequestPerPeriodRule
{
    public interface IRequestPerPeriodRuleRepository
    {
        (DateTime start, int count) GetOrAdd(string key, DateTime start, int counter);
        void Update(string key, DateTime start, int counter);
    }

}
