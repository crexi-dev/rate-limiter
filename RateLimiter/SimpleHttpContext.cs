using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
    public record SimpleHttpContext(string SourceName, string UserToken, DateTime RequestDT);
}
