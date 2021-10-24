namespace RateLimiter.Domain.ApiLimiter
{
    public interface IRule
    {
        bool NewVisitAndRuleCheck();
    }
}