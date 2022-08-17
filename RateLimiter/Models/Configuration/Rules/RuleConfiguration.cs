using RateLimiter.Models.Rules;

namespace RateLimiter.Models.Configuration.Rules
{
    public abstract class RuleConfiguration
    {
        public RuleType Type { get; set; }
    }
}
