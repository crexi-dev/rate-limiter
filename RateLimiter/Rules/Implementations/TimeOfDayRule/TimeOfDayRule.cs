using RateLimiter.Core;
using RateLimiter.Rules.Base;
using RateLimiter.Rules.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Rules.Implementations.TimeOfDayRule
{
    public class TimeOfDayRule(int limit, TimeSpan start, TimeSpan final, TimeSpan period) : RateLimiterRuleBase, IRateLimiterNotification
    {
        private readonly int _limit = limit;
        private readonly TimeSpan _start = start;
        private readonly TimeSpan _end = final;
        private readonly TimeSpan _period = period;

        public override string Description { get { return " Limit stricter during peak hours (e.g., 9 AM - 5 PM)."; } }
        public override string Name { get { return "TimeOfDayRule"; } }
        public override string ViolationMessage { get { return $"Exceeded {_limit} requests per {_period} allowed between {_start} and {_end} UTC."; } }

        public override Task<bool> IsRequestAllowedAsync(ClientRequestContext context)
        {
            // Logic goes here

            return Task.FromResult(true);
        }

        public Task<bool> SendNotificationAsync(string recipient, string message)
        {
            // Logic to send notification

            return Task.FromResult(true);
        }
    }
}
