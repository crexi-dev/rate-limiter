namespace RateLimiter.Models.Rules
{
    public enum RuleType
    {
        None = 0,
        RequestsPerTimeSpanRule = 1,
        TimeSpanSinceLastRequestRule = 2,
        TimeSpanSinceLastSuccessfulRequestRule = 3
    }
}
