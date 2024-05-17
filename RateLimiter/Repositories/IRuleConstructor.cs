using RateLimiter.Rules;
using RateLimiter.RuleTemplates;

namespace RateLimiter.Repositories
{
    public interface IRuleConstructor
    {
        IRule Construct(RuleTemplateParams templateParams);
    }
}