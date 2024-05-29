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
        private readonly IDateTimeWrapper _dateTimeWrapper;

        public XRequestsPerTimespanRule(int requestLimit, TimeSpan timeSpan, IDateTimeWrapper dateTimeWrapper)
        {
            _requestLimit = requestLimit;
            _timeSpan = timeSpan;
            _tokenRequests = new Dictionary<string, (int, DateTime)>();
            _dateTimeWrapper = dateTimeWrapper;
        }

        public bool IsAllowed(string token)
        {
            lock (_tokenRequests)
            {
                if (!_tokenRequests.TryGetValue(token, out var requestInfo))
                {
                    requestInfo = (0, DateTime.MinValue); // Set to a time in the past
                }

                var now = _dateTimeWrapper.UtcNow;
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
