using RateLimiter.Rules;
using RateLimiter.RuleTemplates;

namespace RateLimiter.Repositories
{
    public record ValidateReadyRule (IRule Rule, IRequestConverter RequestConverter, RuleTemplateParams TemplateParams);
    
}