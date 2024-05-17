using RateLimiter.Rules;

internal class TimeSpanFromTheLastCallRuleInfo : RuleRequestInfo
{
    public TimeSpanFromTheLastCallRuleInfo(int actualTimeSpan)
    {
        ActualTimeSpan = actualTimeSpan;
    }

    public int ActualTimeSpan { get; init; }
}