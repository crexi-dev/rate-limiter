using System;

namespace RateLimiter.Rules;

/// <summary>
/// Set a minimum delay between requests
/// </summary>
public class DelayRule : IRule
{
    /// <summary>
    /// Amount of time that needs to be passed after previous request
    /// </summary>
    public int Delay { get; set; }

    public DelayRule(int delay)
    {
        Delay = delay;
    }

    public bool CheckAndUpdate(UserRegInfo userRegInfo)
    {
        if (userRegInfo.ConnectionsCount == 0)
        {
            userRegInfo.ConnectionsCount = 1;
            userRegInfo.PeriodStartDate = DateTime.Now;
            return true;
        }

        if (DateTime.Now - userRegInfo.PeriodStartDate < TimeSpan.FromSeconds(Delay))
        {
            return false;
        }

        userRegInfo.ConnectionsCount = 1;
        userRegInfo.PeriodStartDate = DateTime.Now;
        return true;
    }
}