using System;

namespace RateLimiter.Models;

public class ConsumptionData
{
    public ConsumptionData(
        DateTime lastResponse,
        int numberOfRequests)
    {
        LastResponse = lastResponse;
        NumberOfRequests = numberOfRequests;
    }

    public DateTime LastResponse { get; private set; }

    public int NumberOfRequests { get; private set; }

    public void IncreaseRequests(int maxRequests)
    {
        LastResponse = DateTime.UtcNow;

        if (NumberOfRequests == maxRequests)
            NumberOfRequests = 1;

        else
            NumberOfRequests++;
    }
}