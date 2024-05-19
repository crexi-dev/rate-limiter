using RateLimiter.Models;
using RateLimiter.Rules;
using RateLimiter.RuleTemplates;

namespace RateLimiter.Rules.RequestConverters
{
    public interface IRequestConverter
    {
        RuleRequestInfo Convert(Request request, RuleTemplateParams templateParams);
    }
}