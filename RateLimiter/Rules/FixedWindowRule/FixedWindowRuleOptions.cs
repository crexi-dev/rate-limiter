using System;

namespace RateLimiter.Rules.FixedWindowRule;

public class FixedWindowRuleOptions : RateLimitRuleOptions
{
    public string Name { get; set; } = "FixedWindowRule";
    public int Limit { get; set; }
    public TimeSpan WindowSize { get; set; }
}
