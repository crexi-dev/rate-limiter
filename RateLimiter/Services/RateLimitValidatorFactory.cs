using System;
using System.Linq;
using Microsoft.Extensions.Options;
using RateLimiter.Models;

namespace RateLimiter.Services;

public interface IRateLimitValidatorFactory
{
    IRateLimitValidator Create(string? regionKey);
}

public class RateLimitValidatorFactory : IRateLimitValidatorFactory
{
    private readonly RateLimitSettings _settings;

    public RateLimitValidatorFactory(IOptions<RateLimitSettings> settings)
    {
        _settings = settings.Value;
    }

    public IRateLimitValidator Create(string? regionKey)
    {
        var regionRule = _settings.Regions.FirstOrDefault(x => x.RegionKey == regionKey);
        if (regionRule == null)
        {
            return Create(_settings.CommonRule);
        }

        return Create(regionRule);
    }

    private IRateLimitValidator Create(RateLimitRule rule)
    {
        switch (rule.Type)
        {
            case RateLimitRuleType.Timespan:
                if (rule.WindowSizeInSeconds == null)
                {
                    throw new ArgumentNullException(nameof(rule), "Timespan value isn't set");
                }

                return new TimespanRateValidator(TimeSpan.FromSeconds(rule.WindowSizeInSeconds.Value));
            case RateLimitRuleType.RequestsPerTimespan:
                if (rule.WindowSizeInSeconds == null)
                {
                    throw new ArgumentNullException(nameof(rule), "Timespan value isn't set");
                }
                if (rule.PermitLimit == null)
                {
                    throw new ArgumentNullException(nameof(rule), "Request limit value isn't set");
                }

                return new WindowLimitRateValidator(
                    TimeSpan.FromSeconds(rule.WindowSizeInSeconds.Value),
                    rule.PermitLimit.Value);
            default:
                throw new ArgumentOutOfRangeException(nameof(rule),
                    $"Validator for rule type {rule.Type} isn't implemented");
        }
    }
}