using RateLimiter.Guards;
using RateLimiter.Models;
using RateLimiter.Rules.Info;
using System;

namespace RateLimiter.Rules;
public class RegionBasedRule : Rule, IRule
{
    private readonly string _region;
    
    private IRule _innerRule;

    public RegionBasedRule(string region, IRule innerRule)
    {
        _region = region;
        _innerRule = innerRule;
    }

    public override bool Validate(RuleRequestInfo? requestInfo)
    {
        var info = (RegionBasedRuleInfo?)requestInfo ?? throw new ArgumentNullException(nameof(requestInfo));
        if (info.Region != _region)
        {
            return true;
        }

        
        return _innerRule.Validate(info?.InnerRuleInfo);
    }

}
