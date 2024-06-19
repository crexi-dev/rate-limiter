using System;
using RateLimiter.Models;

namespace RateLimiter.Services;

public class WindowLimitRateValidator : IRateLimitValidator
{
    private readonly TimeSpan _timeSpan;
    private readonly int _requestLimit;

    public WindowLimitRateValidator(
        TimeSpan timeSpan,
        int requestLimit)
    {
        _timeSpan = timeSpan;
        _requestLimit = requestLimit;
    }

    public RateLimitValidationResult Validate(ClientData data)
    {
        var dateDiff = DateTime.UtcNow.Subtract(data.LastVisit);
        if (dateDiff > _timeSpan)
        {
            return new RateLimitValidationResult(true, 1);
        }

        if (dateDiff <= _timeSpan && data.VisitCounts >= _requestLimit)
        {
            return new RateLimitValidationResult(false, data.VisitCounts);
        }

        return new RateLimitValidationResult(true, data.VisitCounts + 1);
    }
}