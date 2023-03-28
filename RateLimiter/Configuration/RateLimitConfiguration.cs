using System;

namespace RateLimiter;

public enum RuleType
{
    RequestCount = 1,
    LastUsage = 2,
}


public abstract class RateLimitConfiguration
{
    public string Url;
    public string HttpMethod;
    public string Regions;
    public virtual RuleType Type { get; }
}

public class RequestCountConfiguration : RateLimitConfiguration
{
    public int Count;
    public TimeSpan Duration;
    public override RuleType Type => RuleType.RequestCount;
}

public class LastUsageConfiguration : RateLimitConfiguration
{
    public TimeSpan DelayTime;

    public override RuleType Type => RuleType.LastUsage;

}