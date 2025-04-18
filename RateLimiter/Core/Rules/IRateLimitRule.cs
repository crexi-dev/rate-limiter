using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Core.Rules
{
    public interface IRateLimitRule
    {
        bool IsAllowed(string clientToken, string resourceId);
    }
}