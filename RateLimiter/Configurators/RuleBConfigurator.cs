using RateLimiter.Rules;

namespace RateLimiter.Configurators
{
    // The purpose of this class is to mimic IOC container
    public sealed class RuleBConfigurator
    {
        private static readonly RuleB? Instance = null;

        public static RuleB GetRule(int period) =>
            Instance ?? RuleB.Configure(period);
    }
}