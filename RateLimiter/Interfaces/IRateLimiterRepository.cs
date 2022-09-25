using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Interfaces
{
    public interface IRateLimiterRepository
    {
        void SaveUserRequest(string userToken, IDateTimeWrapper dateTime);

        int CountRequests(string userToken, IDateTimeWrapper now, TimeSpan timeSpan);

        void Cleanup(DateTime removeBeforeDate);

    }
}
