using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Rules;

/// <summary>
/// A rule that enforces a minimum time span between consecutive requests.
/// </summary>
public class TimeSpanSinceLastRequestRule : IRateLimitRule
{
    private readonly TimeSpan _timeBetweenRequests;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeSpanSinceLastRequestRule"/> class.
    /// </summary>
    /// <param name="timeBetweenRequests">The minimum time span required between consecutive requests.</param>
    public TimeSpanSinceLastRequestRule(TimeSpan timeBetweenRequests)
    {
        _timeBetweenRequests = timeBetweenRequests;
    }

    /// <summary>
    /// Determines whether a request is allowed based on the provided timestamps and request time.
    /// </summary>
    /// <param name="requestLogs">A list of timestamps representing previous requests.</param>
    /// <param name="timeOfRequest">The time of the current request.</param>
    /// <returns>True if the request is allowed; otherwise, false.</returns>
    public bool IsRequestAllowed(List<DateTime> requestLogs, DateTime timeOfRequest)
    {
        if (requestLogs == null || requestLogs.Count == 0)
        {
            return true;
        }

        DateTime lastRequest = requestLogs.Max();

        return timeOfRequest - lastRequest > _timeBetweenRequests;
    }
}