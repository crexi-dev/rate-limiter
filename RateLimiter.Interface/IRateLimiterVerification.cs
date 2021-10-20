using RateLimiter.Model;
using System;
using System.Collections.Generic;

namespace RateLimiter.Interface
{
    public interface IRateLimiterVerification
    {
        bool VerifyAccess(IEnumerable<Request> requests);
    } 
}


