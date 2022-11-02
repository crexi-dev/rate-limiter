using RateLimiter.Interfaces;
using RateLimiter.Models.Request;
using System;

namespace RateLimiter.Rules
{
    public class NumberRequestsPerTimeSpan : Rule, IRateLimitRule
    {
        public bool ValidateRequest(ClientRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
