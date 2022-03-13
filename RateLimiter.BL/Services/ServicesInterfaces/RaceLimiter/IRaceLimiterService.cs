using RateLimiter.Data;
using RateLimiter.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.BL.ServicesInterfaces
{
    public interface IRaceLimiterService
    {
        Task<bool> ProcessLimiters(RateLimiterTypeEnum type, Request currentRequest);
    }
}
