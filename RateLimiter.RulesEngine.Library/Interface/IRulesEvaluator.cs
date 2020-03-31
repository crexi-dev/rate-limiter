namespace RateLimiter.RulesEngine.Library
{
    public interface IRulesEvaluator
    {
        bool EvaluateResourceRule(string resourceName);
        bool EvaluateIPAddressRule(string ipAddress);
    }
}
