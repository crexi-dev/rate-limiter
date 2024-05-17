using RateLimiter.Models;
using RateLimiter.Rules;

public class TimeSpanFromTheLastCallRule : IRule
{
    private int _expectedTimeSpan;

    public TimeSpanFromTheLastCallRule(int expectedTimeSpan)
    {
        _expectedTimeSpan = expectedTimeSpan;
    }

    public bool Validate(Request request)
    {
        return false;
    }
}