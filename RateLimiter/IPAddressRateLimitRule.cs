using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class IPAddressRateLimitRule : RateLimitRule
    {
        private readonly int _limit;
        private readonly TimeSpan _timespan;
        private readonly ConcurrentDictionary<string, List<DateTime>> _requests = new();
        private readonly Func<string, string> _getIPAddress;

        public IPAddressRateLimitRule(int limit, TimeSpan timespan, Func<string, string> getIPAddress)
        {
            _limit = limit;
            _timespan = timespan;
            _getIPAddress = getIPAddress;
        }

        public override bool AllowRequest(string clientId)
        {
            var now = DateTime.UtcNow;
            var ipAddress = _getIPAddress(clientId);
            _requests.AddOrUpdate(ipAddress, new List<DateTime> { now }, (key, list) =>
            {
                list.Add(now);
                list.RemoveAll(t => t < now - _timespan);
                return list;
            });

            return _requests[ipAddress].Count <= _limit;
        }
    }

}
