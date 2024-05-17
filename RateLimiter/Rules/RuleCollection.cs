using System.Collections;
using System.Collections.Generic;
using RateLimiter.Models;

namespace RateLimiter.Rules;
public class RuleCollection : IRuleCollection
{
    private readonly IEnumerable<IRule> _rules;

    public RuleCollection(IEnumerable<IRule> rules){
        _rules = rules;
    }
    public bool Validate(Request request)
    {
        foreach (Rule rule in _rules)
        {
            if (!rule.Validate(request))
            {
                return false;
            }
        }
        
        return true;
       
    }
}