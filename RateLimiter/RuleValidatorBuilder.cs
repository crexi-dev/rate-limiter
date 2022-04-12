
using RateLimiter.Interfaces;
using RateLimiter.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RateLimiter
{
    public class RuleValidatorBuilder
    {
        private readonly List<IRateLimiter> _rateLimiters = new List<IRateLimiter>();


        public RuleValidatorBuilder(List<IRateLimiter> rateLimiter)
        {
            _rateLimiters = rateLimiter;
        }


        public bool ValidateClientToken(string token)
        {
            bool isValid = false;
            if(!string.IsNullOrWhiteSpace(token))
            {
                foreach (var item in _rateLimiters)
                {
                    isValid =  item.ApplyRule(token);
                }
            }
            return isValid;
        }
    }
}
