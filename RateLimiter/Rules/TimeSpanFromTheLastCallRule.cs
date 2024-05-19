using GuardNet;
using RateLimiter.CustomGuards;
using RateLimiter.Rules;

public class TimeSpanFromTheLastCallRule : IRule
{
    private int _expectedTimeSpan;

    public TimeSpanFromTheLastCallRule(int expectedTimeSpan)
    {
        _expectedTimeSpan = expectedTimeSpan;
    }

    public bool Validate(RuleRequestInfo? requestInfo)
    {
        Guard.NotNull(requestInfo, nameof(requestInfo));
        CustomGuard.IsValidRuleRequestInfoType<TimeSpanFromTheLastCallRuleInfo>(requestInfo.GetType());

        var info = (TimeSpanFromTheLastCallRuleInfo)requestInfo;
        return _expectedTimeSpan < info.ActualTimeSpan;
    }
}