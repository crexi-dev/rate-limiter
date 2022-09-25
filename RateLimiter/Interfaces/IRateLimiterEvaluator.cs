using RateLimiter.Enumerators;
using RateLimiter.Models;
using RateLimiter.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Interfaces
{
    public interface IRateLimiterEvaluator
    {
        eRateLimiterResultType Evaluate(RateLimiterHandlerContext context);
    }
}
