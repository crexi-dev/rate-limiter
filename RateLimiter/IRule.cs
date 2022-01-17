using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
    public interface IRule
    {
        bool CanAccess(string token, ClientLogsStorage storage);

    }
}
