using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Common
{
    public interface IRateLimiterRepository
    {
        DateTime? GetLastLoginDateTime(Guid ClientId);

        int GetAmountOfLoginsSinceTimespan(Guid ClientId, TimeSpan span);


    }
}
