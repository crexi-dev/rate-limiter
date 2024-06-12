using RateLimiter.Interfaces;
using System;
using System.Linq;

namespace RateLimiter.Rules;
public class RegionBasedCustomLimiterRule : IRule
{
    private IRule? _euRule;
    private IRule? _usRule;
    public RegionBasedCustomLimiterRule Configure(IRule euRule, IRule usRule)
    {
        _euRule = euRule;
        _usRule = usRule;
        return this;
    }

    public bool IsRequestAllowed(string token)
    {
        if (_euRule is null || _usRule is null)
        {
            throw new ArgumentException($"Call {nameof(Configure)} method to configure this rule first");
        }


        var region = GetRegionFromToken(token);
        return region switch
        {
            Region.US => _usRule.IsRequestAllowed(token),
            Region.EU => _euRule.IsRequestAllowed(token),
            Region.Unknown => true,
            _ => throw new NotImplementedException(),
        };
    }

    private static Region GetRegionFromToken(string token)
    {
        var regionPart = token.Split(':').FirstOrDefault();

        if (Enum.TryParse<Region>(regionPart, out var region))
        {
            return region;
        }
        return Region.Unknown;
    }
}

public enum Region
{
    Unknown,
    US,
    EU
}
