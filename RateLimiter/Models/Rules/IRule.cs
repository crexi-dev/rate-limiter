namespace RateLimiter.Models.Rules
{
    public interface IRule
    {
        RuleResult Execute(СlientStatistics сlientStatistics);
    }
}
