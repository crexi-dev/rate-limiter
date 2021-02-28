using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class LimiterService
    {
        private readonly ConcurrentDictionary<string, ITracker> _limitTracker;
        private readonly IEndpointService _endpointService;

        public LimiterService(IEndpointService endpointService)
        {
            _endpointService = endpointService;
            _limitTracker = new ConcurrentDictionary<string, ITracker>();
        }

        public void Configure(LimiterConfiguration config)
        {
            if(config == null)
            {
                throw new ArgumentException("Limiter is not configured");
            }
            foreach (var lr in config.LimitRules)
            {
                foreach (var endpoint in lr.EndPoints)
                {
                    switch (lr.RuleType)
                    {
                        case RuleType.RequestsPerPeriod:
                            _limitTracker.TryAdd(endpoint, new RequestPerPeriodTracker(lr.Period, lr.Value));
                            break;
                        case RuleType.LastCallPassed:
                            _limitTracker.TryAdd(endpoint, new LastCallPassedTracker(lr.Period, lr.Value));
                            break;
                        default:
                            throw new ArgumentException("Rule type not recognized");
                    }
                }
            }
        }

        public async Task<bool> ProccessRequest(string accessToken)
        {
            var endpoint = await _endpointService.GetEndPoint(accessToken);
            if (!_limitTracker.ContainsKey(endpoint))
            {
                return true;
            }
            return _limitTracker[endpoint].Track();
        }
    }
}
