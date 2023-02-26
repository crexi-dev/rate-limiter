using System;
using System.Collections.Generic;

namespace RateLimiter.Rules
{
    public interface IRule
    {
        bool IsRequestAllowed(string resource, string client);
    }
}