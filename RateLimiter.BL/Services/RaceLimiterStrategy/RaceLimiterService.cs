using RateLimiter.BL.ServicesInterfaces;
using RateLimiter.Data;
using RateLimiter.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace RateLimiter.BL.Services
{
    public class RaceLimiterService : IRaceLimiterService
    {
        private readonly IServiceProvider _provider;
        private readonly ICollection<IRaceLimiterRuleService> _services;

        public RaceLimiterService(IServiceProvider provider)
        {
            _provider = provider;
            _services = _provider.GetServices<IRaceLimiterRuleService>().ToList();
        }

        public async Task<bool> ProcessLimiters(RateLimiterTypeEnum type, Request currentRequest)
        {
            if (type.HasFlag(RateLimiterTypeEnum.AutoByRegion))
            {
                return await ProcessLimiters(GetLimiterForRegion(type, currentRequest), currentRequest);
            }

            foreach (RateLimiterTypeEnum flag in Enum.GetValues(typeof(RateLimiterTypeEnum)))
            {
                if (type.HasFlag(flag))
                {
                    var result = await _services.Single(t => t.Type == flag).CheckRaceLimiterRule(currentRequest);
                    if(result == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private RateLimiterTypeEnum GetLimiterForRegion(RateLimiterTypeEnum type, Request currentRequest)
        {
            //Remove Auto
            type &= ~RateLimiterTypeEnum.AutoByRegion;

            if(currentRequest.GetRegion() == RegionTypeEnum.USA)
            {
                return type | RateLimiterTypeEnum.RequestPerTimespan;
            }
            return type | RateLimiterTypeEnum.RequestDebounce;
        }
    }
}
