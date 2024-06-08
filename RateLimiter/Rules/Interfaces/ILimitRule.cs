namespace RateLimiter.Rules.Interfaces
{
    public interface ILimitRule
    {
        bool IsRequestAllowed(string clientId, string ruleName);
    }
}
