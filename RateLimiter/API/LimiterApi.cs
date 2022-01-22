using RateLimiter.Domain;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.API
{
    public class LimiterApi
    {
        public void Call(RequestModel request)
        {
            Domain.RateLimiter.EnqueuRequest(request);
            bool isRulesOk = Domain.RateLimiter.Check(request);
            if (isRulesOk) { }
                //process requests
            else { }
                //wait
        }
    }
}
