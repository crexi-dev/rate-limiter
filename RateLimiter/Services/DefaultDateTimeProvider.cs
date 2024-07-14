using System;
using RateLimiter.Services.Interfaces;

namespace RateLimiter.Services;

internal sealed class DefaultDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
