using RateLimiter.BL.ServicesInterfaces;
using RateLimiter.Data;
using RateLimiter.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.BL.Services
{
    public class RequestPerTimespanRuleService : IRaceLimiterRuleService
    {
        public RateLimiterTypeEnum Type => RateLimiterTypeEnum.RequestPerTimespan;

        private readonly IRequestRepository _requestRepository;
        private const int RPSecondValue = 1;


        public RequestPerTimespanRuleService(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<bool> CheckRaceLimiterRule(Request currentRequest)
        {
            var allRequest = await _requestRepository.Get(currentRequest.GetIdentifier());
            var timeSpan = currentRequest.RequestAtDateTime.AddSeconds(-1);

            var actualCount = allRequest.Count(t => t.RequestAtDateTime >= timeSpan);

            return actualCount <= RPSecondValue;
        }
    }
}
