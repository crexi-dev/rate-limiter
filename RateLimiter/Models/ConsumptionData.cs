using System;

namespace RateLimiter.Models;

public class ConsumptionData
{
    public ConsumptionData(DateTime responseTime)
    {
        ResponseTime = responseTime;
    }

    public DateTime ResponseTime { get; private set; }
}