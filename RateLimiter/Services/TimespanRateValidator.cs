using System;
using RateLimiter.Models;

namespace RateLimiter.Services;

public class TimespanRateValidator : IRateLimitValidator
{
    private readonly TimeSpan _timeSpan;

    public TimespanRateValidator(TimeSpan timeSpan)
    {
        _timeSpan = timeSpan;
    }

    public RateLimitValidationResult Validate(ClientData data)
    {
        var result = DateTime.UtcNow.Subtract(data.LastVisit) > _timeSpan;
        return new RateLimitValidationResult(result);
    }
}