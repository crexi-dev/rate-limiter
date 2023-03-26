using System;

namespace RateLimiter;

/// <summary>
/// Registration info of accepted tokens
/// </summary>
public class UserRegInfo
{
    /// <summary>
    /// Token 
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Count of accepted requests during a period
    /// </summary>
    public int ConnectionsCount { get; set; }

    /// <summary>
    /// A time when the period started
    /// </summary>
    public DateTime PeriodStartDate { get; set; }
}