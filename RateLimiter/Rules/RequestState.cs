using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Rules
{
    /// <summary>
    /// The current processing state of the request by the rules engine
    /// </summary>
    public enum RequestState
    {
        Unhandled = 0,
        Accepted,
        Denied,
    }
}
