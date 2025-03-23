using RateLimiter.Rules.Implementations.RequestPerPeriodRule;
using System;
using System.Collections.Concurrent;

namespace RateLimiter.Rules.Implementations.RequestPerPeriodRule;
public class InMemoryRequestPerPeriodRuleRepository : IRequestPerPeriodRuleRepository
{
    private readonly ConcurrentDictionary<string, (DateTime start, int count)> _requests = new();

    public (DateTime start, int count) GetOrAdd(string key, DateTime start, int counter)
    {
        return _requests.GetOrAdd(key, (start, counter));
    }

    public void Update(string key, DateTime start, int counter)
    {
        _requests[key] = (start, counter);
    }
}
