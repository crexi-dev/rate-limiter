using RateLimiter.Guards;
using RateLimiter.Models;
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
        Guard.RequestInfoType<TimeSpanFromTheLastCallRuleInfo>(requestInfo);
        var info = (TimeSpanFromTheLastCallRuleInfo)requestInfo;
        return _expectedTimeSpan < info.ActualTimeSpan;
    }
}