using System;
using System.Collections.Generic;

namespace RateLimiter.Rules;

public interface IRateLimitRuleOptions
{
    string Name { get; set; }
    List<Func<AccessToken, bool>>? RuleConditions { get; set; }
}
