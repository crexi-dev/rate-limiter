using RateLimiter.Rules.RuleInfo;

public class RegionBasedRule<T> : Rule<RegionBasedInfo<T>> where T : Info
{
    private readonly string _region;
    private readonly IRule<T> _rule;

    public RegionBasedRule(string region, IRule<T> rule)
    {
        _region = region;
        _rule = rule;
    }


    public override bool Validate(RegionBasedInfo<T> info)
    {
        if (info.Region != _region)
        {
            return true;
        }

        return _rule.Validate(info.InnerRuleInfo);
    }

}
