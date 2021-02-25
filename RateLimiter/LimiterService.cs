using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class LimiterService
    {
        private readonly Dictionary<string, ITracker> _limitTracker;
        private readonly IEndpointService _endpointService;

        public LimiterService(IEndpointService endpointService)
        {
            _endpointService = endpointService;
            _limitTracker = new Dictionary<string, ITracker>();
        }

        public void Configure(LimiterConfiguration config)
        {
            foreach (var lr in config.LimitRules)
            {
                foreach (var endpoint in lr.EndPoints)
                {
                    switch (lr.RuleType)
                    {
                        case RuleType.RequestsPerPeriod:
                            _limitTracker.Add(endpoint, new RequestPerPeriodTracker(lr.Period, lr.Value));
                            break;
                        case RuleType.LastCallPassed:
                            _limitTracker.Add(endpoint, new LastCallPassedTracker(lr.Period, lr.Value));
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
            return _limitTracker[endpoint].Track();
        }
    }
}
