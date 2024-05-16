using System.Collections;

namespace RateLimiter.Rules;
public class RuleCollection
{
    private readonly Rule[] _rules;

    public RuleCollection(Rule[] rules){
        _rules = rules;
    }
    public bool ValidateRules(Request request)
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