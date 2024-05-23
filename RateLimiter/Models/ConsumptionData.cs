using System;

namespace RateLimiter.Models;

/// <summary>
/// Data for tracking client's request consumption.
/// </summary>
public class ConsumptionData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConsumptionData"/> class.
    /// </summary>
    /// <param name="lastResponse">The time of the last response.</param>
    /// <param name="numberOfRequests">The number of requests made.</param>
    public ConsumptionData(DateTime lastResponse, int numberOfRequests)
    {
        LastResponse = lastResponse;
        NumberOfRequests = numberOfRequests;
    }

    /// <summary>
    /// Gets the time of the last response.
    /// </summary>
    public DateTime LastResponse { get; private set; }

    /// <summary>
    /// Gets the number of requests made.
    /// </summary>
    public int NumberOfRequests { get; private set; }

    /// <summary>
    /// Checks if all requests have been consumed within the given time window.
    /// </summary>
    /// <param name="timeWindowInSeconds">The time window in seconds.</param>
    /// <param name="maxRequests">The maximum number of requests allowed.</param>
    /// <returns>true if all requests have been consumed; otherwise, false.</returns>
    public bool HasConsumedAllRequests(int timeWindowInSeconds, int maxRequests)
        => DateTime.UtcNow < LastResponse.AddSeconds(timeWindowInSeconds) && NumberOfRequests == maxRequests;

    /// <summary>
    /// Increases the number of requests made.
    /// </summary>
    /// <param name="maxRequests">The maximum number of requests allowed.</param>
    public void IncreaseRequests(int maxRequests)
    {
        LastResponse = DateTime.UtcNow;

        if (NumberOfRequests == maxRequests)
            NumberOfRequests = 1;
        else
            NumberOfRequests++;
    }
}