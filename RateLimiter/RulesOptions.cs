using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
    public class RulesOptions
    {
        public TimePassedRuleOptions CertainTimePassedRule { get; set; }

    }

    /// <summary>
    /// Configuration for Time Passed Rule Options
    /// </summary>
    public class TimePassedRuleOptions
    {
        public TimeSpan MinTimespan { get; set; }

    }

    /// <summary>
    /// Configuration for Requests Per Time Span rules
    /// </summary>
    public class RequestPerTimeSpanOptions
    {
        public int MaxAlloweRequests { get; set; }

        public TimeSpan WithinTimeSpan { get; set; }
    }
}
