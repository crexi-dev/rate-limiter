namespace RateLimiter.Rules.Info;

internal class RegionBasedRuleInfo : RuleRequestInfo
{
    public string? Region { get; internal set; }
    public RuleRequestInfo? InnerRuleInfo { get; internal set; }
}