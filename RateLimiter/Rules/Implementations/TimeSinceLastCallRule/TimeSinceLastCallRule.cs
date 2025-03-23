using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using RateLimiter.Core;
using RateLimiter.Rules.Base;

namespace RateLimiter.Rules.Implementations.TimeSinceLastCallRule
{
    public class TimeSinceLastCallRule(TimeSpan minimumInterval) : RateLimiterRuleBase
    {
        private readonly TimeSpan _minimumInterval = minimumInterval;

        private readonly ConcurrentDictionary<string, DateTime> _lastRequestTime = new();
        public override string Description { get { return "200 requests allowed per rolling 10-minute window."; } }
        public override string Name { get { return "TimeSinceLastCallRule"; } }
        public override string ViolationMessage { get { return "Exceeded requests limit."; } }
        
        public override Task<bool> IsRequestAllowedAsync(ClientRequestContext context)
        {
            // Logic goes here

            return Task.FromResult(true);

        }
    }
}
