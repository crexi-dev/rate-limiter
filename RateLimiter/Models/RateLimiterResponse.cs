using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Models
{
    public class RateLimiterResponse
    {
        public bool WasAllowed { get; set; }
        public int? StatusCode { get; set; }
        public string? ErrorMessage { get; set; }

        public static RateLimiterResponse Allow()
        {
            return new RateLimiterResponse { WasAllowed = true };
        }

        public static RateLimiterResponse Deny(int? statusCode = null, string? errorMessage = null)
        {
            return new RateLimiterResponse { WasAllowed = false, StatusCode = statusCode, ErrorMessage = errorMessage };
        }
    }
}
