using System;
using System.Collections.Generic;

namespace RateLimiter.Rules;

/// <summary>
/// Defines a rule for rate limiting requests.
/// </summary>
public interface IRateLimitRule
{
    /// <summary>
    /// Determines whether a request is allowed based on the provided timestamps and request time.
    /// </summary>
    /// <param name="requestLogs">A list of timestamps representing previous requests.</param>
    /// <param name="timeOfRequest">The time of the current request.</param>
    /// <returns>True if the request is allowed; otherwise, false.</returns>
    bool IsRequestAllowed(List<DateTime> requestLogs, DateTime timeOfRequest);
}