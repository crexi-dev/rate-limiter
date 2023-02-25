using System;

namespace RateLimiter
{
    public class TimespanPassedSinceTheLastStrategy : IRateLimitStrategy
    {
        private readonly TimeSpan _interval;
        private readonly ITimeService _timeService;

        public TimespanPassedSinceTheLastStrategy(TimeSpan interval,ITimeService timeService)
        {
            _interval = interval;
            _timeService = timeService;
        }

        public bool IsRequestAllowed(string username, string resource, IRequestRepository requestRepository, string region)
        {
            var lastRequest = requestRepository.GetLastRequest(username, resource);
            if (lastRequest is null)
                return true;
            var now = _timeService.GetCurrentTime;
            var earliestTime = now - lastRequest.RequestTime;
            if (earliestTime < _interval)
            {
                return false;
            }
            return true;
        }
    }
}
