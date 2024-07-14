using System;

namespace RateLimiter.Services.Interfaces;

internal interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
