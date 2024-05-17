using RateLimiter.Exceptions;
using RateLimiter.RuleTemplates;
using RateLimiter.RuleTemplates.Params;

namespace RateLimiter.Rules.Constructors
{
    public class RequestByTimeSpanRuleConstructor : IRuleConstructor
    {
        public IRule Construct(RuleTemplateParams templateParams)
        {

            if (templateParams is not RequestByTimeSpanRuleTemplateParams validParams)
            {
                throw new InvalidTemplateParamsException();

            }
            return new RequestByTimeSpanRule(validParams.RequestLimit);
        }
    }
}