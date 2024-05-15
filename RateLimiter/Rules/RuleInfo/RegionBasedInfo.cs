namespace RateLimiter.Rules.RuleInfo;
public class RegionBasedInfo<T> : Info where T : Info
{
    public string? Region { get; set; }
    public T InnerRuleInfo { get; set; } = null!;
}