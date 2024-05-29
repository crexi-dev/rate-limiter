using RateLimiter.Interfaces;
using System;

namespace RateLimiter.Tests.Helpers;

public class MockDateTimeWrapper : IDateTimeWrapper
{
    public MockDateTimeWrapper()
    {
        UtcNow = DateTime.UtcNow;
    }

    public DateTime UtcNow { get; set; }
}
