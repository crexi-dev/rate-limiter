using System;

namespace RateLimiter
{
    public class CertainTimespanPassedSinceTheLastCall : IRateLimitStrategy
    {
        private readonly TimeSpan _interval;

        public CertainTimespanPassedSinceTheLastCall(TimeSpan interval)
        {
            _interval = interval;
        }

        public bool IsRequestAllowed(string username, string resource, IRequestRepository requestRepository)
        {
            var lastRrequest = requestRepository.GetLastRequest(username, resource);
            if (lastRrequest is null)
                return true;
            var now = DateTime.UtcNow;
            var earliestTime = now - lastRrequest.RequestTime;
            if (earliestTime > _interval)
            {
                return false;
            }
            return true;
        }
    }

}
