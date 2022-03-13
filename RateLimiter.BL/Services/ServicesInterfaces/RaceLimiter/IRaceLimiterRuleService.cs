using RateLimiter.Data;
using RateLimiter.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.BL.ServicesInterfaces
{
    public interface IRaceLimiterRuleService
    {
        RateLimiterTypeEnum Type { get; }

        Task<bool> CheckRaceLimiterRule(Request currentRequest);
    }
}
