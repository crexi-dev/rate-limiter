using RateLimiter.Interfaces;
using RateLimiter.Models;

namespace RateLimiterRules.Rules
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

        public RuleCheckResult CheckRule(string token)
        {
            lock (_tokenRequests)
            {
                if (!_tokenRequests.TryGetValue(token, out var requestInfo))
                {
                    requestInfo = (0, DateTime.MinValue);
                }

                var now = _dateTimeWrapper.UtcNow;
                if (now - requestInfo.LastRequestTime > _timeSpan)
                {
                    _tokenRequests[token] = (1, now);
                    return new RuleCheckResult(true);
                }

                if (requestInfo.Count < _requestLimit)
                {
                    _tokenRequests[token] = (requestInfo.Count + 1, now);
                    return new RuleCheckResult(true);
                }

                return new RuleCheckResult(false,
                    "Request limit exceeded for this time span.",
                    "XRequestsPerTimespanExceeded");
            }
        }
    }
}
