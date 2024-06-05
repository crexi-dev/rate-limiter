using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RequestCountPerTimespanRule : RateLimitRule
    {
        private readonly int _limit;
        private readonly TimeSpan _timespan;
        private readonly ConcurrentDictionary<string, List<DateTime>> _requests = new();

        public RequestCountPerTimespanRule(int limit, TimeSpan timespan)
        {
            _limit = limit;
            _timespan = timespan;
        }

        public override bool AllowRequest(string clientId)
        {
            var now = DateTime.UtcNow;
            _requests.AddOrUpdate(clientId, new List<DateTime> { now }, (key, list) =>
            {
                list.Add(now);
                list.RemoveAll(t => t < now - _timespan);
                return list;
            });

            return _requests[clientId].Count <= _limit;
        }
    }

}
