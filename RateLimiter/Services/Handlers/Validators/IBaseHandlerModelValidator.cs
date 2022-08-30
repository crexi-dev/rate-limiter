using RateLimiter.Models.Rules;
using RateLimiter.Services.Handlers.Models;

namespace RateLimiter.Services.Handlers.Validators
{
    public interface IBaseHandlerModelValidator
    {
        // I would use FluentValidator in real scenario
        void Validate<TRule>(BaseHandlerModel<TRule> model)
            where TRule : RateLimiterRuleBase;
    }
}
