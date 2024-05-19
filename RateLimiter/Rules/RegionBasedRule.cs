using GuardNet;
using RateLimiter.Rules.Info;

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
        Guard.NotNull(requestInfo, nameof(requestInfo));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        Guard.For(() => requestInfo.GetType().IsAssignableTo(typeof(RegionBasedRuleInfo)), new InvalidRuleInfoTypeException());
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        var info = (RegionBasedRuleInfo?)requestInfo;
        if (info.Region != _region)
        {
            return true;
        }

        
        return _innerRule.Validate(info?.InnerRuleInfo);
    }

}
