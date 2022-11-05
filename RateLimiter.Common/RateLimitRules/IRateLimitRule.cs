using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using RateLimiter.Models;


namespace RateLimiter.RateLimitRules
{
    public interface IRateLimitRule
    {
        public bool IsAllowed(List<RequestEvent> requestEvents);
    }
}
