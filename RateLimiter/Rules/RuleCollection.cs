using System.Collections.Generic;
using RateLimiter.Models;
using RateLimiter.Repositories;

namespace RateLimiter.Rules;
public class RuleCollection : IRuleCollection
{
    private readonly IEnumerable<ValidateReadyRule> _validateReadyRules;

    public RuleCollection(IEnumerable<ValidateReadyRule> validateReadyRules){
        _validateReadyRules = validateReadyRules;
    }
    public bool Validate(Request request)
    {
        foreach (ValidateReadyRule validateReadyRule in _validateReadyRules)
        {
            if (!validateReadyRule.Rule.Validate(validateReadyRule.RequestConverter.Convert(request, validateReadyRule.TemplateParams)))
            {
                return false;
            }
        }
        
        return true;
       
    }
}
