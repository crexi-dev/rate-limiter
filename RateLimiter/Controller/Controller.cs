using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Controller
{
    internal class Controller
    {
        private readonly IRateLimiterManager _rate_limiter_service;

        public Controller(IRateLimiterManager rate_limiter_service)
        {
            _rate_limiter_service = rate_limiter_service;
        }

        public int ApiFunction1(IClientRequest request)
        {
            if (!_rate_limiter_service.Validate("ApiFunction1", request))
                return -1;

            return 100;
        }

        public int ApiFunction2(IClientRequest request)
        {
            if (!_rate_limiter_service.Validate("ApiFunction2", request))
                return -1;

            return 200;
        }

    }
}
