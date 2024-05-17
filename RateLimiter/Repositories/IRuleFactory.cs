using RateLimiter.Rules;

namespace RateLimiter.Repositories
{
    internal interface IRuleFactory
    {
        IRule Create(RuleValue value);
    }
}