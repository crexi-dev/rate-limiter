using RateLimiter.Library;

namespace RateLimiter.RulesEngine.Library
{
    public interface IRulesEvaluator
    {
        RateLimitSettingsConfig Evaluate(string resourceName, string IPAddress);
    }
}
