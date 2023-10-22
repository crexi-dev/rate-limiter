using System;
using System.Linq;

namespace RateLimiter.Attributes;

public class RateRulesAttribute : Attribute
{
    public RateRulesAttribute(params string[] rules)
    {
        Rules = rules;
    }

    public RateRulesAttribute(params RateRulesEnum[] rules)
    {
        Rules = rules.Select(i=>i.ToString()).ToArray();
    }

    public string[] Rules { get; }
}