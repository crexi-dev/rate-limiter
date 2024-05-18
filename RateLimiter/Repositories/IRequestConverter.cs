using RateLimiter.Models;
using RateLimiter.Rules;
using RateLimiter.RuleTemplates;

namespace RateLimiter.Repositories
{
    public interface IRequestConverter
    {
        RuleRequestInfo Convert(Request request, RuleTemplateParams templateParams);
    }
}