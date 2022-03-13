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
    public class DebounceRuleService : IRaceLimiterRuleService
    {
        public RateLimiterTypeEnum Type => RateLimiterTypeEnum.RequestDebounce;

        private readonly IRequestRepository _requestRepository;
        private const int DebounceTimeSecond = 3;


        public DebounceRuleService(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<bool> CheckRaceLimiterRule(Request currentRequest)
        {
            var allRequest = await _requestRepository.Get(currentRequest.GetIdentifier());

            var lastRequest = allRequest.LastOrDefault();
            if (lastRequest == null)
            {
                return true;
            }
            var actualDebounce = currentRequest.RequestAtDateTime - lastRequest.RequestAtDateTime;
            return actualDebounce.TotalSeconds >= DebounceTimeSecond;
        }
    }
}
