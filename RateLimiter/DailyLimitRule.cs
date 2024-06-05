using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class DailyLimitRule : RateLimitRule
    {
        private readonly int _dailyLimit;
        private readonly ConcurrentDictionary<string, List<DateTime>> _requests = new();

        public DailyLimitRule(int dailyLimit)
        {
            _dailyLimit = dailyLimit;
        }

        public override bool AllowRequest(string clientId)
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            _requests.AddOrUpdate(clientId, new List<DateTime> { now }, (key, list) =>
            {
                list.Add(now);
                list.RemoveAll(t => t.Date < today);
                return list;
            });

            return _requests[clientId].Count <= _dailyLimit;
        }
    }

}
