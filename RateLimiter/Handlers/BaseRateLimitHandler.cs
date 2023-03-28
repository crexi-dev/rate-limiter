using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using RateLimiter.Interfaces;

namespace RateLimiter.Handlers;

public abstract class BaseRateLimitHandler<TStat, TRule> : IRateLimitHandler 
    where TStat : IClientStatistics 
    where TRule : IRateLimitRule
{
    protected TStat Statistics;
    protected List<TRule> Rules;
    protected ITimeProvider TimeProvider;

    protected BaseRateLimitHandler(TStat statistics) : this(statistics, new List<TRule>())
    {
        Statistics = statistics;
    }
    
    protected BaseRateLimitHandler(TStat statistics, List<TRule> rules) : this(statistics, new List<TRule>(), new TimeProvider())
    {
        Statistics = statistics;
        Rules = rules;
    }

    protected BaseRateLimitHandler(TStat statistics, List<TRule> rules, ITimeProvider timeProvider)
    {
        Statistics = statistics;
        Rules = rules;
        TimeProvider = timeProvider;
    }

    public virtual void AddRule(TRule rule)
    {
        Rules.Add(rule);
    }

    public void HandleRequestForUser(RequestInformation request)
    {
        foreach (var rule in Rules.Where(r => r.IsMatched(request)))
        {
            var statId = rule.GetStatisticsId(request);
            CalculateRuleFor(rule, statId);
            Statistics.IncrementStatisticsForRequest(statId, TimeProvider);
        }
        UpdateStatisticFor(request);
    }

    protected void StopExecution()
    {
        throw new HttpRequestException("Limit of Request is reached.", null, HttpStatusCode.TooManyRequests);
    }

    protected abstract void CalculateRuleFor(TRule rule, IBucketIdentifier identifier);

    protected virtual void UpdateStatisticFor(RequestInformation request)
    {
        
    }
}