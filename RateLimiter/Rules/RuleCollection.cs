using System;
using System.Collections;
using RateLimiter.Rules;

namespace RateLimiter.Rules;
public class RuleCollection : CollectionBase
{
    public RuleCollection()
    {
        
    }
    
    public bool ValidateRules(Request token)
    {
        foreach (Rule rule in this)
        {
            if (!rule.Validate(token))
            {
                return false;
            }
        }
        
        return true;
       
    }
}