
using RateLimiter.Rules.RuleInfo;

public class TimeSpanFromTheLastCallRule : Rule<TimeSpanFromTheLastCallInfo>
{
    private int _expectedTimeSpan;

    public TimeSpanFromTheLastCallRule(int expectedTimeSpan)
    {
        _expectedTimeSpan = expectedTimeSpan;
    }

    public override bool Validate(TimeSpanFromTheLastCallInfo info)
    {
        return info.ActualTime > _expectedTimeSpan;
    }
}