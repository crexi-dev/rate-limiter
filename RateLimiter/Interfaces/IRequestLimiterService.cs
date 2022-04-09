using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Interfaces
{
    public interface IRequestLimiterService
    {
        bool MakeRequest(string token, string apiKey);
    }
}
