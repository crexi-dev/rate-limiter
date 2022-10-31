using RateLimiter.Models.Request;
using System;

namespace RateLimiter.Interfaces
{
    public interface IRateLimitRule
    {
        string? Endpoint { get; set; }

        TimeSpan Period { get; set; }

        int Limit { get; set; }

        bool ValidateRequest(ClientRequest request);
    }
}
