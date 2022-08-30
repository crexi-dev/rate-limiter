using System;
using RateLimiter.Models.Rules;
using RateLimiter.Services.Handlers.Models;

namespace RateLimiter.Services.Handlers.Validators
{
    public class BaseHandlerModelValidator : IBaseHandlerModelValidator
    {
        public void Validate<TRule>(BaseHandlerModel<TRule> model)
            where TRule : RateLimiterRuleBase
        {
            if (string.IsNullOrEmpty(model.Token?.Trim()))
            {
                throw new ArgumentException("Token is empty.");
            }
            if (model.Rule == null)
            {
                throw new ArgumentException("Rule is null.");
            }
            if (model.UserInformation == null)
            {
                throw new ArgumentException("UserInformation is null.");
            }
        }
    }
}
