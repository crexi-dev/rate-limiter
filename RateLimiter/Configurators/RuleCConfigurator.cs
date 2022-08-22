using RateLimiter.Rules;

namespace RateLimiter.Configurators
{
    // The purpose of this class is to mimic IOC container
    public sealed class RuleCConfigurator
    {
        private static readonly RuleC? Instance = null;

        public static RuleC GetRule(int usBasedPeriod, int usBasedAllowedNumberOfRequests, int euBasedPeriod) =>
            Instance ?? RuleC.Configure(usBasedPeriod, usBasedAllowedNumberOfRequests, euBasedPeriod);
    }
}