using System;
using System.Collections.Generic;

namespace RateLimiter.Rules;

/// <summary>
/// A rule that limits the number of requests within a specified time window.
/// </summary>
public class RequestPerTimeSpanRule : IRateLimitRule
{
    private readonly int _requestLimit;
    private readonly TimeSpan _window;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestPerTimeSpanRule"/> class.
    /// </summary>
    /// <param name="requestLimit">The maximum number of requests allowed within the time window.</param>
    /// <param name="window">The time window for the rate limit.</param>
    public RequestPerTimeSpanRule(int requestLimit, TimeSpan window)
    {
        _requestLimit = requestLimit;
        _window = window;
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

        lock (requestLogs)
        {   
            requestLogs.RemoveAll(timestamp => timestamp < timeOfRequest - _window);

            return requestLogs.Count < _requestLimit;
        }
    }
}