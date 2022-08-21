using RateLimiter.Rules;

namespace RateLimiter.Configurators
{
    // The purpose of this class is to mimic IOC container
    public sealed class RuleAConfigurator
    {
        private static readonly RuleA? Instance = null;

        public static RuleA GetRule(int period, int allowedRequests) =>
            Instance ?? RuleA.Configure(period, allowedRequests);
    }
}