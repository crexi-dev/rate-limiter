using RateLimiter.Interfaces;
using RateLimiter.Models.Request;
using System;

namespace RateLimiter.Rules
{
    public class PeriodBetweenRequests : IRateLimitRule
    {
        public string? Endpoint { get; set; }

        public TimeSpan Period { get; set; }

        public int Limit { get; set; }

        public bool ValidateRequest(ClientRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
