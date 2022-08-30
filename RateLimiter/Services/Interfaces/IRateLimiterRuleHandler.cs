namespace RateLimiter.Services.Interfaces
{
    public interface IRateLimiterRuleHandler
    {
        void Handle(string token);
    }
}
