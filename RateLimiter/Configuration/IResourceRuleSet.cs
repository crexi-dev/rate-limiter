namespace RateLimiter.Configuration
{
    public interface IResourceRuleSet
    {
        IResourceRuleSet AddRule(IRule rule);
    }
}