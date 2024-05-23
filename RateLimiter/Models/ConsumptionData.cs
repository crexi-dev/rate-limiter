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

    private DateTime LastResponse { get; set; }
    private int NumberOfRequests { get; set; }

    public bool HasConsumedAllRequests(int timeWindowInSeconds, int maxRequests)
        => DateTime.UtcNow < LastResponse.AddSeconds(timeWindowInSeconds) && NumberOfRequests == maxRequests;

    public void IncreaseRequests(int maxRequests)
    {
        LastResponse = DateTime.UtcNow;

        if (NumberOfRequests == maxRequests)
            NumberOfRequests = 1;

        else
            NumberOfRequests++;
    }
}