using RateLimiter.Rules;
using RateLimiter.Rules.RequestConverters;
using RateLimiter.RuleTemplates;

namespace RateLimiter.Repositories
{
    public record ValidateReadyRule (IRule Rule, IRequestConverter RequestConverter, RuleTemplateParams TemplateParams);
    
}