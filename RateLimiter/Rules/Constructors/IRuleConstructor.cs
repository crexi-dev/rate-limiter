using RateLimiter.Rules;
using RateLimiter.RuleTemplates;

namespace RateLimiter.Rules.Constructors
{
    public interface IRuleConstructor
    {
        IRule Construct(RuleTemplateParams templateParams);
    }
}