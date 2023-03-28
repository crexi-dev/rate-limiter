using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using RateLimiter.Rules;

namespace RateLimiter;

public class Client
{
    public Guid Identifier { get; private set; }
    public RateLimitRule? RateLimitRules { get; private set; }

    public Client()
    {
        Identifier = Guid.NewGuid();
    }

    public void SetRateLimitRule(RateLimitRule rule)
    {
        if (RateLimitRules == default)
        {
            RateLimitRules = rule;
        }
        else
        {
            RateLimitRules.SetNextRule(rule);
        }
    }
}