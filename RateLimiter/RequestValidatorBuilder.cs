
using RateLimiter.Interfaces;
using RateLimiter.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RateLimiter
{
    public class RequestValidatorBuilder
    {
        private readonly List<IRateLimiter> _rateLimiters = new();


        public RequestValidatorBuilder(List<IRateLimiter> rateLimiter)
        {
            _rateLimiters = rateLimiter;
        }


        public bool ValidateClientToken(string token)
        {
            if(!string.IsNullOrWhiteSpace(token))
            {
                foreach (var item in _rateLimiters)
                {
                    return item.ApplyRule(token);
                }
            }
            return false;
        }
    }
}
