using System;
using System.Collections.Generic;
using RateLimiter.Interfaces;

namespace RateLimiter.Rules
{
    public class XRequestsPerTimespanRule : IRule
    {
        private readonly int _requestLimit;
        private readonly TimeSpan _timeSpan;
        private readonly Dictionary<string, (int Count, DateTime LastRequestTime)> _tokenRequests;

        public XRequestsPerTimespanRule(int requestLimit, TimeSpan timeSpan)
        {
            _requestLimit = requestLimit;
            _timeSpan = timeSpan;
            _tokenRequests = new Dictionary<string, (int, DateTime)>();
        }

        public bool IsAllowed(string token)
        {
            lock (_tokenRequests)
            {
                if (!_tokenRequests.TryGetValue(token, out var requestInfo))
                {
                    requestInfo = (0, DateTime.MinValue);
                }

                var now = DateTime.UtcNow;
                if (now - requestInfo.LastRequestTime > _timeSpan)
                {
                    _tokenRequests[token] = (1, now);
                    return true;
                }

                if (requestInfo.Count < _requestLimit)
                {
                    _tokenRequests[token] = (requestInfo.Count + 1, now);
                    return true;
                }

                return false;
            }
        }
    }
}
