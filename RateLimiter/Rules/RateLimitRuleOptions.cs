using System;
using System.Collections.Generic;

namespace RateLimiter.Rules;

public class RateLimitRuleOptions : IRateLimitRuleOptions
{
    public string Name { get; set; } = "";

    public List<Func<AccessToken, bool>>? RuleConditions { get; set; }
}
