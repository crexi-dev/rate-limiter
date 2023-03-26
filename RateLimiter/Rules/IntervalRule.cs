using System;

namespace RateLimiter.Rules;

public class IntervalRule : IRule
{
    /// <summary>
    /// Allowed count during the interval
    /// </summary>
    public int AllowedCount { get; set; }

    /// <summary>
    /// Interval of time in seconds
    /// </summary>
    public int Interval { get; set; }

    public IntervalRule(int allowedCount, int interval)
    {
        AllowedCount = allowedCount;
        Interval = interval;
    }

    public bool CheckAndUpdate(UserRegInfo userRegInfo)
    {
        if (userRegInfo.ConnectionsCount == 0)
        {
            userRegInfo.ConnectionsCount = 1;
            userRegInfo.PeriodStartDate = DateTime.Now;
            return true;
        }

        if (DateTime.Now - userRegInfo.PeriodStartDate < TimeSpan.FromSeconds(Interval))
        {
            if (userRegInfo.ConnectionsCount >= AllowedCount)
                return false;

            userRegInfo.ConnectionsCount++;
            return true;

        }

        userRegInfo.ConnectionsCount = 1;
        userRegInfo.PeriodStartDate = DateTime.Now;
        return true;

    }
}