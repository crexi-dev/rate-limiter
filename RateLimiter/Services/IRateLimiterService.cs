using RateLimiter.Models;
using System;
using System.Collections.Generic;

namespace RateLimiter.Services
{
    public interface IRateLimiterService
    {
        IList<RateLimiterProcessorResponse> ProcessRequest(string clientId, DateTime newRequestTime);
    }
}
